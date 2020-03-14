using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PluginCore;

namespace ASCompletion.Model
{
    /// <summary>
    /// Object representation of an ActionScript MemberModel
    /// </summary>
    [Serializable]
    public class MemberModel: ICloneable, IComparable
    {
        const string TypedCallbackHLStart = "<[BGCOLOR=#2F90:NORMAL]"; // <- with alpha (0x02)
        const string TypedCallbackHLEnd = "[/BGCOLOR]>";

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
        public int ValueEndPosition = -1;
        public int LineFrom;
        public int LineTo;
        public List<ASMetaData> MetaDatas;
        public int StartPosition = -1;

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

        public virtual string FullName
        {
            get
            {
                if (Template is null) return Name;
                return Name + Template;
            }
        }

        /// <inheritdoc />
        public object Clone()
        {
            var result = new MemberModel();
            result.Name = Name;
            result.Template = Template;
            result.Flags = Flags;
            result.Access = Access;
            result.Namespace = Namespace;
            result.InFile = InFile;
            result.IsPackageLevel = IsPackageLevel;
            result.Parameters = Parameters?.Select(it => (MemberModel) it.Clone()).ToList();
            result.Type = Type;
            result.Comments = Comments;
            result.Value = Value;
            result.ValueEndPosition = ValueEndPosition;
            result.LineFrom = LineFrom;
            result.LineTo = LineTo;
            result.StartPosition = StartPosition;
            return result;
        }
        
        public override string ToString()
        {
            var result = FullName;
            var type = !string.IsNullOrEmpty(Type) ? FormatType(Type) : null;
            if ((Flags & FlagType.Function) > 0)
            {
                var declaration = "(" + ParametersString(true) + ")";
                if ((Flags & FlagType.Variable) > 0)
                {
                    if (!string.IsNullOrEmpty(type)) declaration += ":" + type;
                    result += " : Function" + TypedCallbackHLStart + declaration + TypedCallbackHLEnd;
                    return result;
                }
                result += " " + declaration;
            }
            else if ((Flags & (FlagType.Setter | FlagType.Getter)) > 0)
            {
                if ((Flags & FlagType.Setter) > 0)
                {
                    if (!Parameters.IsNullOrEmpty() && !string.IsNullOrEmpty(Parameters[0].Type))
                        return result + " : " + FormatType(Parameters[0].Type);
                }
            }
            if ((Flags & FlagType.Constructor) > 0) return result;
            if (!string.IsNullOrEmpty(type)) result += " : " + type;
            return result;
        }

        public string ToDeclarationString() => ToDeclarationString(true, false);

        public string ToDeclarationString(bool wrapWithSpaces, bool concatValue)
        {
            string result = FullName;
            string colon = wrapWithSpaces ? " : " : ":";
            string type = null;
            string comment = "";
            if ((Flags & (FlagType.Function | FlagType.Setter | FlagType.Getter)) > 0)
            {
                if ((Flags & FlagType.Function) > 0 && (Flags & FlagType.Getter | Flags & FlagType.Variable) > 0)
                {
                    if ((Flags & FlagType.Variable) == 0)
                        result += "()";

                    type = "Function";
                    if (!Parameters.IsNullOrEmpty())
                    {
                        comment = "/*(" + ParametersString(true) + ")";
                        if (!string.IsNullOrEmpty(Type)) comment += colon + FormatType(Type);
                        comment += "*/";
                    }
                }
                else
                {
                    result += "(" + ParametersString(true) + ")";
                }
            }

            if (string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(Type))
                type = FormatType(Type);

            if ((Flags & FlagType.Constructor) > 0) return result;
            if (!string.IsNullOrEmpty(type)) result += colon + type;

            result += comment;

            if (concatValue && Value != null)
                result += (wrapWithSpaces ? " = " : "=") + Value.Trim();

            return result;
        }

        public string ParametersString() => ParametersString(false);

        public string ParametersString(bool formatted)
        {
            var result = "";
            if (!Parameters.IsNullOrEmpty())
            {
                var addSep = false;
                foreach (var param in Parameters)
                {
                    if (addSep) result += ", ";
                    else addSep = true;
                    result += param.ToDeclarationString(false, true);
                }
            }
            return result;
        }
        
        public override bool Equals(object obj) => obj is MemberModel to && Name == to.Name && Flags == to.Flags;

        public override int GetHashCode() => (Name + Flags).GetHashCode();

        public int CompareTo(object obj)
        {
            if (!(obj is MemberModel)) throw new InvalidCastException("This object is not of type MemberModel");
            var to = (MemberModel)obj;
            if (Name == to.Name) return Flags.CompareTo(to.Flags);
            return string.Compare(Name, to.Name, false);
        }

        public static string FormatType(string type) => FormatType(type, false);

