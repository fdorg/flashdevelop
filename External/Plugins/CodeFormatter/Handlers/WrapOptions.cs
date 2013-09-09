using System;

namespace CodeFormatter.Handlers
{
	public class WrapOptions
	{
		public const int WRAP_NONE = 1;
		public const int WRAP_DONT_PROCESS = 2;
		public const int WRAP_FORMAT_NO_CRs = 4;
		public const int WRAP_BY_COLUMN = 8;
		public const int WRAP_BY_COLUMN_ONLY_ADD_CRS = 16;
		public const int WRAP_BY_TAG = 128;
		public const int WRAP_STYLE_INDENT_NORMAL = 1000;
		public const int WRAP_STYLE_INDENT_TO_WRAP_ELEMENT = 1001;
		
		private int mWrapType;
		private bool mBeforeSeparator; // Usually, separator is 'comma'
		private int mIndentStyle;
		
		public WrapOptions(int wrapType)
		{
			mWrapType = wrapType;
			mBeforeSeparator = false;
			mIndentStyle = WRAP_STYLE_INDENT_NORMAL;
		}
		
		public int WrapType
		{
			get { return this.mWrapType; }
			set { this.mWrapType = value; }
		}
		
		public bool BeforeSeparator
		{
			get { return this.mBeforeSeparator; }
			set { this.mBeforeSeparator = value; }
		}
		
		public int IndentStyle
		{
			get { return this.mIndentStyle; }
			set { this.mIndentStyle = value; }
		}

	}

}
