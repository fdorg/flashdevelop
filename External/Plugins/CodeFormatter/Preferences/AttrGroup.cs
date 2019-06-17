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
        private int mSortMode;
        private List<string> mAttrs;
        private string mName;
        private int mWrapMode;
        private List<string> mRegexAttrs;
        private int mData; //depends on wrap mode.  
        private bool mIncludeStates;
        public static int Wrap_Data_Use_Default=-1;
        private static string Tag_name = "name=";
        private static string Tag_sort = "sort=";
        private static string Tag_includeStates = "includeStates=";
        private static string Tag_wrap = "wrap=";
        private static string Tag_attrs = "attrs=";
        private static string Tag_data = "data=";
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

        public int getWrapMode() 
        {
            return mWrapMode;
        }

        public void setWrapMode(int wrapMode) 
        {
            mWrapMode = wrapMode;
        }

        public string getName()
        {
            return mName;
        }

        public int getSortMode() 
        {
            return mSortMode;
        }
        public void setSortMode(int sortMode)
        {
            mSortMode = sortMode;
        }

        public List<string> getAttrs() {
            return mAttrs;
        }

        public void setName(string name) 
        {
            mName=name;
        }

        public bool isIncludeStates() 
        {
            return mIncludeStates;
        }

        public void setIncludeStates(bool includeStates) 
        {
            mIncludeStates = includeStates;
        }

        public AttrGroup copy()
        {
            List<string> attrs=new List<string>();
            attrs.AddRange(getAttrs());
            AttrGroup group=new AttrGroup(getName(), attrs, getSortMode(), getWrapMode(), isIncludeStates());
            group.setData(getData());
            return group;
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
            if (name==null) return null;
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
                string[] atts = attrString.Split(new string[]{GroupingSplitter}, StringSplitOptions.RemoveEmptyEntries);
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
    
        private void cacheRegexAttrs()
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

        public int getData()
        {
            return mData;
        }

        public void setData(int data) 
        {
            mData = data;
        }
    
    }

}