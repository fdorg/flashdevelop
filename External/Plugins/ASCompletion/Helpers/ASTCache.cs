using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;

namespace ASCompletion.Helpers
{
    delegate void CacheUpdated();

    internal class ASTCache
    {
        internal bool IsDirty { get; set; } = true;

        Dictionary<ClassModel, CachedClassModel> cache = new Dictionary<ClassModel, CachedClassModel>();

        public CachedClassModel GetCachedModel(ClassModel cls)
        {
            CachedClassModel v;
            cache.TryGetValue(cls, out v);
            return v;
        }

        /// <summary>
        /// Update the cache for the whole project
        /// </summary>
        public void UpdateCache(CacheUpdated finished)
        {
            var action = new Action(() =>
            {
                var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
                if (context.Classpath == null || !IsDirty || PathExplorer.IsWorking)
                {
                    PluginBase.RunAsync(new MethodInvoker(finished));
                    return;
                }

                var c = new Dictionary<ClassModel, CachedClassModel>();

                foreach (MemberModel memberModel in context.GetAllProjectClasses())
                {
                    if (PluginBase.MainForm.ClosingEntirely)
                        return; //make sure we leave if the form is closing, so we do not block it

                    var cls = GetClassModel(memberModel);
                    cls.ResolveExtends();

                    var cachedClassModel = GetOrCreate(c, cls);

                    //look for functions / variables in cls that originate from interfaces of cls
                    var interfaces = ResolveInterfaces(cls);

                    if (interfaces.Count > 0)
                    {
                        //look at each member and see if one of them is defined in an interface of cls
                        foreach (MemberModel member in cls.Members)
                        {
                            var implementing = GetDefiningInterfaces(member, interfaces);

                            if (implementing.Count == 0) continue;

                            cachedClassModel.Implementing.AddUnion(member, implementing.Keys);
                            //now that we know member is implementing the interfaces in implementing, we can add cls as implementor for them
                            foreach (var interf in implementing)
                            {
                                var cachedModel = GetOrCreate(c, interf.Key);
                                var set = CacheHelper.GetOrCreateSet(cachedModel.Implementors, interf.Value);
                                set.Add(cls);
                            }

                        }
                    }

                    //look for functions in cls that originate from a super-class
                    foreach (MemberModel member in cls.Members)
                    {
                        if ((member.Flags & (FlagType.Function | FlagType.Override)) > 0)
                        {
                            var overridden = GetOverriddenClasses(cls, member);

                            if (overridden == null || overridden.Count <= 0) continue;

                            cachedClassModel.Overriding.AddUnion(member, overridden.Keys);
                            //now that we know member is overriding the classes in overridden, we can add cls as overrider for them
                            foreach (var over in overridden)
                            {
                                var cachedModel = GetOrCreate(c, over.Key);
                                var set = CacheHelper.GetOrCreateSet(cachedModel.Overriders, over.Value);
                                set.Add(cls);
                            }
                        }
                    }

                    if (cachedClassModel.Implementing.Count == 0 && cachedClassModel.Implementors.Count == 0 &&
                        cachedClassModel.Overriders.Count == 0 && cachedClassModel.Overriding.Count == 0)
                        c.Remove(cls);
                }

                cache = c;
                IsDirty = false;
                PluginBase.RunAsync(new MethodInvoker(finished));
            });
            action.BeginInvoke(null, null);
        }

        internal HashSet<ClassModel> ResolveInterfaces(ClassModel cls)
        {
            if (cls == null || cls.IsVoid()) return new HashSet<ClassModel>();
            if (cls.Implements == null) return ResolveInterfaces(cls.Extends);

            var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            return cls.Implements
                .Select(impl => context.ResolveType(impl, cls.InFile))
                .Where(interf => interf != null && !interf.IsVoid())
                .SelectMany(interf => //take the interfaces we found already and add all interfaces they extend
                {
                    interf.ResolveExtends();
                    var set = ResolveExtends(interf);
                    set.Add(interf);
                    return set;
                })
                .Union(ResolveInterfaces(cls.Extends)).ToHashSet();
        }

        /// <summary>
        /// Returns a set of all ClassModels that <paramref name="cls"/> extends
        /// </summary>
        /// <param name="cls"></param>
        /// <returns></returns>
        static HashSet<ClassModel> ResolveExtends(ClassModel cls)
        {
            var set = new HashSet<ClassModel>();

            var current = cls.Extends;
            while (current != null && !current.IsVoid())
            {
                set.Add(current);
                current = current.Extends;
            }

            return set;
        }
        
