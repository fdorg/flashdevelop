/*
 * Misc classes used by the plugin
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using PluginCore;
using PluginCore.Managers;
using ASCompletion.Context;

namespace ASCompletion.Model
{
	/// <summary>
	/// Object representation of an Actionscript MemberModel
	/// </summary>
    [Serializable]
	public class MemberModel: ICloneable, IComparable
    {
		public static String TypedCallbackHLStart = "<[BGCOLOR=#2F90:NORMAL]"; // <- with alpha (0x02)
		public static String TypedCallbackHLEnd = "[/BGCOLOR]>";

        public FileModel InFile;
        public bool IsPackageLevel;

		public FlagType Flags;
        public Visibility Access;
		public string Namespace;
		public string Name;
        public List<MemberModel> Parameters;
		public string Type;
        public string Template;
		public string Comments;
        public string Value;
		public int LineFrom;
		public int LineTo;
        public List<ASMetaData> MetaDatas;

        public MemberModel()
        {
        }

        public MemberModel(string name, string type, FlagType flags, Visibility access)
        {
            Name = name;
            Type = type;
            Flags = flags;
            Access = access;
        }

        virtual public string FullName
        {
            get
            {
                if (Template == null) return Name;
                else return Name + Template;
            }
        }

        /// <summary>
		/// Clone member
		/// </summary>
		public Object Clone()
		{
			MemberModel copy = new MemberModel();
            copy.Name = Name;
            copy.Template = Template;
			copy.Flags = Flags;
            copy.Access = Access;
			copy.Namespace = Namespace;
            copy.InFile = InFile;
            copy.IsPackageLevel = IsPackageLevel;
            if (Parameters != null)
            {
                copy.Parameters = new List<MemberModel>();
                foreach (MemberModel param in Parameters)
                    copy.Parameters.Add(param.Clone() as MemberModel);
            }
			copy.Type = Type;
			copy.Comments = Comments;
            copy.Value = Value;
            copy.LineFrom = LineFrom;
            copy.LineTo = LineTo;
			return copy;
		}
		
		public override string ToString()
		{
			string res = FullName;
			string type = (Type != null && Type.Length > 0) ? FormatType(Type) : null;
			string comment = "";
			if ((Flags & FlagType.Function) > 0)
			{
				string functDecl = "(" + ParametersString(true) + ")";

				if ((Flags & FlagType.Variable) > 0 || (Flags & FlagType.Getter) > 0)
				{
					if (type != null && type.Length > 0)
						functDecl += ":" + type;

					res += " : Function" + TypedCallbackHLStart + functDecl + TypedCallbackHLEnd;
					return res;
				}

				res += " " + functDecl;
			}
			else if ((Flags & (FlagType.Setter | FlagType.Getter)) > 0)
			{
				if ((Flags & FlagType.Setter) > 0)
				{
					if (Parameters != null && Parameters.Count > 0 && Parameters[0].Type != null && Parameters[0].Type.Length > 0)
						return res + " : " + FormatType(Parameters[0].Type);
				}
			}

			if ((Flags & FlagType.Constructor) > 0)
				return res;
			
			if (type != null && type.Length > 0)
				res += " : " + type + comment;

            return res;
		}

		public string ToDeclarationString()
		{
			return ToDeclarationString(true, false);
		}
        public string ToDeclarationString(bool wrapWithSpaces, bool concatValue)
        {
			string colon = wrapWithSpaces ? " : " : ":";
            string res = FullName;
			string type = null;
			string comment = "";
            if ((Flags & (FlagType.Function | FlagType.Setter | FlagType.Getter)) > 0)
            {
				if ((Flags & FlagType.Function) > 0 && (Flags & FlagType.Getter | Flags & FlagType.Variable) > 0)
				{
					if ((Flags & FlagType.Variable) == 0)
						res += "()";

					type = "Function";
					if (Parameters != null && Parameters.Count > 0)
					{
						comment = "/*(" + ParametersString(true) + ")";
						if (Type != null && Type.Length > 0)
							comment += colon + FormatType(Type);
						comment += "*/";
					}
				}
				else
				{
					res += "(" + ParametersString(true) + ")";
				}
            }

			if ((type == null || type.Length == 0) && (Type != null && Type.Length > 0))
				type = FormatType(Type);

			if ((Flags & FlagType.Constructor) > 0)
				return res;
			else if (type != null && type.Length > 0)
				res += colon + type;

            res += comment;

			if (concatValue && Value != null)
				res += (wrapWithSpaces ? " = " : "=") + Value.Trim();

			return res;
        }

        public string ParametersString()
        {
            return ParametersString(false);
        }

        public string ParametersString(bool formated)
        {
            string res = "";
            if (Parameters != null && Parameters.Count > 0)
            {
                bool addSep = false;
                foreach (MemberModel param in Parameters)
                {
                    if (addSep) res += ", ";
                    else addSep = true;

					res += param.ToDeclarationString(false, true);
					/*
                    res += param.Name;
                    if (param.Type != null && param.Type.Length > 0)
                        res += ":" + (formated ? FormatType(param.Type) : param.Type);
                    if (param.Value != null)
                        res += " = " + param.Value.Trim();
					*/
                }
            }
            return res;
        }
		
		public override bool Equals(object obj)
		{
			if (!(obj is MemberModel)) 
				return false;
            MemberModel to = (MemberModel)obj;
			return Name == to.Name && Flags == to.Flags;
		}
		
		public override int GetHashCode() 
		{
			return (Name+Flags).GetHashCode();
		}
		
		public int CompareTo(object obj)
		{
			if (!(obj is MemberModel))
				throw new InvalidCastException("This object is not of type MemberModel");
            MemberModel to = (MemberModel)obj;
            if (Name == to.Name) return (int)Flags - (int)to.Flags;
            else return string.Compare(Name, to.Name, false);
		}

		static public string FormatType(string type)
		{
			return FormatType(type, false);
		}
        static public string FormatType(string type, bool allowBBCode)
        {
            if (type == null || type.Length == 0)
                return null;
            int p = type.IndexOf('@');
			if (p > 0)
			{
				string bbCodeOpen = allowBBCode ? "[BGCOLOR=#EEE:SUBTRACT]" : "";
				string bbCodeClose = allowBBCode ? "[/BGCOLOR]" : "";

				if (type.Substring(0, p) == "Array")
					return type.Substring(0, p) + bbCodeOpen + "/*" + type.Substring(p + 1) + "*/" + bbCodeClose;
				else if (type.IndexOf("<T>") > 0)
                    return type.Substring(0, type.IndexOf("<T>")) + bbCodeOpen + "<" + type.Substring(p + 1) + ">" + bbCodeClose;
                else
					return bbCodeOpen + "/*" + type.Substring(p + 1) + "*/" + bbCodeClose + type.Substring(0, p);
			}
			return type;
        }
	}
	
	/// <summary>
	/// Strong-typed MemberModel list with special merging/searching methods
	/// </summary>
    [Serializable]
	public class MemberList: IEnumerable
	{
		private List<MemberModel> items;
		private bool Sorted;
		
		public IEnumerator GetEnumerator()
		{
			return items.GetEnumerator();
		}
		
		public List<MemberModel> Items 
		{
			get {
				return items;
			}
		}

		public int Count
		{
			get {
				return items.Count;
			}
		}
		
		public MemberList()
		{
            items = new List<MemberModel>();
		}
		
		public MemberModel this[int index]
		{
			get {
				return items[index];
			}
			set {
				Sorted = false;
				items[index] = value;
			}
		}
		
		public int Add(MemberModel value)
		{
			Sorted = false;
            items.Add(value);
			return items.Count;
		}

        public int Add(MemberList list)
        {
            Sorted = false;
            items.AddRange(list.Items);
            return items.Count;
        }

        public void Insert(int index, MemberModel value)
		{
			Sorted = false;
			items.Insert(index, value);
		}
		
		public void Remove(MemberModel value)
		{
			items.Remove(value);
		}

        public void Remove(string name)
        {
            MemberModel member = Search(name, 0, 0);
            if (member != null) items.Remove(member);
        }

        public void Clear()
		{
			Sorted = true;
			items.Clear();
		}

        /// <summary>
        /// Return the first MemberModel instance match in the MemberList
        /// </summary>
        /// <param name="name">Member name to mach</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="acc">Visibility mask</param>
        /// <returns>First match</returns>
        public MemberModel Search(string name, FlagType mask, Visibility acc)
        {
            foreach (MemberModel m in items)
                if (((m.Flags & mask) == mask) 
                    && (acc == 0 || (m.Access & acc) > 0)
                    && m.Name == name) return m;
            return null;
        }

        /// <summary>
        /// Return all MemberModel instance matches in the MemberList
        /// </summary>
        /// <param name="name">Member name to match</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="acc">Visibility mask</param>
        /// <returns>All matches</returns>
        public MemberList MultipleSearch(string name, FlagType mask, Visibility acc) 
        {
			MemberList result = new MemberList();
			foreach (MemberModel m in items)
                if (((m.Flags & mask) == mask)
                    && (acc == 0 || (m.Access & acc) > 0)
                    && m.Name == name) result.Add(m);
			return result;
		}
		
		public void Sort()
		{
            this.Sort(null);
		}

        public void Sort(IComparer<MemberModel> comparer)
        {
            if (!Sorted)
            {
                items.Sort(comparer);
                Sorted = true;
            }
        }

		/// <summary>
		/// Merge one item into the list
		/// </summary>
		/// <param name="item">Item to merge</param>
		public void Merge(MemberModel item)
		{
            if (item == null) return;
			MemberList list = new MemberList();
			list.Add(item);
			Merge(list);
		}
		
		/// <summary>
		/// Merge SORTED lists without duplicate values
		/// </summary>
		/// <param name="list">Items to merge</param>
		public void Merge(MemberList list)
		{
            if (list == null) return;
            int index = 0;
            bool added;
            foreach (MemberModel m in list)
            {
                added = false;
                while (index < items.Count)
                {
                    if (m.Name.CompareTo(items[index].Name) <= 0)
                    {
                        if (m.Name != items[index].Name) items.Insert(index, m);
                        else if ((items[index].Flags & FlagType.Setter) > 0)
                        {
                            items.RemoveAt(index);
                            items.Insert(index, m);
                        }
                        added = true;
                        break;
                    }
                    index++;
                }
                if (!added) items.Add(m);
            }
		}

        /// <summary>
        /// Merge ORDERED (by line) lists
        /// </summary>
        /// <param name="list">Items to merge</param>
        public void MergeByLine(MemberList list)
        {
            if (list == null) return;
            int index = 0;
            bool added;
            foreach (MemberModel m in list)
            {
                added = false;
                while (index < items.Count)
                {
                    if (m.LineFrom <= items[index].LineFrom)
                    {
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
		/// </summary>
		/// <param name="list">Items to merge</param>
		public void Merge(MemberList list, FlagType mask, Visibility acc)
		{
            if (list == null) return;
			int index = 0;
			bool added;
			foreach (MemberModel m in list)
			if ((m.Flags & mask) == mask && (m.Access & acc) > 0)
			{
				added = false;
				while (index < items.Count)
				{
					if (m.Name.CompareTo(items[index].Name) <= 0)
					{
                        if (m.Name != items[index].Name) items.Insert(index, m);
                        else if ((items[index].Flags & FlagType.Setter) > 0)
                        {
                            items.RemoveAt(index);
                            items.Insert(index, m);
                        }
						added = true;
						break;
					}
					index++;
				}
				if (!added) items.Add(m);
			}
		}

        public void RemoveAllWithFlag(FlagType flag)
        {
            items.RemoveAll(m => (m.Flags & flag) > 0);   
        }

        public void RemoveAllWithoutFlag(FlagType flag)
        {
            items.RemoveAll(m => (m.Flags & flag) == 0);
        }
	}

    public class ByKindMemberComparer : IComparer<MemberModel>
    {

        public int Compare(MemberModel a, MemberModel b)
        {
            return getPriority(a.Flags).CompareTo(getPriority(b.Flags));
        }

        private uint getPriority(FlagType flag)
        {
            if ((flag & FlagType.Constant) > 0) return 4;
            else if ((flag & FlagType.Variable) > 0) return 3;
            else if ((flag & (FlagType.Getter | FlagType.Setter)) > 0) return 2;
            else return 1;
        }

    }

    public class SmartMemberComparer : IComparer<MemberModel>
    {
        public int Compare(MemberModel a, MemberModel b)
        {
            int cmp = getPriority(a).CompareTo(getPriority(b));
            return cmp != 0 ? cmp : StringComparer.Ordinal.Compare(a.Name,b.Name);
        }

        private uint getPriority(MemberModel m)
        {
            uint visibility_pri;
            if ((m.Access & Visibility.Public) > 0) visibility_pri = 1;
            else if ((m.Access & Visibility.Protected) > 0) visibility_pri = 2;
            else if ((m.Access & Visibility.Private) > 0) visibility_pri = 3;
            else visibility_pri = 4;

            uint type_pri;
            if ((m.Flags & FlagType.Constant) > 0) type_pri = 50;
            else if ((m.Flags & FlagType.Variable) > 0) type_pri = 30;
            else if ((m.Flags & (FlagType.Getter | FlagType.Setter)) > 0) type_pri = 30;
            else if ((m.Flags & FlagType.Constructor) > 0) type_pri = 20;
            else type_pri = 10;

            return visibility_pri + type_pri;
        }

    }

    public class ByDeclarationPositionMemberComparer : IComparer<MemberModel>
    {

        public int Compare(MemberModel a, MemberModel b)
        {
            return a.LineFrom - b.LineFrom;
        }

    }
}