        public static string FormatType(string type, bool allowBBCode)
        {
            if (string.IsNullOrEmpty(type)) return null;
            var p = type.IndexOf('@');
            if (p == -1) return type;
            var bbCodeOpen = allowBBCode ? "[BGCOLOR=#EEE:SUBTRACT]" : "";
            var bbCodeClose = allowBBCode ? "[/BGCOLOR]" : "";
            if (type.Substring(0, p) == "Array") return $"{type.Substring(0, p)}{bbCodeOpen}/*{type.Substring(p + 1)}*/{bbCodeClose}";
            if (type.Contains("<T>", out var p1) && p1 > 0) return $"{type.Substring(0, p1)}{bbCodeOpen}<{type.Substring(p + 1)}>{bbCodeClose}";
            return $"{bbCodeOpen}/*{type.Substring(p + 1)}*/{bbCodeClose}{type.Substring(0, p)}";
        }
    }
    
    /// <summary>
    /// Strong-typed MemberModel list with special merging/searching methods
    /// </summary>
    [Serializable]
    public class MemberList: IEnumerable<MemberModel>
    {
        bool sorted;

        public IEnumerator<MemberModel> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        readonly List<MemberModel> items = new List<MemberModel>();

        public List<MemberModel> Items => items;

        public int Count => items.Count;

        public MemberList()
        {
        }

        public MemberList(IEnumerable<MemberModel> list)
        {
            Items.AddRange(list);
        }
        
        public MemberModel this[int index]
        {
            get => items[index];
            set
            {
                sorted = false;
                items[index] = value;
            }
        }
        
        public int Add(MemberModel value)
        {
            sorted = false;
            items.Add(value);
            return items.Count;
        }

        public int Add(MemberList list)
        {
            sorted = false;
            items.AddRange(list);
            return items.Count;
        }

        public void Insert(int index, MemberModel value)
        {
            sorted = false;
            items.Insert(index, value);
        }
        
        public void Remove(MemberModel value) => items.Remove(value);

        public void Remove(string name)
        {
            var member = Search(name, 0, 0);
            if (member != null) items.Remove(member);
        }

        public void Clear()
        {
            sorted = true;
            items.Clear();
        }

        public bool Contains(string name, FlagType mask, Visibility access) => Search(name, mask, access) != null;

        /// <summary>
        /// Return the first MemberModel instance match in the MemberList
        /// </summary>
        /// <param name="name">Member name to mach</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="access">Visibility mask</param>
        /// <returns>First match</returns>
        public MemberModel Search(string name, FlagType mask, Visibility access)
        {
            var count = items.Count;
            for (var i = 0; i < count; i++)
            {
                var m = items[i];
                if (((m.Flags & mask) == mask)
                    && (access == 0 || (m.Access & access) > 0)
                    && m.Name == name) return m;
            }
            return null;
        }

        /// <summary>
        /// Return all MemberModel instance matches in the MemberList
        /// </summary>
        /// <param name="name">Member name to match</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="access">Visibility mask</param>
        /// <returns>All matches</returns>
        public MemberList MultipleSearch(string name, FlagType mask, Visibility access) 
        {
            var result = new MemberList();
            foreach (var m in items)
                if ((m.Flags & mask) == mask
                    && (access == 0 || (m.Access & access) > 0)
                    && m.Name == name) result.Add(m);
            return result;
        }
        
        public void Sort() => Sort(null);

        public void Sort(IComparer<MemberModel> comparer)
        {
            if (sorted) return;
            items.Sort(comparer);
            sorted = true;
        }

        /// <summary>
        /// Merge one item into the list
        /// </summary>
        /// <param name="item">Item to merge</param>
        public void Merge(MemberModel item)
        {
            if (item != null) Merge(new MemberList {item});
        }
        
        /// <summary>
        /// Merge SORTED lists without duplicate values
        /// </summary>
        /// <param name="list">Items to merge</param>
        public void Merge(IEnumerable<MemberModel> list)
        {
            if (list is null) return;
            var index = 0;
            foreach (var it in list)
            {
                var added = false;
                while (index < items.Count)
                {
                    var item = items[index];
                    if (it.Name.CompareTo(item.Name) <= 0)
                    {
                        if (it.Name != item.Name) items.Insert(index, it);
                        else if ((item.Flags & FlagType.Setter) > 0)
                        {
                            items.RemoveAt(index);
                            items.Insert(index, it);
                        }
                        added = true;
                        break;
                    }
                    index++;
                }
                if (!added) items.Add(it);
            }
        }

        public void MergeByLine(MemberModel item)
        {
            if (item is null) return;
            var index = 0;
            var added = false;
            while (index < items.Count)
            {
                if (item.LineFrom <= items[index].LineFrom)
                {
                    items.Insert(index, item);
                    added = true;
                    break;
                }
                index++;
            }
            if (!added) items.Add(item);
        }

        /// <summary>
        /// Merge ORDERED (by line) lists
        /// </summary>
        /// <param name="list">Items to merge</param>
        public void MergeByLine(MemberList list)
        {
            if (list is null) return;
            var index = 0;
            foreach (MemberModel item in list)
            {
                var added = false;
                while (index < items.Count)
                {
                    if (item.LineFrom <= items[index].LineFrom)
                    {
                        items.Insert(index, item);
                        added = true;
                        break;
                    }
                    index++;
                }
                if (!added) items.Add(item);
            }
        }

