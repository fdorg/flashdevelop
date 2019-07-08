using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;

namespace ASCompletion.Model
{
    /// <summary>
    /// Object representation of an ActionScript class
    /// </summary>
    [Serializable]
    public class ClassModel: MemberModel
    {
        public static readonly ClassModel VoidClass;

        private static readonly Regex reSpacesAfterEOL = new Regex("(?<!(\n[ \t]*))(\n[ \t]+)(?!\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex reEOLAndStar = new Regex(@"[\r\n]+\s*\*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex reMultiSpacedEOL = new Regex("([ \t]*\n[ \t]*){2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex reAsdocWordSpace = new Regex("\\s+(?=\\@\\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex reAsdocWord = new Regex("(\\n[ \\t]*)?\\@\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        static ClassModel()
        {
            VoidClass = new ClassModel();
            VoidClass.Name = "void";
            VoidClass.InFile = new FileModel("");
        }

        public string Constructor;
        public MemberList Members;

        /// <summary>
        /// 1st extends type
        /// </summary>
        public string ExtendsType;

        /// <summary>
        /// Extensible types starting from the second
        /// </summary>
        public List<string> ExtendsTypes;

        public string IndexType;
        public List<string> Implements;
        [NonSerialized]
        private WeakReference resolvedExtend;

        public string QualifiedName
        {
            get
            {
                if (InFile.Package == "") return Name;
                if (InFile.Module == "" || InFile.Module == Name || (Name.Contains("<") && InFile.Module == BaseType)) return InFile.Package + "." + Name;
                return InFile.Package + "." + InFile.Module + "." + Name;
            }
        }

        /// <summary>
        /// The class name without its generic part (if present)
        /// </summary>
        public string BaseType
        {
            get
            {
                var genericIndex = Name.IndexOf('<');
                if (genericIndex > 0) return Name.Substring(0, genericIndex);
                return Name;
            }
        }

        public override string FullName
        {
            get
            {
                if (Template is null || Name.IndexOf('<') > 0) return Name;
                if (IndexType != null)
                {
                    if (InFile != null && InFile.haXe) return Name + IndexType;
                    return Name + "." + IndexType;
                }
                if (InFile != null && InFile.haXe) return Name + Template;
                return Name + "." + Template;
            }
        }

        /// <summary>
        /// Resolved extended type. Update using ResolveExtends()
        /// </summary>
        /// <returns>An extends ClassModel or empty ClassModel if the class do not have extends</returns>
        public ClassModel Extends
        {
            get 
            {
                if (resolvedExtend is null || !resolvedExtend.IsAlive)
                {
                    resolvedExtend = null;
                    return VoidClass;
                }
                return resolvedExtend.Target as ClassModel ?? VoidClass;
            }
        }

        /// <summary>
        /// Resolve inheritance chain starting with this class
        /// </summary>
        public void ResolveExtends()
        {
            var aClass = this;
            var extensionList = new List<ClassModel> {this};
            while (!aClass.IsVoid())
            {
                aClass = aClass.ResolveExtendedType(extensionList);
            }
        }

        private ClassModel ResolveExtendedType(IList<ClassModel> extensionList)
        {
            if (InFile.Context is null)
            {
                resolvedExtend = null;
                return VoidClass;
            }
            var objectKey = InFile.Context.Features.objectKey;
            if (Name == objectKey && !string.IsNullOrEmpty(InFile.Package))
            {
                var info = string.Format(TextHelper.GetString("ASCompletion.Info.InheritanceLoop"), objectKey, objectKey);
                MessageBar.ShowWarning(info);
                resolvedExtend = null;
                return VoidClass;
            }
            if (string.IsNullOrEmpty(ExtendsType))
            {
                if (IsVoid() || (Flags & FlagType.Interface) > 0)
                {
                    resolvedExtend = null;
                    return VoidClass;
                }
                ExtendsType = InFile.Context.DefaultInheritance(InFile.Package, Name);
                if (ExtendsType == QualifiedName)
                {
                    ExtendsType = InFile.Context.Features.voidKey;
                    resolvedExtend = null;
                    return VoidClass;
                }
            }
            var extends = InFile.Context.ResolveType(ExtendsType, InFile);
            if (!extends.IsVoid())
            {
                // check loops in inheritance
                if (extends.Name != objectKey)
                {
                    foreach(ClassModel model in extensionList)
                    {
                        if (model.QualifiedName != extends.QualifiedName) continue;
                        var info = string.Format(TextHelper.GetString("ASCompletion.Info.InheritanceLoop"), Type, extensionList[0].Type);
                        MessageBar.ShowWarning(info);
                        resolvedExtend = null;
                        return VoidClass;
                    }
                }
                extensionList.Add(extends);
                extends.InFile.Check();
            }
            resolvedExtend = new WeakReference(extends);
            return extends;
        }

        public ClassModel()
        {
            Name = null;
            Members = new MemberList();
        }

        public bool IsVoid() => this == VoidClass;

        [Obsolete("Please use (Flags & FlagType.Enum) != 0 or Flags.HasFlag(FlagType.Enum")]
        public bool IsEnum() => (Flags & FlagType.Enum) != 0;

        public new object Clone()
        {
            var result = new ClassModel();
            result.Name = Name;
            result.Template = Template;
            result.Flags = Flags;
            result.Access = Access;
            result.Namespace = Namespace;
            if (Parameters != null)
            {
                result.Parameters = new List<MemberModel>();
                foreach (var param in Parameters)
                    result.Parameters.Add((MemberModel) param.Clone());
            }
            result.Type = Type;
            result.Comments = Comments;
            result.InFile = InFile;
            result.Constructor = Constructor;
            if (Implements != null) result.Implements = new List<string>(Implements);
            result.ExtendsType = ExtendsType;
            result.IndexType = IndexType;
            result.Members = new MemberList();
            foreach (MemberModel item in Members)
                result.Members.Add((MemberModel) item.Clone());
            result.LineFrom = LineFrom;
            result.LineTo = LineTo;
            if (MetaDatas != null)
            {
                result.MetaDatas = new List<ASMetaData>();
                foreach (var meta in MetaDatas)
                {
                    result.MetaDatas.Add(new ASMetaData(meta.Name)
                    {
                        LineFrom = meta.LineFrom,
                        LineTo = meta.LineTo,
                        Params = meta.Params != null ? new Dictionary<string, string>(meta.Params) : null,
                        RawParams =  meta.RawParams,
                        Comments = meta.Comments,
                        Kind = meta.Kind,
                    });
                }
            }
            return result;
        }

        #region Completion-dedicated methods

        public MemberModel ToMemberModel()
        {
            var result = (ClassModel) Clone();
            result.Type = QualifiedName;
            result.IndexType = string.Empty;
            return result;
        }

        public MemberList GetSortedMembersList()
        {
            var items = new MemberList();
            foreach (MemberModel item in Members)
                if ((item.Flags & FlagType.Constructor) == 0) items.Add(item);
            items.Sort();
            return items;
        }

        /// <summary>
        /// Returns all members inherited from super classes of this class.
        /// Does not take static inheritance into account.
        /// </summary>
        internal MemberList GetSortedInheritedMembersList()
        {
            var items = new MemberList();
            var curClass = this;
            curClass.ResolveExtends();
            do
            {
                curClass = curClass.Extends;
                var newMembers = curClass.GetSortedMembersList();
                items.Merge(newMembers);
                
            } while (!curClass.Extends.IsVoid());
            items.RemoveAllWithFlag(FlagType.Static);
            items.Sort();
            return items;
        }

        #endregion

        #region Sorting

        public void Sort() => Members.Sort();

        public override bool Equals(object obj) => obj is ClassModel model && Name.Equals(model.Name);

        public override int GetHashCode() => Name.GetHashCode();

        #endregion

        #region Text output

        public override string ToString() => ClassDeclaration(this);

        public string GenerateIntrinsic(bool caching)
        {
            StringBuilder sb = new StringBuilder();
            string nl = (caching) ? "" : "\r\n";
            char semi = ';';
            string tab0 = (!caching && InFile.Version == 3) ? "\t" : "";
            string tab = (caching) ? "" : ((InFile.Version == 3) ? "\t\t" : "\t");
            bool preventVis = (Flags & FlagType.Interface) > 0;

            // SPECIAL DELEGATE
            /*if ((Flags & FlagType.Delegate) > 0)
            {
                if (Members.Count > 0)
                {
                    MemberModel ctor = Members[0].Clone() as MemberModel;
                    ctor.Flags |= FlagType.Delegate;
                    ctor.Access = Access;
                    String comment = CommentDeclaration(ctor.Comments, tab0);
                    sb.Append(comment);
                    sb.Append(tab0).Append(MemberDeclaration(ctor, preventVis)).Append(semi).Append(nl);
                    return sb.ToString();
                }
            }*/
            
            // META
            ASMetaData.GenerateIntrinsic(MetaDatas, sb, nl, tab0);
            
            // CLASS
            sb.Append(CommentDeclaration(Comments, tab0)).Append(tab0);
            if (!caching && InFile.Version != 3 && (Flags & (FlagType.Intrinsic | FlagType.Interface)) == 0)
            {
                sb.Append((InFile.haXe) ? "extern " : "intrinsic ");
            }
            sb.Append(ClassDeclaration(this, InFile.Version < 3));

            if (ExtendsType != null)
            {
                if ((Flags & FlagType.Abstract) > 0) sb.Append(" from ").Append(ExtendsType);
                else sb.Append(" extends ").Append(ExtendsType);
            }
            if (Implements != null)
            {
                sb.Append(" implements ");
                bool addSep = false;
                foreach (string iname in Implements)
                {
                    if (addSep) sb.Append(", ");
                    else addSep = true;
                    sb.Append(iname);
                }
            }
            sb.Append(nl).Append(tab0).Append('{');

            // MEMBERS
            int count = 0;
            foreach (MemberModel var in Members) 
                if ((var.Flags & FlagType.Variable) > 0)
                {
                    ASMetaData.GenerateIntrinsic(var.MetaDatas, sb, nl, tab);
                    var comment = CommentDeclaration(var.Comments, tab);
                    if (count == 0 || comment != "") sb.Append(nl);
                    sb.Append(comment);
                    sb.Append(tab).Append(MemberDeclaration(var, preventVis)).Append(semi).Append(nl);
                    count++;
                }

            // MEMBERS
            string prevProperty = null;
            foreach (MemberModel property in Members)
                if ((property.Flags & (FlagType.Getter | FlagType.Setter)) > 0)
                {
                    if (prevProperty != property.Name) sb.Append(nl);
                    prevProperty = property.Name;
                    ASMetaData.GenerateIntrinsic(property.MetaDatas, sb, nl, tab);
                    sb.Append(CommentDeclaration(property.Comments, tab));
                    var flags = (property.Flags & ~(FlagType.Setter | FlagType.Getter)) | FlagType.Function;

                    MemberModel temp;
                    if ((property.Flags & FlagType.Getter) > 0)
                    {
                        temp = (MemberModel)property.Clone();
                        temp.Name = "get " + temp.Name;
                        temp.Flags = flags;
                        temp.Parameters = null;

                        string memberDecl = MemberDeclaration(temp, preventVis);

                        // Typed callback declaration (in get property)
                        if ((property.Flags & FlagType.Function) > 0)
                        {
                            var commentDecl = property.ToDeclarationString();
                            var idxA = Math.Max(memberDecl.LastIndexOf(':'), memberDecl.LastIndexOf(')') + 1);
                            var idxB = Math.Min(commentDecl.IndexOf(':'), commentDecl.IndexOfOrdinal("/*"));

                            if (idxA > 0 && idxB > -1)
                                memberDecl = memberDecl.Substring(0, idxA) + commentDecl.Substring(idxB);
                        }
                        sb.Append(tab).Append(memberDecl).Append(semi).Append(nl);
                    }
                    if ((property.Flags & FlagType.Setter) > 0)
                    {
                        temp = (MemberModel)property.Clone();
                        temp.Name = "set " + temp.Name;
                        temp.Flags = flags;
                        temp.Type = (InFile.Version == 3) ? "void" : "Void";
                        sb.Append(tab).Append(MemberDeclaration(temp, preventVis)).Append(semi).Append(nl);
                    }
                }

            // MEMBERS
            foreach (MemberModel method in Members)
                if ((method.Flags & FlagType.Function) > 0 && (method.Flags & FlagType.Variable) == 0 && (method.Flags & FlagType.Getter) == 0)
                {
                    var decl = MemberDeclaration(method, preventVis);
                    if (InFile.haXe && (method.Flags & FlagType.Constructor) > 0)
                        decl = decl.Replace("function " + method.Name, "function new");
                    ASMetaData.GenerateIntrinsic(method.MetaDatas, sb, nl, tab);
                    sb.Append(nl).Append(CommentDeclaration(method.Comments, tab));
                    sb.Append(tab).Append(decl).Append(semi).Append(nl);
                }

            // END CLASS
            sb.Append(tab0).Append('}');
            return sb.ToString();
        }

        public static string ClassDeclaration(ClassModel ofClass) => ClassDeclaration(ofClass, true);

        public static string ClassDeclaration(ClassModel ofClass, bool qualified)
        {
            // package
            if (ofClass.Flags == FlagType.Package) return "package " + ofClass.Name.Replace('\\', '.');

            // modifiers
            var access = ofClass.Access;
            var modifiers = "";
            if ((ofClass.Flags & FlagType.Intrinsic) > 0)
            {
                if ((ofClass.Flags & FlagType.Extern) > 0) modifiers += "extern ";
                else modifiers += "intrinsic ";
            }
            else if (ofClass.InFile.Version > 2)
                if (!string.IsNullOrEmpty(ofClass.Namespace) && ofClass.Namespace != "internal") 
                {
                    //if ((ft & FlagType.Interface) == 0)
                    modifiers += ofClass.Namespace + " ";
                }
                else
                {
                    //  if ((ft & FlagType.Interface) == 0)
                    //  {
                    if ((access & Visibility.Public) > 0) modifiers += "public ";
                    else if ((access & Visibility.Internal) > 0) modifiers += "internal ";
                    else if ((access & Visibility.Protected) > 0) modifiers += "protected ";
                    else if ((access & Visibility.Private) > 0) modifiers += "private ";
                    //  }
                }

            if ((ofClass.Flags & FlagType.Final) > 0) modifiers += "final ";
            if ((ofClass.Flags & FlagType.Dynamic) > 0) modifiers += "dynamic ";

            var classType = "class";
            if ((ofClass.Flags & FlagType.Interface) > 0) classType = "interface";
            else if ((ofClass.Flags & FlagType.Enum) > 0) classType = "enum";
            else if ((ofClass.Flags & FlagType.Abstract) > 0) classType = "abstract";
            else if ((ofClass.Flags & FlagType.TypeDef) > 0) classType = "typedef";
            else if ((ofClass.Flags & FlagType.Struct) > 0) classType = "struct";
            else if ((ofClass.Flags & FlagType.Delegate) > 0) classType = "delegate";

            // signature
            if (qualified) return $"{modifiers}{classType} {ofClass.QualifiedName}";
            return $"{modifiers}{classType} {ofClass.FullName}";
        }

        public static string MemberDeclaration(MemberModel member) => MemberDeclaration(member, false);

        public static string MemberDeclaration(MemberModel member, bool preventVisibility)
        {
            // modifiers
            var flags = member.Flags;
            var access = member.Access;
            var modifiers = "";
            if ((flags & FlagType.Intrinsic) > 0)
            {
                if ((flags & FlagType.Extern) > 0) modifiers += "extern ";
                else modifiers += "intrinsic ";
            }
            else if (!string.IsNullOrEmpty(member.Namespace) && member.Namespace != "internal")
            {
                if ((flags & FlagType.Interface) == 0) modifiers = member.Namespace + " ";
            }
            else if (!preventVisibility)
            {
                if ((member.Flags & FlagType.Interface) == 0)
                {
                    if ((access & Visibility.Public) > 0) modifiers += "public ";
                    //  else if ((acc & Visibility.Internal) > 0) modifiers += "internal "; // AS3 default
                    else if ((access & Visibility.Protected) > 0) modifiers += "protected ";
                    else if ((access & Visibility.Private) > 0) modifiers += "private ";
                }
            }

            if ((flags & FlagType.Final) > 0) modifiers += "final ";
            if ((flags & FlagType.Enum) > 0) return member.ToString();
            if ((flags & FlagType.Class) > 0)
            {
                if ((flags & FlagType.Dynamic) > 0) modifiers += "dynamic ";
                string classType = "class";
                if ((member.Flags & FlagType.Interface) > 0) classType = "interface";
                else if ((member.Flags & FlagType.Enum) > 0) classType = "enum";
                else if ((member.Flags & FlagType.Abstract) > 0) classType = "abstract";
                else if ((member.Flags & FlagType.TypeDef) > 0) classType = "typedef";
                else if ((member.Flags & FlagType.Struct) > 0) classType = "struct";
                else if ((member.Flags & FlagType.Delegate) > 0) classType = "delegate";
                return $"{modifiers}{classType} {member.Type}";
            }
            if ((flags & FlagType.Enum) == 0)
            {
                if ((flags & FlagType.Native) > 0) modifiers += "native ";
                if ((flags & FlagType.Static) > 0) modifiers += "static ";
            }

            // signature
            if ((flags & FlagType.Namespace) > 0) return $"{modifiers}namespace {member.Name}";
            if ((flags & FlagType.Variable) > 0)
            {
                if ((flags & FlagType.LocalVar) > 0) modifiers = "local ";
                if ((flags & FlagType.Constant) > 0)
                {
                    if (member.Value is null) return $"{modifiers}const {member.ToDeclarationString()}";
                    return $"{modifiers}const {member.ToDeclarationString()} = {member.Value}";
                }
                return $"{modifiers}var {member.ToDeclarationString()}";
            }
            if ((flags & (FlagType.Getter | FlagType.Setter)) > 0) return $"{modifiers}property {member}";
            if ((flags & FlagType.Delegate) > 0) return $"{modifiers}delegate {member}";
            if ((flags & FlagType.Function) > 0) return $"{modifiers}function {member}";
            if (flags == FlagType.Package) return $"Package {member.Type}";
            if (flags == FlagType.Template) return $"Template {member.Type}";
            if (flags == FlagType.Declaration) return $"Declaration {member.Type}";
            return $"{modifiers}type {member.Type}";
        }

        public static string CommentDeclaration(string comment, string tab)
        {
            if (comment is null) return "";
            comment = comment.Trim();
            if (comment.Length == 0) return "";
            var startWithStar = comment.StartsWith('*');
            if (startWithStar || comment.IndexOf('\n') > 0 || comment.IndexOf('\r') > 0)
            {
                if (!startWithStar) comment = "* " + comment;
                comment = reEOLAndStar.Replace(comment, "\n");
                comment = comment.Replace("\r\n", "\n");
                comment = comment.Replace("\r", "\n");
                comment = reSpacesAfterEOL.Replace(comment, "\n");
                comment = reMultiSpacedEOL.Replace(comment, "\n\n  ");
                comment = reAsdocWordSpace.Replace(comment, "\n");
                comment = GetCorrectComment(comment, "\n", "\n  ");
                if (tab != "")
                {
                    var space = PluginBase.Settings.CommentBlockStyle == CommentBlockStyle.Indented ? " " : "";
                    comment = comment.Replace("\n", "\r\n" + tab + space + "* ");
                    return tab + "/**\r\n" + tab + space + comment + "\r\n" + tab + space + "*/\r\n";
                }
                return tab + "/**\r\n" + tab + comment + "\r\n" + tab + "*/\r\n";
            }

            return tab + "/// " + comment + "\r\n";
        }

        static string GetCorrectComment(string comment, string eolSrc, string eolRepl)
        {
            MatchCollection mc = reAsdocWord.Matches(comment);

            string outComment = "";

            int j0 = 0;
            int i, l = mc.Count;
            for (i = 0; i <= l; i++)
            {
                var j1 = i < l ? mc[i].Index : comment.Length;

                var s = comment.Substring(j0, j1 - j0);

                if (i > 0)
                    s = s.Replace(eolSrc, eolRepl);

                outComment += s;

                if (i < l)
                {
                    if (i == 0 && MoreLines(comment, 5))
                        outComment += "\n";

                    outComment += mc[i].Value;
                    j0 = mc[i].Index + mc[i].Length;
                }
            }

            return outComment;
        }

        private static bool MoreLines(string text, int count)
        {
            int p = text.IndexOf('\n');
            while (p > 0 && count >= 0)
            {
                count--;
                p = text.IndexOf('\n', p);
            }
            return count >= 0;
        }
        #endregion
    }
}