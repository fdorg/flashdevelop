/*
 * Misc classes used by the plugin
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace ASCompletion
{
    /// <summary>
    /// Object representation of an Actionscript class
    /// </summary>
    sealed public class ASClass
    {
        #region actionscript class instance
        public FlagType Flags;
        private string fileName;
        public string ClassName;
        public ASMemberList Imports;
        public ASMemberList Methods;
        public ASMemberList Vars;
        public ASMemberList Properties;
        public ASMemberList Package;
        public bool OutOfDate;
        public bool IsAS3;
        public string BasePath;
        private ASClass extends;
        public string Implements;
        public string Comments;
        
        public ASClass Extends
        {
            get {
                if (extends == null) return null;
                else return ASClassParser.Context.GetCachedClass(extends);
            }
            set {
                extends = value;
            }
        }

        public string FileName
        {
            get {
                return fileName;
            }
            set {
                fileName = value;
            }
        }
        
        public ASClass() 
        {
            ClassName = null;
            Imports = new ASMemberList();
            Methods = new ASMemberList();
            Vars = new ASMemberList();
            Properties = new ASMemberList();
        }
        
        public bool IsVoid()
        {
            return (ClassName == null || ClassName.Length == 0 || ClassName.ToLower() == "void");
        }
        
        public ASMember ToASMember()
        {
            ASMember self = new ASMember();
            int p = ClassName.LastIndexOf(".");
            self.Name = (p >= 0) ? ClassName.Substring(p+1) : ClassName;
            self.Type = ClassName;
            self.Flags = Flags;
            return self;
        }
        
        public string GenerateIntrinsic()
        {
            StringBuilder sb = new StringBuilder();
            string nl = "\r\n";
            char semi = ';';
            char tab = '\t';
            
            // IMPORTS
            ArrayList known = new ArrayList();
            known.Add(ClassName);
            foreach(ASMember import in Imports)
            if (!known.Contains(import.Type))
            {
                known.Add(import.Type);
                sb.Append("import ").Append(import.Type).Append(semi).Append(nl);
            }
            
            // CLASS
            sb.Append(CommentDeclaration(Comments, false));
            if ((this.Flags & (FlagType.Intrinsic | FlagType.Interface)) == 0) sb.Append("intrinsic ");
            sb.Append(ClassDeclaration(this));
            if (!extends.IsVoid() && (extends.ClassName != "Object"))
                sb.Append(" extends ").Append(extends.ClassName);
            if (Implements != null)
                sb.Append(" implements ").Append(Implements);
            sb.Append(nl).Append('{').Append(nl);
            
            // MEMBERS
            int count = 0;
            foreach(ASMember var in Vars)
            //if ( (var.Flags & FlagType.Public) > 0 )
            {
                sb.Append(CommentDeclaration(var.Comments, true));
                sb.Append(tab).Append(MemberDeclaration(var)).Append(semi).Append(nl);
                count ++;
            }
            if (count > 0) sb.Append(nl);
            
            // MEMBERS
            string decl;
            ASMember temp;
            count = 0;
            foreach(ASMember property in Properties)
            //if ( (property.Flags & FlagType.Public) > 0 )
            {
                sb.Append(CommentDeclaration(property.Comments, true));
                FlagType flags = (property.Flags & ~(FlagType.Setter | FlagType.Getter)) | FlagType.Function;
                
                if ( (property.Flags & FlagType.Getter) > 0 )
                {
                    temp = (ASMember)property.Clone();
                    temp.Name = "get "+temp.Name;
                    temp.Flags = flags;
                    temp.Parameters = "";
                    sb.Append(tab).Append(MemberDeclaration(temp)).Append(semi).Append(nl);
                }
                if ( (property.Flags & FlagType.Setter) > 0 )
                {
                    temp = (ASMember)property.Clone();
                    temp.Name = "set "+temp.Name;
                    temp.Flags = flags;
                    temp.Type = (IsAS3) ? "void" : "Void";
                    sb.Append(tab).Append(MemberDeclaration(temp)).Append(semi).Append(nl);
                }
                sb.Append(nl);
                count ++;
            }
            if (count > 0) sb.Append(nl);
            
            // MEMBERS
            count = 0;
            foreach(ASMember method in Methods)
            //if ( (method.Flags & FlagType.Public) > 0 )
            {
                decl = MemberDeclaration(method);
                if ( (method.Flags & FlagType.Constructor) > 0 ) decl = decl.Replace(" : constructor", "");
                sb.Append(CommentDeclaration(method.Comments, true));
                sb.Append(tab).Append(decl).Append(semi).Append(nl).Append(nl);
                count ++;
            }
                        
            // END CLASS
            sb.Append('}');
            return sb.ToString();
        }
        
        public void Sort()
        {
            Imports.Sort();
            Methods.Sort();
            Vars.Sort();
            Properties.Sort();
        }
        
        public override string ToString()
        {
            //string res = "";
            return ClassName;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ASClass)) return false;
            return FileName.Equals( ((ASClass)obj).FileName );
        }
        public override int GetHashCode()
        {
            return ClassName.GetHashCode();
        }
        #endregion
        
        #region actionscript declaration generation
        static public string ClassDeclaration(ASClass oClass)
        {
            // package
            if (oClass.Flags == FlagType.Package)
            {
                return "package "+oClass.ClassName.Replace('\\', '.');
            }
            else
            {
                // modifiers
                string modifiers = "";
                if ((oClass.Flags & FlagType.Intrinsic) > 0)
                    modifiers += "intrinsic ";
                if ((oClass.Flags & FlagType.Dynamic) > 0)
                    modifiers += "dynamic ";
                
                string classType = ((oClass.Flags & FlagType.Interface) > 0) ? "interface" : "class";
                // signature
                return String.Format("{0}{1} {2}", modifiers, classType, oClass.ClassName);
            }
        }
        
        static public string MemberDeclaration(ASMember member)
        {
            // modifiers
            FlagType ft = member.Flags;
            string modifiers = "";
            if ((ft & FlagType.Class) > 0)
            {
                if ((ft & FlagType.Intrinsic) > 0)
                    modifiers += "intrinsic ";
                if ((ft & FlagType.Dynamic) > 0)
                    modifiers += "dynamic ";
                // TODO (or not?) ASClasses: parse classes in completion list to eval if there are interfaces or classes?
                string classType = ((member.Flags & FlagType.Interface) > 0) ? "interface" : "class";
                return String.Format("{0}{1} {2}", modifiers, classType, member.Type);
            }
            else 
            {
                if ((ft & FlagType.Static) > 0)
                    modifiers += "static ";
                if ((ft & FlagType.Private) > 0)
                    modifiers += "private ";
                else if ((ft & FlagType.Public) > 0)
                    modifiers += "public ";
            }
            // signature
            if ((ft & FlagType.Function) > 0)
                return String.Format("{0}function {1}", modifiers, member.ToString());
            else if ((ft & FlagType.Variable) > 0)
            {
                if (modifiers.Length == 0) modifiers = "local ";
                return String.Format("{0}var {1}", modifiers, member.ToString());
            }
            else if ((ft & (FlagType.Getter | FlagType.Setter)) > 0)
                return String.Format("{0}property {1}", modifiers, member.ToString());
            else if ((ft & FlagType.Template) > 0)
                return String.Format("Template {0}", member.Type);
            else if (ft == FlagType.Package)
                return String.Format("Package {0}", member.Type);
            else 
            {
                if ((ft & FlagType.Intrinsic) > 0) modifiers = "intrinsic "+modifiers;
                return String.Format("{0}type {1}", modifiers, member.Type);
            }
        }
        
        static public string CommentDeclaration(string comment, bool indent)
        {
            if (comment == null) return "";
            if (indent) return "\t/**"+comment+"*/\r\n";
            else return "/**"+comment+"*/\r\n";
        }
        #endregion
    }
    
    #region class_members
    /// <summary>
    /// Object representation of an Actionscript ASMember
    /// </summary>
    sealed public class ASMember: ICloneable, IComparable
    {
        public FlagType Flags;
        public string Name;
        public string Parameters;
        public string Type;
        public string Comments;
        
        public Object Clone()
        {
            ASMember copy = new ASMember();
            copy.Name = Name;
            copy.Flags = Flags;
            copy.Parameters = Parameters;
            copy.Type = Type;
            copy.Comments = Comments;
            return copy;
        }
        
        public override string ToString()
        {
            string res = Name;
            if ((Flags & FlagType.Function) > 0) 
                res += "("+Parameters+")";
            if ((Flags & FlagType.Constructor) > 0)
                return res+" : constructor";
            else if (Type.Length > 0)
                return res+" : "+Type;
            else
                return res;
        }
        
        public override bool Equals(object obj)
        {
            if (!(obj is ASMember)) 
                return false;
            return Name.Equals( ((ASMember)obj).Name );
        }
        
        public override int GetHashCode() 
        {
            return (Name+Flags).GetHashCode();
        }
        
        private string upperName;
        internal string UpperName
        {
            get {
                if (upperName == null) upperName = Name.ToUpper();
                return upperName;
            }
        }
        public int CompareTo(object obj)
        {
            // using ascii comparison to be compatible with Scintilla completion list
            if (!(obj is ASMember))
                throw new InvalidCastException("This object is not of type ASMember");
            return string.CompareOrdinal(UpperName, ((ASMember)obj).UpperName);
        }
    }
    
    /// <summary>
    /// Strong-typed ASMember list with special merging/searching methods
    /// </summary>
    sealed public class ASMemberList: IEnumerable
    {
        private ArrayList items;
        private bool Sorted;
        
        public IEnumerator GetEnumerator()
        {
            return items.GetEnumerator();
        }
        
        /*public ArrayList Items 
        {
            get {
                return items;
            }
        }*/

        public int Count
        {
            get {
                return items.Count;
            }
        }
        
        public ASMemberList()
        {
            items = new ArrayList();
        }
        
        public ASMember this[int index]
        {
            get {
                return (ASMember)items[index];
            }
            set {
                Sorted = false;
                items[index] = value;
            }
        }
        
        public int Add(ASMember value)
        {
            Sorted = false;
            return items.Add(value);
        }
        
        public void Insert(int index, ASMember value)
        {
            Sorted = false;
            items.Insert(index, value);
        }
        
        public void Remove(ASMember value)
        {
            Sorted = false;
            items.Remove(value);
        }
        
        public void Clear()
        {
            Sorted = true;
            items.Clear();
        }
        
        public ASMember Search(string name, FlagType mask) {
            foreach (ASMember m in items)
                if ((m.Name.Equals(name)) && ((m.Flags & mask) == mask)) return m;
            return null;
        }
        
        public ASMemberList MultipleSearch(string name, FlagType mask) {
            ASMemberList result = new ASMemberList();
            foreach (ASMember m in items)
                if ((m.Name.Equals(name)) && ((m.Flags & mask) == mask)) result.Add(m);
            return result;
        }
        
        public void Sort()
        {
            if (!Sorted) 
            {
                items.Sort();
                Sorted = true;
            }
        }
        
        /// <summary>
        /// Merge one item into the list
        /// </summary>
        /// <param name="item">Item to merge</param>
        public void Merge(ASMember item)
        {
            ASMemberList list = new ASMemberList();
            list.Add(item);
            Merge(list, 0);
        }
        
        /// <summary>
        /// Merge SORTED lists without duplicate values
        /// </summary>
        /// <param name="list">Items to merge</param>
        public void Merge(ASMemberList list)
        {
            Merge(list, 0);
        }
        
        /// <summary>
        /// Merge selected items from the SORTED lists without duplicate values
        /// </summary>
        /// <param name="list">Items to merge</param>
        public void Merge(ASMemberList list, FlagType mask)
        {
            int index = 0;
            bool added;
            foreach (ASMember m in list)
            if ((m.Flags & mask) == mask) 
            {
                added = false;
                while (index < items.Count)
                {
                    if (m.CompareTo(items[index]) <= 0)
                    {
                        if (!m.Equals(items[index]))
                            items.Insert(index, m);
                        added = true;
                        break;
                    }
                    index++;
                }
                if (!added) items.Add(m);
            }
        }
        
        /// <summary>
        /// Merge selected items from the SORTED lists without duplicate values
        /// 
        /// </summary>
        /// <param name="list">Items to merge</param>
        /// <param name="mask">Filter by mask</param>
        /// <param name="filterStatic">Ignore static members (inheritance)</param>
        public void Merge(ASMemberList list, FlagType mask, bool filterStatic)
        {
            int index = 0;
            bool added;
            foreach (ASMember m in list)
            if ((m.Flags & mask) == mask && (!filterStatic || (m.Flags & FlagType.Static) == 0))
            {
                added = false;
                while (index < items.Count)
                {
                    if (m.CompareTo(items[index]) <= 0)
                    {
                        if (!m.Equals(items[index]))
                            items.Insert(index, m);
                        added = true;
                        break;
                    }
                    index++;
                }
                if (!added) items.Add(m);
            }
        }
    }
    #endregion
    
    [Flags]
    public enum FlagType
    {
        Dynamic = 1<<1,
        Static = 1<<2,
        Public = 1<<3,
        Private = 1<<4,
        Getter = 1<<5,
        Setter = 1<<6,
        Function = 1<<7,
        Variable = 1<<8,
        Constructor = 1<<9,
        Intrinsic = 1<<10,
        Class = 1<<11,
        Interface = 1<<12,
        Package = 1<<13,
        Template = 1<<14,
        DocTemplate = 1<<15,
        CodeTemplate = 1<<16,
        Custom = 1<<17
    }
    
}