        /// <summary>
        /// Gets all ClassModels from <paramref name="interfaces"/> and the interfaces they extend that contain a definition of <paramref name="member"/>
        /// </summary>
        /// <returns>A Dictionary containing all pairs of ClassModels and MemberModels were implemented by <paramref name="member"/></returns>
        internal Dictionary<ClassModel, MemberModel> GetDefiningInterfaces(MemberModel member, HashSet<ClassModel> interfaces)
        {
            //look for all ClassModels with variables / functions of the same name as member
            //this could give faulty results if there are variables / functions of the same name with different signature in the interface
            var implementors = new Dictionary<ClassModel, MemberModel>();

            foreach (var interf in interfaces)
            {
                var interfMember = interf.Members.Search(member.Name, 0, 0);
                if (interfMember != null)
                    implementors.Add(interf, interfMember);
            }

            return implementors;
        }

        /// <summary>
        /// Gets all ClassModels that are a super-class of the given <paramref name="cls"/> and contain a function that is overridden
        /// by <paramref name="function"/>
        /// </summary>
        /// <returns>A Dictionary containing all pairs of ClassModels and MemberModels that were overridden by <paramref name="function"/></returns>
        internal Dictionary<ClassModel, MemberModel> GetOverriddenClasses(ClassModel cls, MemberModel function)
        {
            if (cls.Extends == null || cls.Extends.IsVoid()) return null;
            if ((function.Flags & FlagType.Function) == 0 || (function.Flags & FlagType.Override) == 0) return null;

            var parentFunctions = new Dictionary<ClassModel, MemberModel>();

            var currentParent = cls.Extends;
            while (currentParent != null && !currentParent.IsVoid())
            {
                var parentFun = currentParent.Members.Search(function.Name, FlagType.Function, function.Access); //can it have a different access?
                //it should not be necessary to check the parameters, because two functions with different signature cannot have the same name (at least in Haxe)

                if (parentFun != null)
                    parentFunctions.Add(currentParent, parentFun);

                currentParent = currentParent.Extends;
            }

            return parentFunctions;
        }

        static CachedClassModel GetOrCreate(Dictionary<ClassModel, CachedClassModel> cache, ClassModel cls)
        {
            CachedClassModel cached;
            if (!cache.TryGetValue(cls, out cached))
            {
                cached = new CachedClassModel();
                cache.Add(cls, cached);
            }
            
            return cached;
        }

        static ClassModel GetClassModel(MemberModel cls)
        {
            var pos = cls.Type.LastIndexOf('.');
            var package = pos == -1 ? "" : cls.Type.Substring(0, pos);
            var name = cls.Type.Substring(pos + 1);

            var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            return context.GetModel(package, name, "");
        }
    }

    class CachedClassModel
    {
        /// <summary>
        /// If this ClassModel is an interface, this contains a set of classes - for each member - that implement the given members
        /// </summary>
        public Dictionary<MemberModel, HashSet<ClassModel>> Implementors = new Dictionary<MemberModel, HashSet<ClassModel>>();
        
        /// <summary>
        /// Contains a set of interfaces - for each member - that contain the member.
        /// </summary>
        public Dictionary<MemberModel, HashSet<ClassModel>> Implementing = new Dictionary<MemberModel, HashSet<ClassModel>>();
        
        /// <summary>
        /// Contains a set of classes - for each member - that override the member.
        /// </summary>
        public Dictionary<MemberModel, HashSet<ClassModel>> Overriders = new Dictionary<MemberModel, HashSet<ClassModel>>();

        /// <summary>
        /// Contains a set of classes - for each member - that this member overrides
        /// </summary>
        public Dictionary<MemberModel, HashSet<ClassModel>> Overriding = new Dictionary<MemberModel, HashSet<ClassModel>>();
    }

    static class CacheHelper
    {
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> e)
        {
            if (e == null) return new HashSet<T>();

            return new HashSet<T>(e);
        }

        internal static void AddUnion<S, T>(this Dictionary<S, HashSet<T>> dict, S key, IEnumerable<T> value)
        {
            var set = GetOrCreateSet(dict, key);

            set.UnionWith(value);
        }

        internal static ISet<T> GetOrCreateSet<S, T>(Dictionary<S, HashSet<T>> dict, S key)
        {
            HashSet<T> set;
            if (!dict.TryGetValue(key, out set))
            {
                set = new HashSet<T>();
                dict.Add(key, set);
            }
            return set;
        }
    }
}
