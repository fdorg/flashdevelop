using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;

namespace ASCompletion.Helpers
{
    delegate void CacheUpdated();

    internal class ASTCache
    {
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
                //FIXME: need to make sure this is called AFTER the Context has cached everything
                SpinWait.SpinUntil(() => !PathExplorer.IsWorking); //wait for it to finish

                //Thread.Sleep(1500);
                var c = new Dictionary<ClassModel, CachedClassModel>();
                
                var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
                foreach (MemberModel cls in context.GetAllProjectClasses())
                {
                    var clas = GetClassModel(c, cls);
                    clas.ResolveExtends();

                    var cachedClassModel = GetOrCreate(c, clas);

                    //look for functions / variables in clas that originate from interfaces of clas
                    var interfaces = ResolveInterfaces(clas);

                    if (interfaces.Count > 0)
                    {
                        //look at each member and see if one of them is defined in an interface of clas
                        foreach (MemberModel member in clas.Members)
                        {
                            //if ((member.Flags & FlagType.Function) == 0 && (member.Flags & FlagType.Variable) == 0) continue;

                            var implementing = GetDefiningInterfaces(member, interfaces).ToHashSet();

                            if (implementing.Count == 0) continue;

                            cachedClassModel.Implementing.AddUnion(member, implementing);
                            //now that we know member is implementing the interfaces in implementing, we can add clas as implementor for them
                            foreach (var interf in implementing)
                            {
                                var cachedModel = GetOrCreate(c, interf);
                                var set = CacheHelper.GetOrCreateSet(cachedModel.Implementors, interf.Members.Search(member.Name, 0, 0)); //TODO: search is done already in GetDefiningInterfaces
                                set.Add(clas);
                            }

                        }
                    }

                    //look for functions in clas that originate from a super-class
                    foreach (MemberModel member in clas.Members)
                    {
                        if ((member.Flags & (FlagType.Function | FlagType.Override)) > 0)
                        {
                            var overridden = GetOverrideParents(clas, member).ToHashSet();

                            if (overridden == null || overridden.Count <= 0) continue;

                            cachedClassModel.Overriding.AddUnion(member, overridden);
                            //now that we know member is overriding the classes in overridden, we can add clas as overrider for them
                            foreach (var over in overridden)
                            {
                                var cachedModel = GetOrCreate(c, over);
                                var set = CacheHelper.GetOrCreateSet(cachedModel.Overriders, over.Members.Search(member.Name, FlagType.Function, member.Access)); //TODO: this is doing work twice
                                set.Add(clas);
                            }
                        }
                    }

                    if (cachedClassModel.Implementing.Count == 0 && cachedClassModel.Implementors.Count == 0 &&
                        cachedClassModel.Overriders.Count == 0 && cachedClassModel.Overriding.Count == 0)
                        c.Remove(clas);
                }

                cache = c;
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
        HashSet<ClassModel> ResolveExtends(ClassModel cls)
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
        /// <returns></returns>
        internal HashSet<ClassModel> GetDefiningInterfaces(MemberModel member, HashSet<ClassModel> interfaces)
        {
            //if ((member.Flags & FlagType.Function) == 0 && (member.Flags & FlagType.Variable) == 0) return null;

            //look for all ClassModels with variables / functions of the same name as member
            //this could give faulty results if there are variables / functions of the same name with different signature in the interface
            var implementors = interfaces.Where(interf => interf.Members.Search(member.Name, 0, 0) != null).ToHashSet();
            
            //var parentInterfaces = interfaces.Select(interf => interf.Extends).Where(e => !e.IsVoid()).ToHashSet();
            //if (parentInterfaces.Count == 0) return implementors;

            //implementors.UnionWith(GetDefiningInterfaces(member, parentInterfaces));

            return implementors;
        }

        /// <summary>
        /// Gets all ClassModels that are a super-class of the given <paramref name="cls"/> and contain a function that is overridden
        /// by <paramref name="function"/>
        /// </summary>
        internal HashSet<ClassModel> GetOverrideParents(ClassModel cls, MemberModel function)
        {
            if (cls.Extends == null || cls.Extends.IsVoid()) return null;
            if ((function.Flags & FlagType.Function) == 0 || (function.Flags & FlagType.Override) == 0) return null;

            var parentFunctions = new HashSet<ClassModel>();

            var currentParent = cls.Extends;
            while (currentParent != null && !currentParent.IsVoid())
            {
                var parentFun = currentParent.Members.Search(function.Name, FlagType.Function, function.Access); //can it have a different access?
                //it should not be necessary to check the parameters in Haxe, because two functions with different signature cannot have the same name

                if (parentFun != null)
                    parentFunctions.Add(currentParent);

                currentParent = currentParent.Extends;
            }

            return parentFunctions;
        }

