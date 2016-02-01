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
        private List<String> mAttrs;
        private String mName;
        private int mWrapMode;
        private List<String> mRegexAttrs;
        private int mData; //depends on wrap mode.  
        private bool mIncludeStates;
        public static int Wrap_Data_Use_Default=-1;
        private static String Tag_name = "name=";
        private static String Tag_sort = "sort=";
        private static String Tag_includeStates = "includeStates=";
        private static String Tag_wrap = "wrap=";
        private static String Tag_attrs = "attrs=";
        private static String Tag_data = "data=";
        public static String TagSplitter = "|";
        public static String GroupingSplitter = ",";
        public static String SplitterEscape = "char(Splitter)";
    
        public AttrGroup(String name, List<String> attrs, int sortMode, int wrapMode, bool includeStates)
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

        public String getName()
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

        public List<String> getAttrs() {
            return mAttrs;
        }

        public void setName(String name) 
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
            List<String> attrs=new List<String>();
            attrs.AddRange(getAttrs());
            AttrGroup group=new AttrGroup(getName(), attrs, getSortMode(), getWrapMode(), isIncludeStates());
            group.setData(getData());
            return group;
        }

        public String save()
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
            foreach (String attr in getAttrs()) 
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
    
        public static String getValue(String source, String tagName)
        {
            try
            {
                int index = source.IndexOfOrdinal(tagName);
                int endIndex = source.IndexOfOrdinal(TagSplitter, index);
                if (index < 0 || endIndex < 0) return null;
                String value = source.Substring(index + tagName.Length, endIndex - (index + tagName.Length));
                value = value.Replace(SplitterEscape, TagSplitter);
                return value;
            }
            catch { return null; }
        }
    
        public static AttrGroup load(String data)
        {
            List<String> attrs=new List<String>();
            bool includeStates=true;
            int sortMode=MXMLPrettyPrinter.MXML_Sort_AscByCase;
            int wrapMode=MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT;
            String name=getValue(data, Tag_name);
            if (name==null) return null;
            String num=getValue(data, Tag_sort);
            if (num!=null)
            {
                try { sortMode=Int32.Parse(num); }
                catch {}
            }
            num=getValue(data, Tag_wrap);
            if (num!=null)
            {
                try { wrapMode=Int32.Parse(num); }
                catch {}
            }
            int wrapData = Wrap_Data_Use_Default;
            num = getValue(data, Tag_data);
            if (num!=null)
            {
                try { wrapData=Int32.Parse(num); }
                catch{}
            }
            String attrString = getValue(data, Tag_attrs);
            if (attrString != null)
            {
                String[] atts = attrString.Split(new string[]{GroupingSplitter}, StringSplitOptions.RemoveEmptyEntries);
                foreach (String attr in atts) 
                {
                    String attr2 = AntlrUtilities.asTrim(attr);
                    if (attr2.Length > 0) attrs.Add(attr2);
                }
            }
            String includeStatesData=getValue(data, Tag_includeStates);
            if (includeStatesData!=null)
            {
                includeStates=Boolean.Parse(includeStatesData);
            }
            AttrGroup group=new AttrGroup(name, attrs, sortMode, wrapMode, includeStates);
            group.setData(wrapData);
            return group;
        }
    
        private void cacheRegexAttrs()
        {
            if (mRegexAttrs!=null) return;
            mRegexAttrs=new List<String>();
            foreach (String attr in mAttrs)
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
    
        public List<String> getRegexAttrs()
        {
            cacheRegexAttrs();
            return mRegexAttrs;
        }
    
        public bool isRegexAttr(String attr)
        {
            cacheRegexAttrs();
            return mRegexAttrs.Contains(attr);
        }
    
        public static bool isRegexString(String str)
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

        public String toString()
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(getName());
            buffer.Append("(");
            foreach (String attr in getAttrs()) 
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