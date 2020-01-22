using System;
using System.Collections.Generic;
using System.Text;
using CodeFormatter.Handlers;
using CodeFormatter.InfoCollector;
using PluginCore;

namespace CodeFormatter.Preferences
{
    public class AttrGroup
    {
        int mSortMode;
        readonly List<string> mAttrs;
        string mName;
        int mWrapMode;
        List<string> mRegexAttrs;
        int mData; //depends on wrap mode.  
        bool mIncludeStates;
        public static int Wrap_Data_Use_Default=-1;
        static readonly string Tag_name = "name=";
        static readonly string Tag_sort = "sort=";
        static readonly string Tag_includeStates = "includeStates=";
        static readonly string Tag_wrap = "wrap=";
        static readonly string Tag_attrs = "attrs=";
        static readonly string Tag_data = "data=";
        public static string TagSplitter = "|";
        public static string GroupingSplitter = ",";
        public static string SplitterEscape = "char(Splitter)";
    
        public AttrGroup(string name, List<string> attrs, int sortMode, int wrapMode, bool includeStates)
        {
            mName=name;
            mAttrs=attrs;
            mSortMode=sortMode;
            mWrapMode=wrapMode;
            mRegexAttrs=null;
            mIncludeStates=includeStates;
            mData=Wrap_Data_Use_Default;
        }

        public int getWrapMode() => mWrapMode;

        public void setWrapMode(int wrapMode) => mWrapMode = wrapMode;

        public string getName() => mName;

        public int getSortMode() => mSortMode;

        public void setSortMode(int sortMode) => mSortMode = sortMode;

        public List<string> getAttrs() => mAttrs;

        public void setName(string name) => mName=name;

        public bool isIncludeStates() => mIncludeStates;

        public void setIncludeStates(bool includeStates) => mIncludeStates = includeStates;

        public AttrGroup copy()
        {
            List<string> attrs=new List<string>();
            attrs.AddRange(getAttrs());
            AttrGroup result = new AttrGroup(getName(), attrs, getSortMode(), getWrapMode(), isIncludeStates());
            result.setData(getData());
            return result;
        }

        public string save()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(Tag_name);
            buffer.Append(getName().Replace(TagSplitter, SplitterEscape));
            buffer.Append(TagSplitter);
            buffer.Append(Tag_sort);
            buffer.Append(getSortMode().ToString());
            buffer.Append(TagSplitter);
            buffer.Append(Tag_includeStates);
            buffer.Append(isIncludeStates().ToString());
            buffer.Append(TagSplitter);
            buffer.Append(Tag_wrap);
            buffer.Append(getWrapMode().ToString());
            buffer.Append(TagSplitter);
            buffer.Append(Tag_attrs);
            foreach (string attr in getAttrs()) 
            {
                buffer.Append(attr.Replace(TagSplitter, SplitterEscape));
                buffer.Append(GroupingSplitter);
            }
            buffer.Append(TagSplitter);
            buffer.Append(Tag_data);
            buffer.Append(mData.ToString());
            buffer.Append(TagSplitter);
            return buffer.ToString();
        }
    
        public static string getValue(string source, string tagName)
        {
            try
            {
                int index = source.IndexOfOrdinal(tagName);
                int endIndex = source.IndexOfOrdinal(TagSplitter, index);
                if (index < 0 || endIndex < 0) return null;
                string value = source.Substring(index + tagName.Length, endIndex - (index + tagName.Length));
                value = value.Replace(SplitterEscape, TagSplitter);
                return value;
            }
            catch { return null; }
        }
    
        public static AttrGroup load(string data)
        {
            List<string> attrs=new List<string>();
            bool includeStates=true;
            int sortMode=MXMLPrettyPrinter.MXML_Sort_AscByCase;
            int wrapMode=MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT;
            string name=getValue(data, Tag_name);
            if (name is null) return null;
            string num=getValue(data, Tag_sort);
            if (num!=null)
            {
                try { sortMode=int.Parse(num); }
                catch {}
            }
            num=getValue(data, Tag_wrap);
            if (num!=null)
            {
                try { wrapMode=int.Parse(num); }
                catch {}
            }
            int wrapData = Wrap_Data_Use_Default;
            num = getValue(data, Tag_data);
            if (num!=null)
            {
                try { wrapData=int.Parse(num); }
                catch{}
            }
            string attrString = getValue(data, Tag_attrs);
            if (attrString != null)
            {
                string[] atts = attrString.Split(new[]{GroupingSplitter}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string attr in atts) 
                {
                    string attr2 = AntlrUtilities.asTrim(attr);
                    if (attr2.Length > 0) attrs.Add(attr2);
                }
            }
            string includeStatesData=getValue(data, Tag_includeStates);
            if (includeStatesData!=null)
            {
                includeStates=bool.Parse(includeStatesData);
            }
            AttrGroup group=new AttrGroup(name, attrs, sortMode, wrapMode, includeStates);
            group.setData(wrapData);
            return group;
        }

        void cacheRegexAttrs()
        {
            if (mRegexAttrs!=null) return;
            mRegexAttrs=new List<string>();
            foreach (string attr in mAttrs)
            {
                if (isRegexString(attr))
                {
                    mRegexAttrs.Add(attr);
                }
                if (isIncludeStates())
                {
                    mRegexAttrs.Add(attr + MXMLPrettyPrinter.StateRegexSuffix);
                }
            }
        }
    
        public List<string> getRegexAttrs()
        {
            cacheRegexAttrs();
            return mRegexAttrs;
        }
    
        public bool isRegexAttr(string attr)
        {
            cacheRegexAttrs();
            return mRegexAttrs.Contains(attr);
        }
    
        public static bool isRegexString(string str)
        {
            for (int i=0;i<str.Length;i++)
            {
                char c=str[i];
                if (InfoCollector.Utilities.isJavaIdentifierPart(c)) continue;          
                if (c==':' || c=='_' || c=='-') continue;       
                return true;
            }
            return false;
        }

        public string toString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(getName());
            buffer.Append("(");
            foreach (string attr in getAttrs()) 
            {
                buffer.Append(attr);
                buffer.Append(",");
            }
            buffer.Append(")");
            return buffer.ToString();
        }

        public int getData() => mData;

        public void setData(int data) => mData = data;
    }
}