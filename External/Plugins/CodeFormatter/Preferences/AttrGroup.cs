using System;
using System.Text;
using System.Collections.Generic;
using CodeFormatter.Handlers;

namespace CodeFormatter.Preferences
{
	public class AttrGroup
	{
		private int mSortMode;
		private List<string> mAttrs;
		private string mName;
		private int mWrapMode;
		private List<string> mRegexAttrs;
		
		public AttrGroup(string name, List<string> attrs, int sortMode, int wrapMode)
		{
			mName = name;
			mAttrs = attrs;
			mSortMode = sortMode;
			mWrapMode = wrapMode;
			mRegexAttrs = null;
		}

		public int GetWrapMode() 
        {
			return mWrapMode;
		}

		public void SetWrapMode(int wrapMode) 
        {
			mWrapMode = wrapMode;
		}

		public String GetName()
		{
			return mName;
		}

		public int GetSortMode() 
        {
			return mSortMode;
		}

		public void SetSortMode(int sortMode) 
        {
			mSortMode = sortMode;
		}

		public List<string> GetAttrs() 
        {
			return mAttrs;
		}

		public void SetName(string name) 
        {
			mName = name;
		}

		public AttrGroup Copy()
		{
			List<string> attrs = new List<String>();
			attrs.AddRange(GetAttrs());
			return new AttrGroup(GetName(), attrs, GetSortMode(), GetWrapMode());
		}
		
		private const string Tag_name = "name=";
		private const string Tag_sort = "sort=";
		private const string Tag_wrap = "wrap=";
		private const string Tag_attrs = "attrs=";
		private const string TagSplitter = "|";
		public const string NewLineFlag = "\\n";
		public const char Attr_Group_Marker = '%';
		public const string Attr_Grouping_Splitter = ",";
		
		public string Save()
		{
			StringBuilder buffer = new StringBuilder();
			buffer.Append(Tag_name);
			buffer.Append(GetName());
			buffer.Append(TagSplitter);
			buffer.Append(Tag_sort);
			buffer.Append("" + GetSortMode());
			buffer.Append(TagSplitter);
			buffer.Append(Tag_wrap);
			buffer.Append("" + GetWrapMode());
			buffer.Append(TagSplitter);
			buffer.Append(Tag_attrs);
			foreach (string attr in GetAttrs()) 
            {
				buffer.Append(attr);
				buffer.Append(Attr_Grouping_Splitter);
			}
			buffer.Append(TagSplitter);
			return buffer.ToString();
		}
		
		private static String GetValue(String source, String tagName)
		{
			int index = source.IndexOf(tagName);
			int endIndex = source.IndexOf(TagSplitter, index);
			if (index < 0 || endIndex < 0) return null;
			return source.Substring(index + tagName.Length, endIndex - (index + tagName.Length));
		}
		
		public static AttrGroup Load(String data)
		{
			List<string> attrs = new List<string>();
			int sortMode = MXMLPrettyPrinter.MXML_Sort_AscByCase;
			int wrapMode = MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT;
			String name = GetValue(data, Tag_name);
			if (name == null) return null;
			String num = GetValue(data, Tag_sort);
			if (num != null)
			{
				try
				{
					sortMode = Convert.ToInt32(num);
				}
				catch (FormatException) {}
			}
			num = GetValue(data, Tag_wrap);
			if (num != null)
			{
				try
				{
					wrapMode = Convert.ToInt32(num);
				}
				catch (FormatException) {}
			}
			String attrString = GetValue(data, Tag_attrs);
			if (attrString != null)
			{
				String[] atts = attrString.Split(Attr_Grouping_Splitter[0]);
				foreach (string attr in atts) 
                {
					string attr2 = attr.Trim();
					if (attr2.Length > 0) attrs.Add(attr2);
				}
			}
			return new AttrGroup(name, attrs, sortMode, wrapMode);
		}
		
		private void CacheRegexAttrs()
		{
			if (mRegexAttrs != null) return;
			mRegexAttrs = new List<String>();
			foreach (string attr in mAttrs)
			{
				if (IsRegexString(attr))
				{
					mRegexAttrs.Add(attr);
				}
			}
		}
		
		public List<String> GetRegexAttrs()
		{
			CacheRegexAttrs();
			return mRegexAttrs;
		}
		
		public bool IsRegexAttr(String attr)
		{
			CacheRegexAttrs();
			return mRegexAttrs.Contains(attr);
		}
		
		public static bool IsRegexString(String str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				char c = str.ToCharArray()[i];
				if (AS3_exParser.IsIdentifierPart(c)) continue;
				if (c == ':' || c == '_' || c == '-') continue;
				return true;
			}
			return false;
		}

		override public String ToString()
		{
			StringBuilder buffer=new StringBuilder();
			buffer.Append(GetName());
			buffer.Append('(');
			foreach (String attr in GetAttrs()) 
            {
				buffer.Append(attr);
				buffer.Append(',');
			}
			buffer.Append(')');
			return buffer.ToString();
		}

	}

}