        #region No longer needed code

        //public void UpdateCache(ClassModel clas, CacheUpdated finished)
        //{
        //    var action = new Action(() =>
        //    {
        //        clas.ResolveExtends();
        //        var subClasses = helper.FindSubClasses(clas);

        //        var cachedClassModel = new CachedClassModel();

        //        //look for functions / variables in clas that originate from interfaces of clas
        //        var interfaces = helper.ResolveInterfaces(clas);

        //        if (interfaces.Count > 0)
        //        {
        //            //look at each member and see if one of them is defined in an interface of clas
        //            foreach (MemberModel member in clas.Members)
        //            {
        //                //if ((member.Flags & FlagType.Function) == 0 && (member.Flags & FlagType.Variable) == 0) continue;

        //                var implementing = helper.GetDefiningInterfaces(member, interfaces).ToHashSet();

        //                if (implementing.Count == 0) continue;

        //                cachedClassModel.Implementing.AddUnion(member, implementing);
        //                //now that we know member is implementing the interfaces in implementing, we can add clas as implementor for them
        //                foreach (var interf in implementing)
        //                {
        //                    var cachedModel = GetOrCreate(interf);
        //                    var set = CacheHelper.GetOrCreateSet(cachedModel.Implementors, interf.Members.Search(member.Name, 0, 0)); //TODO: search is done already in GetDefiningInterfaces
        //                    set.Add(clas);
        //                }

        //            }
        //        }

        //        //look for functions in clas that originate from a super-class
        //        foreach (MemberModel member in clas.Members)
        //        {
        //            if ((member.Flags & FlagType.Function) > 0)
        //            {
        //                var overriders = helper.FindOverriders(subClasses, member).ToHashSet();

        //                if (overriders.Count > 0)
        //                {
        //                    cachedClassModel.Overriders.AddUnion(member, overriders);

        //                    //now that we know member is overridden by overriders, we can add this to the cached classes of the overriders
        //                    foreach (var over in overriders)
        //                    {
        //                        var cachedModel = GetOrCreate(over);
        //                        var set = CacheHelper.GetOrCreateSet(cachedModel.Overriding, over.Members.Search(member.Name, FlagType.Override, member.Access)); //access should probably be the same
        //                        //TODO: search is done already in FindOverriders
        //                        set.Add(clas);
        //                    }
        //                }

        //                if ((member.Flags & FlagType.Override) > 0)
        //                {
        //                    var overridden = helper.GetOverrideParents(clas, member).ToHashSet();

        //                    if (overridden.Count <= 0) continue;

        //                    cachedClassModel.Overriding.AddUnion(member, overridden);
        //                    //now that we know member is overriding the classes in overridden, we can add clas as overrider for them
        //                    foreach (var over in overridden)
        //                    {
        //                        var cachedModel = GetOrCreate(over);
        //                        var set = CacheHelper.GetOrCreateSet(cachedModel.Overriders, over.Members.Search(member.Name, FlagType.Function, member.Access)); //TODO: this is doing work twice
        //                        set.Add(clas);
        //                    }

        //                }
        //            }
        //        }

        //        //if clas is an interface, check for classes that implement it
        //        if ((clas.Flags & FlagType.Interface) > 0)
        //        {
        //            var implementors = helper.FindImplementors(clas);
        //            if (implementors.Count > 0)
        //            {
        //                //look at each member and see which implementors actually implement it
        //                foreach (MemberModel member in clas.Members)
        //                {
        //                    var implementingMembers = new HashSet<ClassModel>();
        //                    foreach (var implementor in implementors)
        //                    {
        //                        var implementedMember = implementor.Members.Search(member.Name, member.Flags, 0);
        //                        if (implementedMember != null)
        //                            implementingMembers.Add(implementor);
        //                    }

        //                    if (implementingMembers.Count > 0) //should always be the case?
        //                    {
        //                        cachedClassModel.Implementors.AddUnion(member, implementingMembers);
        //                        //now that we know member is implemented by implementingMembers, we can add this to the cached classes of the implementingMembers
        //                        foreach (var implementing in implementingMembers)
        //                        {
        //                            var cachedModel = GetOrCreate(implementing);

