namespace CodeFormatter.Handlers
{
    public class WrapOptions
    {
        public static int WRAP_NONE = 1;
        public static int WRAP_DONT_PROCESS = 2;
        public static int WRAP_ITEMS_PER_LINE = 4;
        public static int WRAP_FORMAT_NO_CRs = 4;
        public static int WRAP_BY_COLUMN = 8;
        public static int WRAP_BY_COLUMN_ONLY_ADD_CRS = 16;
        public static int WRAP_BY_TAG = 128;
        public static int WRAP_STYLE_INDENT_NORMAL = 1000;
        public static int WRAP_STYLE_INDENT_TO_WRAP_ELEMENT = 1001;
        private int mWrapType;
        private bool mBeforeSeparator; //usually, separator is 'comma'
        private bool mBeforeLogicalOperator; //And/Or
        private bool mBeforeArithmeticOperator;
        private bool mBeforeAssignmentOperator;
        private int mIndentStyle;

        public WrapOptions(int wrapType)
        {
            mWrapType = wrapType;
            mBeforeSeparator = false;
            mBeforeArithmeticOperator = false;
            mBeforeAssignmentOperator = false;
            mBeforeLogicalOperator = false;
            mIndentStyle = WRAP_STYLE_INDENT_NORMAL;
        }

        public int getWrapType()
        {
            return mWrapType;
        }
        public void setWrapType(int wrapType)
        {
            mWrapType = wrapType;
        }
        public int getIndentStyle()
        {
            return mIndentStyle;
        }
        public void setIndentStyle(int indentStyle)
        {
            mIndentStyle = indentStyle;
        }
        public bool isBeforeSeparator()
        {
            return mBeforeSeparator;
        }
        public void setBeforeSeparator(bool beforeSeparator)
        {
            mBeforeSeparator = beforeSeparator;
        }
        public bool isBeforeLogicalOperator()
        {
            return mBeforeLogicalOperator;
        }
        public void setBeforeLogicalOperator(bool beforeLogicalOperator)
        {
            mBeforeLogicalOperator = beforeLogicalOperator;
        }
        public bool isBeforeArithmeticOperator()
        {
            return mBeforeArithmeticOperator;
        }
        public void setBeforeArithmeticOperator(bool beforeArithmeticOperator)
        {
            mBeforeArithmeticOperator = beforeArithmeticOperator;
        }
        public bool isBeforeAssignmentOperator()
        {
            return mBeforeAssignmentOperator;
        }
        public void setBeforeAssignmentOperator(bool beforeAssignmentOperator)
        {
            mBeforeAssignmentOperator = beforeAssignmentOperator;
        }

    }

}