        /// <summary>
        /// Merge selected items from the SORTED lists without duplicate values
        /// </summary>
        /// <param name="list">Items to merge</param>
        public void Merge(MemberList list, FlagType mask, Visibility access)
        {
            if (list is null) return;
            int index = 0;
            foreach (MemberModel m in list)
                if ((m.Flags & mask) == mask && (m.Access & access) > 0)
                {
                    var added = false;
                    while (index < items.Count)
                    {
                        var item = items[index];
                        if (m.Name.CompareTo(item.Name) <= 0)
                        {
                            if (m.Name != item.Name) Items.Insert(index, m);
                            else if ((item.Flags & FlagType.Setter) > 0)
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

        public void RemoveAllWithFlag(FlagType flag) => Items.RemoveAll(m => (m.Flags & flag) > 0);

        public void RemoveAllWithoutFlag(FlagType flag) => Items.RemoveAll(m => (m.Flags & flag) == 0);
    }

    public class ByKindMemberComparer : IComparer<MemberModel>
    {
        public int Compare(MemberModel a, MemberModel b) => GetPriority(a.Flags).CompareTo(GetPriority(b.Flags));

        static uint GetPriority(FlagType flag)
        {
            if ((flag & FlagType.Constant) > 0) return 4;
            if ((flag & FlagType.Variable) > 0) return 3;
            if ((flag & (FlagType.Getter | FlagType.Setter)) > 0) return 2;
            return 1;
        }
    }

    public class SmartMemberComparer : IComparer<MemberModel>
    {
        public int Compare(MemberModel a, MemberModel b)
        {
            var cmp = GetPriority(a).CompareTo(GetPriority(b));
            return cmp != 0 ? cmp : StringComparer.Ordinal.Compare(a.Name,b.Name);
        }

        static uint GetPriority(MemberModel m)
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
        public int Compare(MemberModel a, MemberModel b) => a.LineFrom - b.LineFrom;
    }

    /// <summary>
    /// Compare members based on import name
    /// </summary>
    public class CaseSensitiveImportComparer : IComparer<MemberModel>
    {
        static int GetPackageTypeSeparation(string import)
        {
            var dot = import.IndexOf('.');
            var lastDot = -1;
            var max = import.Length - 1;
            while (dot > 0 && dot < max)
            {
                if (char.IsUpper(import[dot + 1]))
                    return dot;
                lastDot = dot;
                dot = import.IndexOf('.', dot + 1);
            }
            if (dot < 0 || dot >= max) return lastDot;
            if (dot == 0) return -1;
            return dot;
        }

        public static int CompareImports(string import1, string import2)
        {
            IComparer cmp = StringComparer.Ordinal;
            var d1 = GetPackageTypeSeparation(import1);
            var d2 = GetPackageTypeSeparation(import2);
            // one or both imports do not have a package
            if (d1 < 0) 
            {
                if (d2 > 0) return -1;
                return cmp.Compare(import1, import2);
            }
            if (d2 < 0) 
            {
                if (d1 > 0) return 1;
                return cmp.Compare(import1, import2);
            }
            // compare package
            var pkg1 = import1.Substring(0, d1);
            var pkg2 = import2.Substring(0, d2);
            var result = cmp.Compare(pkg1, pkg2);
            if (result != 0) return result;
            // compare type
            var tp1 = import1.Substring(d1 + 1);
            var tp2 = import2.Substring(d2 + 1);
            result = cmp.Compare(tp1, tp2);
            return result;
        }

#if DEBUG 
        static void Assert(int res, int expected)
        {
            System.Diagnostics.Debug.Assert(res == expected, res + " was not expected " + expected);
        }

        static CaseSensitiveImportComparer()
        {
            // poor man's unit tests
            Assert(GetPackageTypeSeparation("a.b.C"), 3);
            Assert(GetPackageTypeSeparation("a.b.c"), 3);
            Assert(GetPackageTypeSeparation("a.b.C.D"), 3);
            Assert(GetPackageTypeSeparation("a"), -1);
            Assert(GetPackageTypeSeparation(".a"), -1);
            Assert(GetPackageTypeSeparation("a."), -1);
            Assert(GetPackageTypeSeparation("a.b.c."), 3);

            Assert(CompareImports("a", "A"), 32);
            Assert(CompareImports("a", "b"), -1);
            Assert(CompareImports("b", "a"), 1);
            Assert(CompareImports("a", "a"), 0);
            Assert(CompareImports("a.A", "b"), 1);
            Assert(CompareImports("a", "b.B"), -1);
            Assert(CompareImports("a.A", "b.A"), -1);
            Assert(CompareImports("b.A", "a.A"), 1);
            Assert(CompareImports("a.A", "a.A"), 0);
            Assert(CompareImports("a.A", "a.B"), -1);
            Assert(CompareImports("b.A", "a.A"), 1);
            Assert(CompareImports("a.A", "a.a"), -32);
            Assert(CompareImports("a.MathReal", "a.Mathematics"), -19);
        }
#endif

        public int Compare(string import1, string import2) => CompareImports(import1, import2);

        public int Compare(MemberModel item1, MemberModel item2) => CompareImports(item1.Type, item2.Type);
    }
}