        //                            var set = CacheHelper.GetOrCreateSet(cachedModel.Implementing, implementing.Members.Search(member.Name, member.Flags, 0)); //TODO: this is doing work twice
        //                            set.Add(clas);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        cache.Remove(clas);
        //        cache.Add(clas, cachedClassModel);

        //        PluginBase.RunAsync(new MethodInvoker(finished));
        //    });
        //    action.BeginInvoke(null, null);
        //}

        ///// <summary>
        ///// Returns a set of ClassModels that implement the given interface <paramref name="interf"/>
        ///// </summary>
        //internal HashSet<ClassModel> FindImplementors(ClassModel interf)
        //{
        //    var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
        //    var implementingClasses = context.GetAllProjectClasses().Items
        //        .Select(m => context.ResolveType(m.FullName, null));

        //    implementingClasses = implementingClasses.Where(clazz => clazz != null && !clazz.IsEnum() && Implements(clazz, interf));

        //    //TODO: are conflicting function definitions in different interfaces possible?
        //    return implementingClasses.ToHashSet();
        //}

        //internal HashSet<ClassModel> FindOverriders(HashSet<ClassModel> subClasses, MemberModel function)
        //{
        //    return subClasses.Where(clazz => clazz.Members.Items.Exists(m => m.Name == function.Name && (m.Flags & FlagType.Override) > 0)).ToHashSet();
        //}

        ///// <summary>
        ///// Returns a set of ClassModels that override the given function
        ///// </summary>
        //internal HashSet<ClassModel> FindSubClasses(ClassModel cls)
        //{
        //    var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
        //    var subClasses = context.GetAllProjectClasses().Items
        //        .Select(m => context.ResolveType(m.FullName, null))
        //        .Where(clazz => clazz != null && !clazz.IsEnum() && Extends(clazz, cls));

        //    return subClasses.ToHashSet();
        //}

        ///// <summary>
        ///// Returns true if <paramref name="class1"/> extends <paramref name="class2"/>
        ///// </summary>
        //static bool Extends(ClassModel class1, ClassModel class2)
        //{
        //    if (class1 == null || class2 == null)
        //        return false;

        //    class1.ResolveExtends();
        //    return !class1.Extends.IsVoid() && (class1.Extends.Equals(class2) || Extends(class1.Extends, class2));
        //}

        ///// <summary>
        ///// Returns true if <paramref name="clazz"/> implements <paramref name="interf"/>
        ///// </summary>
        //bool Implements(ClassModel clazz, ClassModel interf)
        //{
        //    if (clazz == null || interf == null || Equals(clazz, interf))
        //        return false;

        //    var clazzInterfaces = ResolveInterfaces(clazz);

        //    if (clazzInterfaces.Contains(interf)) return true; //clazz directly implements interf

        //    return !clazz.Extends.IsVoid() && Implements(clazz.Extends, interf); //parent class implements interf
        //}

        #endregion

        static CachedClassModel GetOrCreate(Dictionary<ClassModel, CachedClassModel> cache, ClassModel cls)
        {
            CachedClassModel cached;
            cache.TryGetValue(cls, out cached);
            if (cached != null) return cached;

            cached = new CachedClassModel();
            cache.Add(cls, cached);
            return cached;
        }

        static ClassModel GetClassModel(Dictionary<ClassModel, CachedClassModel> cache, MemberModel clas)
        {
            var pos = clas.Type.LastIndexOf('.');
            var package = pos == -1 ? "" : clas.Type.Substring(0, pos);
            var name = clas.Type.Substring(pos + 1);

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

        //TODO: investigate whether this one can be done on the fly
        /// <summary>
        /// Contains a set of interfaces - for each member - that contain the member.
        /// </summary>
        public Dictionary<MemberModel, HashSet<ClassModel>> Implementing = new Dictionary<MemberModel, HashSet<ClassModel>>();

        //TODO: investigate whether this one can be done on the fly
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

        internal static void AddUnion<S, T>(this Dictionary<S, HashSet<T>> dict, S key, HashSet<T> value)
        {
            var set = GetOrCreateSet(dict, key);

            set.UnionWith(value);
        }

        internal static ISet<T> GetOrCreateSet<S, T>(Dictionary<S, HashSet<T>> dict, S key)
        {
            HashSet<T> set;
            dict.TryGetValue(key, out set);

            if (set != null) return set;

            set = new HashSet<T>();
            dict.Add(key, set);
            return set;
        }
    }
}
