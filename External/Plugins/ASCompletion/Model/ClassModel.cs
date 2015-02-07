using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using ASCompletion.Context;

namespace ASCompletion.Model
{
    /// <summary>
    /// Object representation of an Actionscript class
    /// </summary>
    [Serializable]
    public class ClassModel: MemberModel
    {
        static public ClassModel VoidClass;
        static private List<ClassModel> extensionList;

		static private Regex reSpacesAfterEOL = new Regex("(?<!(\n[ \t]*))(\n[ \t]+)(?!\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		static private Regex reEOLAndStar = new Regex(@"[\r\n]+\s*\*", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		static private Regex reMultiSpacedEOL = new Regex("([ \t]*\n[ \t]*){2,}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		static private Regex reAsdocWordSpace = new Regex("\\s+(?=\\@\\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		static private Regex reAsdocWord = new Regex("(\\n[ \\t]*)?\\@\\w+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
        static ClassModel()
        {
            VoidClass = new ClassModel();
            VoidClass.Name = "void";
            VoidClass.InFile = new FileModel("");
        }

        static private void EndResolveExtend()
        {
            extensionList = null;
        }

        static private void BeginResolveExtend(ClassModel firstClass)
        {
            extensionList = new List<ClassModel>();
            if (firstClass != null) extensionList.Add(firstClass);
        }

        public string Constructor;
        public MemberList Members;

        public string ExtendsType;
        public string IndexType;
        public List<string> Implements;
        [NonSerialized]
        private WeakReference resolvedExtend;

        public string QualifiedName
        {
            get
            {
                if (InFile.Package == "") return Name;
                if (InFile.Module == "" || InFile.Module == Name) return InFile.Package + "." + Name;
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
                int genericIndex = Name.IndexOf('<');
                if (genericIndex > 0)
                    return Name.Substring(0, genericIndex);
                else return Name;
            }
        }

        override public string FullName
        {
            get
            {
                if (Template == null || Name.IndexOf('<') > 0) return Name;
                if (IndexType != null)
                {
                    if (InFile != null && InFile.haXe) return Name + IndexType;
                    return Name + "." + IndexType;
                }
                else if (InFile != null && InFile.haXe) return Name + Template;
                else return Name + "." + Template;
            }
        }

        /// <summary>
        /// Resolved extended type. Update using ResolveExtends()
        /// </summary>
        public ClassModel Extends
        {
            get 
            {
                if (resolvedExtend == null || !resolvedExtend.IsAlive)
                {
                    resolvedExtend = null;
                    return ClassModel.VoidClass;
                }
                else return resolvedExtend.Target as ClassModel ?? ClassModel.VoidClass;
            }
        }

        /// <summary>
        /// Resolve inheritance chain starting with this class
        /// </summary>
        public void ResolveExtends()
        {
            ClassModel aClass = this;
            BeginResolveExtend(aClass);
            try
            {
                while (!aClass.IsVoid())
                {
                    aClass = aClass.ResolveExtendedType();
                }
            }
            finally { EndResolveExtend(); }
        }

        private ClassModel ResolveExtendedType()
        {
            if (InFile.Context == null)
            {
                resolvedExtend = null;
                return VoidClass;
            }
            string objectKey = InFile.Context.Features.objectKey;
            if (Name == objectKey && !string.IsNullOrEmpty(InFile.Package))
            {
                string info = string.Format(TextHelper.GetString("ASCompletion.Info.InheritanceLoop"), objectKey, objectKey);
                PluginCore.Controls.MessageBar.ShowWarning(info);
                resolvedExtend = null;
                return VoidClass;
            }
            if (string.IsNullOrEmpty(ExtendsType))
            {
                if (this == VoidClass || (Flags & FlagType.Interface) > 0)
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
            ClassModel extends = InFile.Context.ResolveType(ExtendsType, InFile);
            if (!extends.IsVoid())
            {
                // check loops in inheritance
                if (extensionList != null)
                {
                    if (extends.Name != objectKey)
                    {
                        foreach(ClassModel model in extensionList)
                        {
                            if (model.QualifiedName == extends.QualifiedName)
                            {
                                string info = String.Format(TextHelper.GetString("ASCompletion.Info.InheritanceLoop"), Type, extensionList[0].Type);
                                PluginCore.Controls.MessageBar.ShowWarning(info);
                                resolvedExtend = null;
                                return VoidClass;
                            }
                        }
                    }
                    extensionList.Add(extends);
                }
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

        public bool IsVoid()
        {
            return this == VoidClass;
        }

        public bool IsEnum() {
            return (this.Flags & FlagType.Enum) != 0;
        }

        public new object Clone()
        {
            ClassModel copy = new ClassModel();
            copy.Name = Name;
            copy.Template = Template;
            copy.Flags = Flags;
            copy.Access = Access;
            copy.Namespace = Namespace;
            if (Parameters != null)
            {
                copy.Parameters = new List<MemberModel>();
                foreach (MemberModel param in Parameters)
                    copy.Parameters.Add(param.Clone() as MemberModel);
            }
            copy.Type = Type;
            copy.Comments = Comments;
            copy.InFile = InFile;
            copy.Constructor = Constructor;
            if (Implements != null)
            {
                copy.Implements = new List<string>();
                foreach (string cname in Implements) copy.Implements.Add(cname);
            }
            copy.ExtendsType = ExtendsType;
            copy.IndexType = IndexType;
            copy.Members = new MemberList();
            foreach (MemberModel item in Members)
                copy.Members.Add(item.Clone() as MemberModel);
            copy.LineFrom = LineFrom;
            copy.LineTo = LineTo;

            return copy;
        }

        #region Completion-dedicated methods

        public MemberModel ToMemberModel()
        {
            MemberModel self = new MemberModel();
            //int p = Name.LastIndexOf(".");
            //self.Name = (p >= 0) ? Name.Substring(p + 1) : Name;
            self.Name = Name;
            self.Type = QualifiedName;
            self.Flags = Flags;
            return self;
        }

        internal MemberList GetSortedMembersList()
        {
            MemberList items = new MemberList();
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
            MemberList items = new MemberList();
            ClassModel curClass = this;
            do
            {
                curClass.ResolveExtends();
                curClass = curClass.Extends;
                MemberList newMembers = curClass.GetSortedMembersList();
                items.Merge(newMembers);
                
            } while (curClass.Extends != ClassModel.VoidClass);
            items.RemoveAllWithFlag(FlagType.Static);
            items.Sort();
            return items;
        }

        #endregion

        #region Sorting

        public void Sort()
        {
            Members.Sort();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClassModel)) return false;
            return Name.Equals(((ClassModel)obj).Name);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Text output

        public override string ToString()
        {
            return ClassDeclaration(this);
        }

        public string GenerateIntrinsic(bool caching)
        {
            StringBuilder sb = new StringBuilder();
            string nl = (caching) ? "" : "\r\n";
            char semi = ';';
            string tab0 = (!caching && InFile.Version == 3) ? "\t" : "";
            string tab = (caching) ? "" : ((InFile.Version == 3) ? "\t\t" : "\t");
			bool preventVis = (bool)((this.Flags & FlagType.Interface) > 0);

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

            // CLASS
            sb.Append(CommentDeclaration(Comments, tab0)).Append(tab0);
            if (!caching && InFile.Version != 3 && (this.Flags & (FlagType.Intrinsic | FlagType.Interface)) == 0)
            {
                sb.Append((InFile.haXe) ? "extern " : "intrinsic ");
            }
            sb.Append(ClassDeclaration(this, InFile.Version < 3));

            if (ExtendsType != null)
            {
                if ((this.Flags & FlagType.Abstract) > 0) sb.Append(" from ").Append(ExtendsType);
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
                    String comment = CommentDeclaration(var.Comments, tab);
                    if (count == 0 || comment != "") sb.Append(nl);
                    sb.Append(comment);
                    sb.Append(tab).Append(MemberDeclaration(var, preventVis)).Append(semi).Append(nl);
                    count++;
                }

            // MEMBERS
            string decl;
            MemberModel temp;
            string prevProperty = null;
            foreach (MemberModel property in Members)
                if ((property.Flags & (FlagType.Getter | FlagType.Setter)) > 0)
                {
                    if (prevProperty != property.Name) sb.Append(nl);
                    prevProperty = property.Name;
                    ASMetaData.GenerateIntrinsic(property.MetaDatas, sb, nl, tab);
                    sb.Append(CommentDeclaration(property.Comments, tab));
                    FlagType flags = (property.Flags & ~(FlagType.Setter | FlagType.Getter)) | FlagType.Function;

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
							string commentDecl = property.ToDeclarationString();
							int idxA = System.Math.Max(memberDecl.LastIndexOf(":"), memberDecl.LastIndexOf(")") + 1);
							int idxB = System.Math.Min(commentDecl.IndexOf(":"), commentDecl.IndexOf("/*"));

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
                    decl = MemberDeclaration(method, preventVis);
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

        static public string ClassDeclaration(ClassModel ofClass)
        {
            return ClassDeclaration(ofClass, true);
        }

        static public string ClassDeclaration(ClassModel ofClass, bool qualified)
        {
            // package
            if (ofClass.Flags == FlagType.Package)
            {
                return "package " + ofClass.Name.Replace('\\', '.');
            }
            else
            {
                // modifiers
                FlagType ft = ofClass.Flags;
                Visibility acc = ofClass.Access;
                string modifiers = "";
                if ((ofClass.Flags & FlagType.Intrinsic) > 0)
                {
                    if ((ofClass.Flags & FlagType.Extern) > 0) modifiers += "extern ";
                    else modifiers += "intrinsic ";
                }
                else if (ofClass.InFile.Version > 2)
                    if (ofClass.Namespace != null && ofClass.Namespace.Length > 0 
                        && ofClass.Namespace != "internal") 
                    {
					//	if ((ft & FlagType.Interface) == 0)
                            modifiers += ofClass.Namespace + " ";
                    }
                    else
                    {
					//	if ((ft & FlagType.Interface) == 0)
					//	{
							if ((acc & Visibility.Public) > 0) modifiers += "public ";
							else if ((acc & Visibility.Internal) > 0) modifiers += "internal ";
							else if ((acc & Visibility.Protected) > 0) modifiers += "protected ";
							else if ((acc & Visibility.Private) > 0) modifiers += "private ";
					//	}
                    }

				if ((ofClass.Flags & FlagType.Final) > 0)
					modifiers += "final ";

                if ((ofClass.Flags & FlagType.Dynamic) > 0)
                    modifiers += "dynamic ";

                string classType = "class";
                if ((ofClass.Flags & FlagType.Interface) > 0) classType = "interface";
                else if ((ofClass.Flags & FlagType.Enum) > 0) classType = "enum";
                else if ((ofClass.Flags & FlagType.Abstract) > 0) classType = "abstract";
                else if ((ofClass.Flags & FlagType.TypeDef) > 0) classType = "typedef";
                else if ((ofClass.Flags & FlagType.Struct) > 0) classType = "struct";
                else if ((ofClass.Flags & FlagType.Delegate) > 0) classType = "delegate";

                // signature
                if (qualified)
                    return String.Format("{0}{1} {2}", modifiers, classType, ofClass.QualifiedName);
                else
                    return String.Format("{0}{1} {2}", modifiers, classType, ofClass.FullName);
            }
        }

		static public string MemberDeclaration(MemberModel member)
		{
			return MemberDeclaration(member, false);
		}
        static public string MemberDeclaration(MemberModel member, bool preventVisibility)
        {
            // modifiers
            FlagType ft = member.Flags;
            Visibility acc = member.Access;
            string modifiers = "";
            if ((ft & FlagType.Intrinsic) > 0)
            {
                if ((ft & FlagType.Extern) > 0) modifiers += "extern ";
                else modifiers += "intrinsic ";
            }
            else if (member.Namespace != null && member.Namespace.Length > 0 
                && member.Namespace != "internal")
            {
				if ((ft & FlagType.Interface) == 0)
					modifiers = member.Namespace + " ";
            }
            else if (!preventVisibility)
            {
				if ((member.Flags & FlagType.Interface) == 0)
				{
					if ((acc & Visibility.Public) > 0) modifiers += "public ";
				//	else if ((acc & Visibility.Internal) > 0) modifiers += "internal "; // AS3 default
					else if ((acc & Visibility.Protected) > 0) modifiers += "protected ";
					else if ((acc & Visibility.Private) > 0) modifiers += "private ";
				}
            }

			if ((ft & FlagType.Final) > 0)
				modifiers += "final ";

			if ((ft & FlagType.Enum) > 0)
			{
				return member.ToString();
			}
			else if ((ft & FlagType.Class) > 0)
			{
				if ((ft & FlagType.Dynamic) > 0)
					modifiers += "dynamic ";
				string classType = "class";
				if ((member.Flags & FlagType.Interface) > 0) classType = "interface";
                else if ((member.Flags & FlagType.Enum) > 0) classType = "enum";
                else if ((member.Flags & FlagType.Abstract) > 0) classType = "abstract";
                else if ((member.Flags & FlagType.TypeDef) > 0) classType = "typedef";
                else if ((member.Flags & FlagType.Struct) > 0) classType = "struct";
                else if ((member.Flags & FlagType.Delegate) > 0) classType = "delegate";
				return String.Format("{0}{1} {2}", modifiers, classType, member.Type);
			}
			else if ((ft & FlagType.Enum) == 0)
			{
				if ((ft & FlagType.Native) > 0)
					modifiers += "native ";
				if ((ft & FlagType.Static) > 0)
					modifiers += "static ";
			}

            // signature
            if ((ft & FlagType.Namespace) > 0)
            {
                return String.Format("{0}namespace {1}", modifiers, member.Name);
            }
            else if ((ft & FlagType.Variable) > 0)
            {
                if ((ft & FlagType.LocalVar) > 0) modifiers = "local ";
                if ((ft & FlagType.Constant) > 0)
                {
                    if (member.Value == null)
						return String.Format("{0}const {1}", modifiers, member.ToDeclarationString());
                    else
						return String.Format("{0}const {1} = {2}", modifiers, member.ToDeclarationString(), member.Value);
                }
				else return String.Format("{0}var {1}", modifiers, member.ToDeclarationString());
            }
			else if ((ft & (FlagType.Getter | FlagType.Setter)) > 0)
				return String.Format("{0}property {1}", modifiers, member.ToString());
            else if ((ft & FlagType.Delegate) > 0)
                return String.Format("{0}delegate {1}", modifiers, member.ToString());
            else if ((ft & FlagType.Function) > 0)
				return String.Format("{0}function {1}", modifiers, member.ToString());
            else if (ft == FlagType.Package)
                return String.Format("Package {0}", member.Type);
            else if (ft == FlagType.Template)
                return String.Format("Template {0}", member.Type);
            else if (ft == FlagType.Declaration)
                return String.Format("Declaration {0}", member.Type);
            else
                return String.Format("{0}type {1}", modifiers, member.Type);
        }

        static public string CommentDeclaration(string comment, string tab)
        {
            if (comment == null) return "";
            comment = comment.Trim();
            if (comment.Length == 0) return "";
            Boolean indent = tab != "";
            String space = PluginCore.PluginBase.Settings.CommentBlockStyle == PluginCore.CommentBlockStyle.Indented ? " " : "";
			Boolean startWithStar = comment.StartsWith("*");
			if (startWithStar || comment.IndexOf('\n') > 0 || comment.IndexOf('\r') > 0)
            {
				if (!startWithStar)
					comment = "* " + comment;

				comment = reEOLAndStar.Replace(comment, "\n");
				comment = comment.Replace("\r\n", "\n");
				comment = comment.Replace("\r", "\n");
				comment = reSpacesAfterEOL.Replace(comment, "\n");
				comment = reMultiSpacedEOL.Replace(comment, "\n\n  ");
				comment = reAsdocWordSpace.Replace(comment, "\n");
				comment = GetCorrectComment(comment, "\n", "\n  ");
                if (indent)
                {
                    comment = comment.Replace("\n", "\r\n" + tab + space + "* ");
                    return tab + "/**\r\n" + tab + space + comment + "\r\n" + tab + space + "*/\r\n";
                }
                else return tab + "/**\r\n" + tab + comment + "\r\n" + tab + "*/\r\n";
            }
            else return tab + "/// " + comment + "\r\n";
        }

		static public string GetCorrectComment(string comment, string eolSrc, string eolRepl)
		{
			MatchCollection mc = reAsdocWord.Matches(comment);

			string outComment = "";
			string s;

			int j0 = 0;
			int j1 = 0;
			int i, l = mc.Count;
			for (i = 0; i <= l; i++)
			{
				if (i < l)
					j1 = mc[i].Index;
				else
					j1 = comment.Length;

				s = comment.Substring(j0, j1 - j0);

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