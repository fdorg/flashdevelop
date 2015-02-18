using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using CodeFormatter.Preferences;
using CodeFormatter.Handlers;
using PluginCore.Localization;

namespace CodeFormatter
{
	[Serializable]
	public class Settings
	{
		public const char LineSplitter = '\n';
		private bool pref_Flex_UseTabs = true;

		/////////////// ActionScript /////////////////////////////////////

		private int pref_AS_SpacesBeforeComma = 0;
		private int pref_AS_SpacesAfterComma = 1;
		private int pref_AS_SpacesAroundColons = 0;
		private bool pref_AS_UseGlobalSpacesAroundColons = true;
		private int pref_AS_AdvancedSpacesBeforeColons = 0;
		private int pref_AS_AdvancedSpacesAfterColons = 0;
		private int pref_AS_BlankLinesBeforeFunctions = 1;
		private int pref_AS_BlankLinesBeforeClasses = 1;
		private int pref_AS_BlankLinesBeforeProperties = 0;
		private int pref_AS_BlankLinesBeforeControlStatements = 0;
		private bool pref_AS_KeepBlankLines = false;
		private int pref_AS_BlankLinesToKeep = 1;
		private bool pref_AS_OpenBraceOnNewLine = true;
		private bool pref_AS_ElseOnNewLine = true;
		private bool pref_AS_CatchOnNewLine = true;
		private bool pref_AS_ElseIfOnSameLine = true;
		private int pref_AS_MaxLineLength = 200;
		private int pref_AS_SpacesAroundAssignment = 1;
		private int pref_AS_SpacesAroundSymbolicOperator = 1;
		private bool pref_AS_KeepSLCommentsOnColumn1 = true;
		private bool pref_AS_BreakLinesBeforeComma = false;
        private WrapType pref_AS_WrapExpressionMode = WrapType.None;
        private WrapType pref_AS_WrapMethodDeclMode = WrapType.None;
        private WrapType pref_AS_WrapMethodCallMode = WrapType.None;
        private WrapType pref_AS_WrapArrayDeclMode = WrapType.None;
        private WrapType pref_AS_WrapXMLMode = WrapType.DontProcess;
        private WrapIndent pref_AS_WrapIndentStyle = WrapIndent.Normal;
		private bool pref_AS_CollapseSpacesForAdjacentParens = true;
		private bool pref_AS_NewlineAfterBindable = true;
		private int pref_AS_SpacesAfterLabel = 1;
		private bool pref_AS_TrimTrailingWhitespace = false;
		private bool pref_AS_PutEmptyStatementsOnNewLine = true;
		private int pref_AS_SpacesBeforeOpenControlParen = 1;
		private bool pref_AS_AlwaysGenerateIndent = true;
		private bool pref_AS_DontIndentPackageItems = false;
		private bool pref_AS_LeaveExtraWhitespaceAroundVarDecls = false;
        private BraceStyle pref_AS_BraceStyle = BraceStyle.AfterLine;
		private bool pref_AS_UseBraceStyle = true;
		private int pref_AS_SpacesInsideParens = 0;
		private bool pref_AS_UseGlobalSpacesInsideParens = true;
		private int pref_AS_AdvancedSpacesInsideArrayDeclBrackets = 1;
		private int pref_AS_AdvancedSpacesInsideArrayRefBrackets = 0;
		private int pref_AS_AdvancedSpacesInsideLiteralBraces = 1;
		private int pref_AS_AdvancedSpacesInsideParens = 0;
		private bool pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters = false;
		private int pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters = 0;
		private bool pref_AS_DoAutoFormat = true;
		private bool pref_AS_AutoFormatStyle = true;
        private bool pref_AS_IndentMultilineComments = true;
        //
        private string pref_AS_MetaTagsOnSameLineAsTargetFunction = "";
        private string pref_AS_MetaTagsOnSameLineAsTargetProperty = "";
		
		////////////////// MXML ///////////////////////////////////////

		private int pref_MXML_SpacesAroundEquals = 0;
		private bool pref_MXML_SortExtraAttrs = false;
		private bool pref_MXML_AddNewlineAfterLastAttr = false;
		private string pref_MXML_SortAttrData = "";
        private SortMode pref_MXML_SortAttrMode = SortMode.UseData;
		private int pref_MXML_MaxLineLength = 200;
        private WrapMode pref_MXML_AttrWrapMode = WrapMode.CountPerLine;
		private int pref_MXML_AttrsPerLine = 1;
		private bool pref_MXML_KeepBlankLines = true;
        private WrapIndent pref_MXML_WrapIndentStyle = WrapIndent.WrapElement;
		private string pref_MXML_TagsCanFormat = "mx:List,fx:List";
		private string pref_MXML_TagsCannotFormat = "mx:String,fx:String";
		private string pref_MXML_TagsWithBlankLinesBefore = "";
		private int pref_MXML_BlankLinesBeforeTags = 1;
		private string pref_MXML_AttrGroups = "";
		private bool pref_MXML_UseAttrsToKeepOnSameLine = false;
		private int pref_MXML_AttrsToKeepOnSameLine = 4;
		private int pref_MXML_SpacesBeforeEmptyTagEnd = 1;
		private bool pref_MXML_RequireCDATAForASFormatting = false;
		private string pref_MXML_TagsWithASContent = "";
		private bool pref_MXML_AutoFormatStyle = true;
		private bool pref_MXML_DoAutoFormat = true;
        //
        private string pref_MXML_TagsDoNotFormatInside = "";
        private string pref_MXML_ParentTagsWithBlankLinesAfter = "";

        ////////////////// ActionScript ///////////////////////////////////////

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Use Tabs")]
		public bool Pref_Flex_UseTabs
		{
			get { return this.pref_Flex_UseTabs; }
			set { this.pref_Flex_UseTabs = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Spaces Before Comma")]
		public int Pref_AS_SpacesBeforeComma
		{
			get { return this.pref_AS_SpacesBeforeComma; }
			set { this.pref_AS_SpacesBeforeComma = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces After Comma")]
		public int Pref_AS_SpacesAfterComma
		{
			get { return this.pref_AS_SpacesAfterComma; }
			set { this.pref_AS_SpacesAfterComma = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Spaces Around Colons")]
		public int Pref_AS_SpacesAroundColons
		{
			get { return this.pref_AS_SpacesAroundColons; }
			set { this.pref_AS_SpacesAroundColons = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Use Global Spaces Around Colons")]
		public bool Pref_AS_UseGlobalSpacesAroundColons
		{
			get { return this.pref_AS_UseGlobalSpacesAroundColons; }
			set { this.pref_AS_UseGlobalSpacesAroundColons = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Before Colons")]
		public int Pref_AS_AdvancedSpacesBeforeColons
		{
			get { return this.pref_AS_AdvancedSpacesBeforeColons; }
			set { this.pref_AS_AdvancedSpacesBeforeColons = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces After Colons")]
		public int Pref_AS_AdvancedSpacesAfterColons
		{
			get { return this.pref_AS_AdvancedSpacesAfterColons; }
			set { this.pref_AS_AdvancedSpacesAfterColons = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines Before Functions")]
		public int Pref_AS_BlankLinesBeforeFunctions
		{
			get { return this.pref_AS_BlankLinesBeforeFunctions; }
			set { this.pref_AS_BlankLinesBeforeFunctions = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines Before Classes")]
		public int Pref_AS_BlankLinesBeforeClasses
		{
			get { return this.pref_AS_BlankLinesBeforeClasses; }
			set { this.pref_AS_BlankLinesBeforeClasses = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines Before Properties")]
		public int Pref_AS_BlankLinesBeforeProperties
		{
			get { return this.pref_AS_BlankLinesBeforeProperties; }
			set { this.pref_AS_BlankLinesBeforeProperties = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines Before Control Statements")]
		public int Pref_AS_BlankLinesBeforeControlStatements
		{
			get { return this.pref_AS_BlankLinesBeforeControlStatements; }
			set { this.pref_AS_BlankLinesBeforeControlStatements = value; }
		}

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Keep Blank Lines")]
		public bool Pref_AS_KeepBlankLines
		{
			get { return this.pref_AS_KeepBlankLines; }
			set { this.pref_AS_KeepBlankLines = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines To Keep")]
		public int Pref_AS_BlankLinesToKeep
		{
			get { return this.pref_AS_BlankLinesToKeep; }
			set { this.pref_AS_BlankLinesToKeep = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Open Brace On New Line")]
		public bool Pref_AS_OpenBraceOnNewLine
		{
			get { return this.pref_AS_OpenBraceOnNewLine; }
			set { this.pref_AS_OpenBraceOnNewLine = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Else On New Line")]
		public bool Pref_AS_ElseOnNewLine
		{
			get { return this.pref_AS_ElseOnNewLine; }
			set { this.pref_AS_ElseOnNewLine = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Catch On New Line")]
		public bool Pref_AS_CatchOnNewLine
		{
			get { return this.pref_AS_CatchOnNewLine; }
			set { this.pref_AS_CatchOnNewLine = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("ElseIf On Same Line")]
		public bool Pref_AS_ElseIfOnSameLine
		{
			get { return this.pref_AS_ElseIfOnSameLine; }
			set { this.pref_AS_ElseIfOnSameLine = value; }
		}

        [DefaultValue(200)]
        [Category("ActionScript")]
        [DisplayName("Max Line Length")]
		public int Pref_AS_MaxLineLength
		{
			get { return this.pref_AS_MaxLineLength; }
			set { this.pref_AS_MaxLineLength = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces Around Assignment")]
		public int Pref_AS_SpacesAroundAssignment
		{
			get { return this.pref_AS_SpacesAroundAssignment; }
			set { this.pref_AS_SpacesAroundAssignment = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces Around Symbolic Operator")]
		public int Pref_AS_SpacesAroundSymbolicOperator
		{
			get { return this.pref_AS_SpacesAroundSymbolicOperator; }
			set { this.pref_AS_SpacesAroundSymbolicOperator = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Keep Single Line Comments On Column One")]
		public bool Pref_AS_KeepSLCommentsOnColumn1
		{
			get { return this.pref_AS_KeepSLCommentsOnColumn1; }
			set { this.pref_AS_KeepSLCommentsOnColumn1 = value; }
		}

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Break Lines Before Comma")]
		public bool Pref_AS_BreakLinesBeforeComma
		{
			get { return this.pref_AS_BreakLinesBeforeComma; }
			set { this.pref_AS_BreakLinesBeforeComma = value; }
		}

        [DefaultValue(WrapType.None)]
        [Category("ActionScript")]
        [DisplayName("Wrap Expression Mode")]
		public WrapType Pref_AS_WrapExpressionMode
		{
			get { return this.pref_AS_WrapExpressionMode; }
			set { this.pref_AS_WrapExpressionMode = value; }
		}

        [DefaultValue(WrapType.None)]
        [Category("ActionScript")]
        [DisplayName("Wrap Method Declaration Mode")]
        public WrapType Pref_AS_WrapMethodDeclMode
		{
			get { return this.pref_AS_WrapMethodDeclMode; }
			set { this.pref_AS_WrapMethodDeclMode = value; }
		}

        [DefaultValue(WrapType.None)]
        [Category("ActionScript")]
        [DisplayName("Wrap Method Call Mode")]
        public WrapType Pref_AS_WrapMethodCallMode
		{
			get { return this.pref_AS_WrapMethodCallMode; }
			set { this.pref_AS_WrapMethodCallMode = value; }
		}

        [DefaultValue(WrapType.None)]
        [Category("ActionScript")]
        [DisplayName("Wrap Array Declaration Mode")]
        public WrapType Pref_AS_WrapArrayDeclMode
		{
			get { return this.pref_AS_WrapArrayDeclMode; }
			set { this.pref_AS_WrapArrayDeclMode = value; }
		}

        [DefaultValue(WrapType.DontProcess)]
        [Category("ActionScript")]
        [DisplayName("Wrap XML Mode")]
        public WrapType Pref_AS_WrapXMLMode
		{
			get { return this.pref_AS_WrapXMLMode; }
			set { this.pref_AS_WrapXMLMode = value; }
		}

        [DefaultValue(WrapIndent.Normal)]
        [Category("ActionScript")]
        [DisplayName("Wrap Indent Style")]
        public WrapIndent Pref_AS_WrapIndentStyle
		{
			get { return this.pref_AS_WrapIndentStyle; }
			set { this.pref_AS_WrapIndentStyle = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Collapse Spaces For Adjacent Parens")]
		public bool Pref_AS_CollapseSpacesForAdjacentParens
		{
			get { return this.pref_AS_CollapseSpacesForAdjacentParens; }
			set { this.pref_AS_CollapseSpacesForAdjacentParens = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("New Line After Bindable")]
		public bool Pref_AS_NewlineAfterBindable
		{
			get { return this.pref_AS_NewlineAfterBindable; }
			set { this.pref_AS_NewlineAfterBindable = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces After Label")]
		public int Pref_AS_SpacesAfterLabel
		{
			get { return this.pref_AS_SpacesAfterLabel; }
			set { this.pref_AS_SpacesAfterLabel = value; }
		}

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Trim Trailing Whitespace")]
		public bool Pref_AS_TrimTrailingWhitespace
		{
			get { return this.pref_AS_TrimTrailingWhitespace; }
			set { this.pref_AS_TrimTrailingWhitespace = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Empty Statements On New Line")]
		public bool Pref_AS_PutEmptyStatementsOnNewLine
		{
			get { return this.pref_AS_PutEmptyStatementsOnNewLine; }
			set { this.pref_AS_PutEmptyStatementsOnNewLine = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces Before Open Control Paren")]
		public int Pref_AS_SpacesBeforeOpenControlParen
		{
			get { return this.pref_AS_SpacesBeforeOpenControlParen; }
			set { this.pref_AS_SpacesBeforeOpenControlParen = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Always Generate Indent")]
		public bool Pref_AS_AlwaysGenerateIndent
		{
			get { return this.pref_AS_AlwaysGenerateIndent; }
			set { this.pref_AS_AlwaysGenerateIndent = value; }
		}

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Don't Indent Package Items")]
		public bool Pref_AS_DontIndentPackageItems
		{
			get { return this.pref_AS_DontIndentPackageItems; }
			set { this.pref_AS_DontIndentPackageItems = value; }
		}

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Leave Extra Whitespace Around Var Declarations")]
		public bool Pref_AS_LeaveExtraWhitespaceAroundVarDecls
		{
			get { return this.pref_AS_LeaveExtraWhitespaceAroundVarDecls; }
			set { this.pref_AS_LeaveExtraWhitespaceAroundVarDecls = value; }
		}

        [DefaultValue(BraceStyle.Inherit)]
        [Category("ActionScript")]
        [DisplayName("Brace Style")]
        public BraceStyle Pref_AS_BraceStyle
		{
			get { return this.pref_AS_BraceStyle; }
			set { this.pref_AS_BraceStyle = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Use Brace Style")]
		public bool Pref_AS_UseBraceStyle
		{
			get { return this.pref_AS_UseBraceStyle; }
			set { this.pref_AS_UseBraceStyle = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Spaces Inside Parens")]
		public int Pref_AS_SpacesInsideParens
		{
			get { return this.pref_AS_SpacesInsideParens; }
			set { this.pref_AS_SpacesInsideParens = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Use Global Spaces Inside Parens")]
		public bool Pref_AS_UseGlobalSpacesInsideParens
		{
			get { return this.pref_AS_UseGlobalSpacesInsideParens; }
			set { this.pref_AS_UseGlobalSpacesInsideParens = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Array Declaration Brackets")]
		public int Pref_AS_AdvancedSpacesInsideArrayDeclBrackets
		{
			get { return this.pref_AS_AdvancedSpacesInsideArrayDeclBrackets; }
			set { this.pref_AS_AdvancedSpacesInsideArrayDeclBrackets = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Array Reference Brackets")]
		public int Pref_AS_AdvancedSpacesInsideArrayRefBrackets
		{
			get { return this.pref_AS_AdvancedSpacesInsideArrayRefBrackets; }
			set { this.pref_AS_AdvancedSpacesInsideArrayRefBrackets = value; }
		}

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Literal Braces")]
		public int Pref_AS_AdvancedSpacesInsideLiteralBraces
		{
			get { return this.pref_AS_AdvancedSpacesInsideLiteralBraces; }
			set { this.pref_AS_AdvancedSpacesInsideLiteralBraces = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Parens")]
		public int Pref_AS_AdvancedSpacesInsideParens
		{
			get { return this.pref_AS_AdvancedSpacesInsideParens; }
			set { this.pref_AS_AdvancedSpacesInsideParens = value; }
		}

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Spaces Around Equals In Optional Parameters")]
		public bool Pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters
		{
			get { return this.pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters; }
			set { this.pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters = value; }
		}

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Spaces Around Equals In Optional Parameters")]
		public int Pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters
		{
			get { return this.pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters; }
			set { this.pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters = value; }
		}

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Indent Multiline Comments")]
        public bool Pref_AS_IndentMultilineComments
        {
            get { return this.pref_AS_IndentMultilineComments; }
            set { this.pref_AS_IndentMultilineComments = value; }
        }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines Before Import Block")]
        public int Pref_AS_BlankLinesBeforeImportBlock { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines At Function Start")]
        public int Pref_AS_BlankLinesAtFunctionStart { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Blank Lines At Function End")]
        public int Pref_AS_BlankLinesAtFunctionEnd { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("While On New Line")]
        public bool Pref_AS_WhileOnNewLine { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Spaces Around Equals In Metatags")]
        public int Pref_AS_Tweak_SpacesAroundEqualsInMetatags { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Use Spaces Around Equals In Metatags")]
        public bool Pref_AS_Tweak_UseSpacesAroundEqualsInMetatags { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces After Colons In Declarations")]
        public int Pref_AS_AdvancedSpacesAfterColonsInDeclarations { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Before Colons In Declarations")]
        public int Pref_AS_AdvancedSpacesBeforeColonsInDeclarations { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces After Colons In Function Types")]
        public int Pref_AS_AdvancedSpacesAfterColonsInFunctionTypes { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Before Colons In Function Types")]
        public int Pref_AS_AdvancedSpacesBeforeColonsInFunctionTypes { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Line Comment Wrapping")]
        public bool Pref_AS_UseLineCommentWrapping { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Multiline Comment Wrapping")]
        public bool Pref_AS_UseMLCommentWrapping { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Multiline Comment Reflow")]
        public bool Pref_AS_MLCommentReflow { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Doc Comment Reflow")]
        public bool Pref_AS_DocCommentReflow { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Multiline Comment Header On Separate Line")]
        public bool Pref_AS_MLCommentHeaderOnSeparateLine { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Multiline Comment Asterisk Mode")]
        public int Pref_AS_MLCommentAsteriskMode { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Doc Comment Wrapping")]
        public bool Pref_AS_UseDocCommentWrapping { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Doc Comment Hanging Indent Tabs")]
        public int Pref_AS_DocCommentHangingIndentTabs { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Doc Comment Keep Blank Lines")]
        public bool Pref_AS_DocCommentKeepBlankLines { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Multiline Comment Keep Blank Lines")]
        public bool Pref_AS_MLCommentKeepBlankLines { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Leave Single Line Functions")]
        public bool Pref_AS_LeaveSingleLineFunctions { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Unindent Expression Terminators")]
        public bool Pref_AS_UnindentExpressionTerminators { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("No New CRs Before Break")]
        public bool Pref_AS_NoNewCRsBeforeBreak { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("No New CRs Before Continue")]
        public bool Pref_AS_NoNewCRsBeforeContinue { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("No New CRs Before Return")]
        public bool Pref_AS_NoNewCRsBeforeReturn { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("No New CRs Before Throw")]
        public bool Pref_AS_NoNewCRsBeforeThrow { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("No New CRs Before Expression")]
        public bool Pref_AS_NoNewCRsBeforeExpression { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Keep Relative Indent In Doc Comments")]
        public bool Pref_AS_KeepRelativeIndentInDocComments { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Tabs In Hanging Indent")]
        public int Pref_AS_TabsInHangingIndent { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Parens In Other Places")]
        public int Pref_AS_AdvancedSpacesInsideParensInOtherPlaces { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Parens In Parameter Lists")]
        public int Pref_AS_AdvancedSpacesInsideParensInParameterLists { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Spaces Inside Parens In Argument Lists")]
        public int Pref_AS_AdvancedSpacesInsideParensInArgumentLists { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces Before Formal Parameters")]
        public int Pref_AS_SpacesBeforeFormalParameters { get; set; }

        [DefaultValue(1)]
        [Category("ActionScript")]
        [DisplayName("Spaces Before Arguments")]
        public int Pref_AS_SpacesBeforeArguments { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Gnu Brace Indent")]
        public bool Pref_AS_UseGnuBraceIndent { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Ensure Loops Have Braces")]
        public bool Pref_AS_EnsureLoopsHaveBraces { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Add Braces To Loops")]
        public int Pref_AS_AddBracesToLoops { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Ensure Switch Cases Have Braces")]
        public bool Pref_AS_EnsureSwitchCasesHaveBraces { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Add Braces To Cases")]
        public int Pref_AS_AddBracesToCases { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Ensure Conditionals Have Braces")]
        public bool Pref_AS_EnsureConditionalsHaveBraces { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Add Braces To Conditionals")]
        public int Pref_AS_AddBracesToConditionals { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Dont Indent Switch Cases")]
        public bool Pref_AS_DontIndentSwitchCases { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Align Decl Equals")]
        public bool Pref_AS_AlignDeclEquals { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Align Decl Mode")]
        public int Pref_AS_AlignDeclMode { get; set; }

        [DefaultValue(true)]
        [Category("ActionScript")]
        [DisplayName("Keep Spaces Before Line Comments")]
        public bool Pref_AS_KeepSpacesBeforeLineComments { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Align Line Comments At Column")]
        public int Pref_AS_AlignLineCommentsAtColumn { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Global CR Before Brace")]
        public bool Pref_AS_UseGlobalCRBeforeBrace { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced CR Before Brace Settings")]
        public int Pref_AS_AdvancedCRBeforeBraceSettings { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Newline Before Bindable Function")]
        public bool Pref_AS_NewlineBeforeBindableFunction { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Newline Before Bindable Property")]
        public bool Pref_AS_NewlineBeforeBindableProperty { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Break Lines Before Arithmetic")]
        public bool Pref_AS_BreakLinesBeforeArithmetic { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Break Lines Before Logical")]
        public bool Pref_AS_BreakLinesBeforeLogical { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Break Lines Before Assignment")]
        public bool Pref_AS_BreakLinesBeforeAssignment { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Use Advanced Wrapping")]
        public bool Pref_AS_UseAdvancedWrapping { get; set; }

        [DefaultValue(0)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping Elements")]
        public int Pref_AS_AdvancedWrappingElements { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping Enforce Max")]
        public bool Pref_AS_AdvancedWrappingEnforceMax { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping All Args")]
        public bool Pref_AS_AdvancedWrappingAllArgs { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping All Parms")]
        public bool Pref_AS_AdvancedWrappingAllParms { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping First Arg")]
        public bool Pref_AS_AdvancedWrappingFirstArg { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping First Parm")]
        public bool Pref_AS_AdvancedWrappingFirstParm { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping First Array Item")]
        public bool Pref_AS_AdvancedWrappingFirstArrayItem { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping First Object Item")]
        public bool Pref_AS_AdvancedWrappingFirstObjectItem { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping All Array Items")]
        public bool Pref_AS_AdvancedWrappingAllArrayItems { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping All Object Items")]
        public bool Pref_AS_AdvancedWrappingAllObjectItems { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping Align Array Items")]
        public bool Pref_AS_AdvancedWrappingAlignArrayItems { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping Align Object Items")]
        public bool Pref_AS_AdvancedWrappingAlignObjectItems { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping Grace Columns")]
        public int Pref_AS_AdvancedWrappingGraceColumns { get; set; }

        [DefaultValue(false)]
        [Category("ActionScript")]
        [DisplayName("Advanced Wrapping Preserve Phrases")]
        public bool Pref_AS_AdvancedWrappingPreservePhrases { get; set; }

        [Browsable(false)]
        public string Pref_AS_MetaTagsOnSameLineAsTargetFunction 
        {
            get { return pref_AS_MetaTagsOnSameLineAsTargetFunction; }
            set { pref_AS_MetaTagsOnSameLineAsTargetFunction = value; } 
        }
        
        [Browsable(false)]
        public string Pref_AS_MetaTagsOnSameLineAsTargetProperty
        {
            get { return pref_AS_MetaTagsOnSameLineAsTargetProperty; }
            set { pref_AS_MetaTagsOnSameLineAsTargetProperty = value; }
        }

        [Browsable(false)]
		public bool Pref_AS_DoAutoFormat
		{
			get { return this.pref_AS_DoAutoFormat; }
			set { this.pref_AS_DoAutoFormat = value; }
		}

        [Browsable(false)]
		public bool Pref_AS_AutoFormatStyle
		{
			get { return this.pref_AS_AutoFormatStyle; }
			set { this.pref_AS_AutoFormatStyle = value; }
		}
		
		////////////////// MXML ///////////////////////////////////////
        
        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Tabs")]
        public bool Pref_Flex2_UseTabs
        {
            get { return this.pref_Flex_UseTabs; }
            set { this.pref_Flex_UseTabs = value; }
        }

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Spaces Around Equals")]
		public int Pref_MXML_SpacesAroundEquals
		{
			get { return this.pref_MXML_SpacesAroundEquals; }
			set { this.pref_MXML_SpacesAroundEquals = value; }
		}

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Sort Extra Attributes")]
		public bool Pref_MXML_SortExtraAttrs
		{
			get { return this.pref_MXML_SortExtraAttrs; }
			set { this.pref_MXML_SortExtraAttrs = value; }
		}

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Add New Line After Last Attribute")]
		public bool Pref_MXML_AddNewlineAfterLastAttr
		{
			get { return this.pref_MXML_AddNewlineAfterLastAttr; }
			set { this.pref_MXML_AddNewlineAfterLastAttr = value; }
		}

        [DefaultValue("")]
        [Category("MXML")]
        [DisplayName("Sort Attribute Data")]
		public string Pref_MXML_SortAttrData
		{
			get { return this.pref_MXML_SortAttrData; }
			set { this.pref_MXML_SortAttrData = value; }
		}

        [DefaultValue(SortMode.UseData)]
        [Category("MXML")]
        [DisplayName("Sort Attribute Mode")]
        public SortMode Pref_MXML_SortAttrMode
		{
			get { return this.pref_MXML_SortAttrMode; }
			set { this.pref_MXML_SortAttrMode = value; }
		}

        [DefaultValue(200)]
        [Category("MXML")]
        [DisplayName("Max Line Length")]
		public int Pref_MXML_MaxLineLength
		{
			get { return this.pref_MXML_MaxLineLength; }
			set { this.pref_MXML_MaxLineLength = value; }
		}

        [DefaultValue(WrapMode.CountPerLine)]
        [Category("MXML")]
        [DisplayName("Attribute Wrap Mode")]
        public WrapMode Pref_MXML_AttrWrapMode
		{
			get { return this.pref_MXML_AttrWrapMode; }
			set { this.pref_MXML_AttrWrapMode = value; }
		}

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Attributes Per Line")]
		public int Pref_MXML_AttrsPerLine
		{
			get { return this.pref_MXML_AttrsPerLine; }
			set { this.pref_MXML_AttrsPerLine = value; }
		}

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Keep Blank Lines")]
		public bool Pref_MXML_KeepBlankLines
		{
			get { return this.pref_MXML_KeepBlankLines; }
			set { this.pref_MXML_KeepBlankLines = value; }
		}

        [DefaultValue(WrapIndent.WrapElement)]
        [Category("MXML")]
        [DisplayName("Wrap Indent Style")]
		public WrapIndent Pref_MXML_WrapIndentStyle
		{
			get { return this.pref_MXML_WrapIndentStyle; }
			set { this.pref_MXML_WrapIndentStyle = value; }
		}

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines Before Tags")]
		public int Pref_MXML_BlankLinesBeforeTags
		{
			get { return this.pref_MXML_BlankLinesBeforeTags; }
			set { this.pref_MXML_BlankLinesBeforeTags = value; }
		}

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Use Attributes To Keep On Same Line")]
		public bool Pref_MXML_UseAttrsToKeepOnSameLine
		{
			get { return this.pref_MXML_UseAttrsToKeepOnSameLine; }
			set { this.pref_MXML_UseAttrsToKeepOnSameLine = value; }
		}

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Attributes To Keep On Same Line")]
		public int Pref_MXML_AttrsToKeepOnSameLine
		{
			get { return this.pref_MXML_AttrsToKeepOnSameLine; }
			set { this.pref_MXML_AttrsToKeepOnSameLine = value; }
		}

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Spaces Before Empty Tag End")]
		public int Pref_MXML_SpacesBeforeEmptyTagEnd
		{
			get { return this.pref_MXML_SpacesBeforeEmptyTagEnd; }
			set { this.pref_MXML_SpacesBeforeEmptyTagEnd = value; }
		}

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Require CDATA For AS3 Formatting")]
		public bool Pref_MXML_RequireCDATAForASFormatting
		{
			get { return this.pref_MXML_RequireCDATAForASFormatting; }
			set { this.pref_MXML_RequireCDATAForASFormatting = value; }
		}

        [Category("MXML")]
        [DisplayName("Tags That Can Format")]
        public string Pref_MXML_TagsCanFormat
        {
            get { return this.pref_MXML_TagsCanFormat; }
            set { this.pref_MXML_TagsCanFormat = value; }
        }

        [Category("MXML")]
        [DisplayName("Tags That Cannot Format")]
        public string Pref_MXML_TagsCannotFormat
        {
            get { return this.pref_MXML_TagsCannotFormat; }
            set { this.pref_MXML_TagsCannotFormat = value; }
        }

        [Category("MXML")]
        [DisplayName("Tags With Blank Lines Before")]
        public string Pref_MXML_TagsWithBlankLinesBefore
        {
            get { return this.pref_MXML_TagsWithBlankLinesBefore; }
            set { this.pref_MXML_TagsWithBlankLinesBefore = value; }
        }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Indent Size")]
        public int Pref_Flex_IndentSize { get; set; }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Tab Size")]
        public int Pref_Flex_TabSize { get; set; }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Keep Relative Indent In Multiline Comments")]
        public bool Pref_MXML_KeepRelativeIndentInMultilineComments { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines Before Comments")]
        public int Pref_MXML_BlankLinesBeforeComments { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines After Specific Parent Tags")]
        public int Pref_MXML_BlankLinesAfterSpecificParentTags { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines Between Sibling Tags")]
        public int Pref_MXML_BlankLinesBetweenSiblingTags { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Line sAfter Parent Tags")]
        public int Pref_MXML_BlankLinesAfterParentTags { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines Before Closing Tags")]
        public int Pref_MXML_BlankLinesBeforeClosingTags { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Tabs In Hanging Indent")]
        public int Pref_MXML_TabsInHangingIndent { get; set; }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Spaces Inside Attribute Braces")]
        public bool Pref_MXML_UseSpacesInsideAttributeBraces { get; set; }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Formatting Of Bound Attributes")]
        public bool Pref_MXML_UseFormattingOfBoundAttributes { get; set; }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Spaces Inside Attribute Braces")]
        public int Pref_MXML_SpacesInsideAttributeBraces { get; set; }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Script CData Indent Tabs")]
        public int Pref_MXML_ScriptCDataIndentTabs { get; set; }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Script Indent Tabs")]
        public int Pref_MXML_ScriptIndentTabs { get; set; }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines At CData Start")]
        public int Pref_MXML_BlankLinesAtCDataStart { get; set; }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Keep Script CData On Same Line")]
        public bool Pref_MXML_KeepScriptCDataOnSameLine { get; set; }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Indent Tag Close")]
        public bool Pref_MXML_IndentTagClose { get; set; }

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Always Use Max Line Length")]
        public bool Pref_MXML_AlwaysUseMaxLineLength { get; set; }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Tags Do Not Format Inside")]
        public bool Pref_MXML_UseTagsDoNotFormatInside { get; set; }

        [Browsable(false)]
        public string Pref_MXML_ParentTagsWithBlankLinesAfter
        {
            get { return this.pref_MXML_ParentTagsWithBlankLinesAfter; }
            set { this.pref_MXML_ParentTagsWithBlankLinesAfter = value; }
        }

        [Browsable(false)]
        public string Pref_MXML_TagsDoNotFormatInside
        {
            get { return this.pref_MXML_TagsDoNotFormatInside; }
            set { this.pref_MXML_TagsDoNotFormatInside = value; }
        }

        [Browsable(false)]
		public string Pref_MXML_TagsWithASContent
		{
			get { return this.pref_MXML_TagsWithASContent; }
			set { this.pref_MXML_TagsWithASContent = value; }
		}

        [Browsable(false)]
        public string Pref_MXML_AttrGroups
        {
            get { return this.pref_MXML_AttrGroups; }
            set { this.pref_MXML_AttrGroups = value; }
        }

        [Browsable(false)]
		public bool Pref_MXML_AutoFormatStyle
		{
			get { return this.pref_MXML_AutoFormatStyle; }
			set { this.pref_MXML_AutoFormatStyle = value; }
		}

        [Browsable(false)]
		public bool Pref_MXML_DoAutoFormat
		{
			get { return this.pref_MXML_DoAutoFormat; }
			set { this.pref_MXML_DoAutoFormat = value; }
		}
		
		public void InitializeDefaultPreferences()
		{		
			Pref_MXML_SortAttrData = "";
			Pref_MXML_SortAttrMode = SortMode.UseData;
            Pref_MXML_AttrWrapMode = WrapMode.CountPerLine;	
            Pref_MXML_TagsCanFormat = "mx:List,fx:List";
			Pref_MXML_TagsCannotFormat = "mx:String,fx:String";
			List<String> eventAttrs = GetEvents();
			StringBuilder asTags = new StringBuilder();
            foreach (String tag in eventAttrs) 
            {
				asTags.Append(".*:");
				asTags.Append(tag);
				asTags.Append(',');
			}
			asTags.Append(".*:Script");
			Pref_MXML_TagsWithASContent = asTags.ToString();
			List<AttrGroup> defaultGroups = CreateDefaultGroups();
			StringBuilder buffer = new StringBuilder();
			foreach (AttrGroup attrGroup in defaultGroups) 
            {
				buffer.Append(attrGroup.save());
				buffer.Append(LineSplitter);
			}
			Pref_MXML_AttrGroups = buffer.ToString();
		}
		
		private List<AttrGroup> CreateDefaultGroups()
		{
			List<AttrGroup> groups = new List<AttrGroup>();
			List<String> attrs = new List<String>();
			attrs.Add("allowDisjointSelection");
			attrs.Add("allowMultipleSelection");
			attrs.Add("allowThumbOverlap");
			attrs.Add("allowTrackClick");
			attrs.Add("autoLayout");
			attrs.Add("autoRepeat");
			attrs.Add("automationName");
			attrs.Add("cachePolicy");
			attrs.Add("class");
			attrs.Add("clipContent");
			attrs.Add("condenseWhite");
			attrs.Add("conversion");
			attrs.Add("creationIndex");
			attrs.Add("creationPolicy");
			attrs.Add("currentState");
			attrs.Add("data");
			attrs.Add("dataDescriptor");
			attrs.Add("dataProvider");
			attrs.Add("dataTipFormatFunction");
			attrs.Add("dayNames");
			attrs.Add("defaultButton");
			attrs.Add("direction");
			attrs.Add("disabledDays");
			attrs.Add("disabledRanges");
			attrs.Add("displayedMonth");
			attrs.Add("displayedYear");
			attrs.Add("doubleClickEnabled");
			attrs.Add("emphasized");
			attrs.Add("enabled");
			attrs.Add("explicitHeight");
			attrs.Add("explicitMaxHeight");
			attrs.Add("explicitMaxWidth");
			attrs.Add("explicitMinHeight");
			attrs.Add("explicitMinWidth");
			attrs.Add("explicitWidth");
			attrs.Add("firstDayOfWeek");
			attrs.Add("focusEnabled");
			attrs.Add("fontContext");
			attrs.Add("height");
			attrs.Add("horizontalLineScrollSize");
			attrs.Add("horizontalPageScrollSize");
			attrs.Add("horizontalScrollBar");
			attrs.Add("horizontalScrollPolicy");
			attrs.Add("horizontalScrollPosition");
			attrs.Add("htmlText");
			attrs.Add("icon");
			attrs.Add("iconField");
			attrs.Add("id");
			attrs.Add("imeMode");
			attrs.Add("includeInLayout");
			attrs.Add("indeterminate");
			attrs.Add("label");
			attrs.Add("labelField");
			attrs.Add("labelFunction");
			attrs.Add("labelPlacement");
			attrs.Add("labels");
			attrs.Add("layout");
			attrs.Add("lineScrollSize");
			attrs.Add("listData");
			attrs.Add("liveDragging");
			attrs.Add("maxChars");
			attrs.Add("maxHeight");
			attrs.Add("maxScrollPosition");
			attrs.Add("maxWidth");
			attrs.Add("maxYear");
			attrs.Add("maximum");
			attrs.Add("measuredHeight");
			attrs.Add("measuredMinHeight");
			attrs.Add("measuredMinWidth");
			attrs.Add("measuredWidth");
			attrs.Add("menuBarItemRenderer");
			attrs.Add("menuBarItems");
			attrs.Add("menus");
			attrs.Add("minHeight");
			attrs.Add("minScrollPosition");
			attrs.Add("minWidth");
			attrs.Add("minYear");
			attrs.Add("minimum");
			attrs.Add("mode");
			attrs.Add("monthNames");
			attrs.Add("monthSymbol");
			attrs.Add("mouseFocusEnabled");
			attrs.Add("pageScrollSize");
			attrs.Add("pageSize");
			attrs.Add("percentHeight");
			attrs.Add("percentWidth");
			attrs.Add("scaleX");
			attrs.Add("scaleY");
			attrs.Add("scrollPosition");
			attrs.Add("selectable");
			attrs.Add("selectableRange");
			attrs.Add("selected");
			attrs.Add("selectedDate");
			attrs.Add("selectedField");
			attrs.Add("selectedIndex");
			attrs.Add("selectedRanges");
			attrs.Add("showDataTip");
			attrs.Add("showRoot");
			attrs.Add("showToday");
			attrs.Add("sliderDataTipClass");
			attrs.Add("sliderThumbClass");
			attrs.Add("snapInterval");
			attrs.Add("source");
			attrs.Add("states");
			attrs.Add("stepSize");
			attrs.Add("stickyHighlighting");
			attrs.Add("styleName");
			attrs.Add("text");
			attrs.Add("text");
			attrs.Add("thumbCount");
			attrs.Add("tickInterval");
			attrs.Add("tickValues");
			attrs.Add("toggle");
			attrs.Add("toolTip");
			attrs.Add("transitions");
			attrs.Add("truncateToFit");
			attrs.Add("validationSubField");
			attrs.Add("value");
			attrs.Add("value");
			attrs.Add("verticalLineScrollSize");
			attrs.Add("verticalPageScrollSize");
			attrs.Add("verticalScrollBar");
			attrs.Add("verticalScrollPolicy");
			attrs.Add("verticalScrollPosition");
			attrs.Add("width");
			attrs.Add("x");
			attrs.Add("y");
			attrs.Add("yearNavigationEnabled");
			attrs.Add("yearSymbol");
			groups.Add(new AttrGroup("properties", attrs, MXMLPrettyPrinter.MXML_Sort_AscByCase, MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT, true));
			attrs = GetEvents();
			groups.Add(new AttrGroup("events", attrs, MXMLPrettyPrinter.MXML_Sort_AscByCase, MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT, true));
			attrs = new List<String>();
			attrs.Add("backgroundAlpha");
			attrs.Add("backgroundAttachment");
			attrs.Add("backgroundColor");
			attrs.Add("backgroundDisabledColor");
			attrs.Add("backgroundImage");
			attrs.Add("backgroundSize");
			attrs.Add("backgroundSkin");
			attrs.Add("barColor");
			attrs.Add("barSkin");
			attrs.Add("borderColor");
			attrs.Add("borderSides");
			attrs.Add("borderSkin");
			attrs.Add("borderStyle");
			attrs.Add("borderThickness");
			attrs.Add("bottom");
			attrs.Add("color");
			attrs.Add("cornerRadius");
			attrs.Add("dataTipOffset");
			attrs.Add("dataTipPrecision");
			attrs.Add("dataTipStyleName");
			attrs.Add("disabledColor");
			attrs.Add("disabledIcon");
			attrs.Add("disabledIconColor");
			attrs.Add("disabledSkin");
			attrs.Add("disbledOverlayAlpha");
			attrs.Add("downArrowDisabledSkin");
			attrs.Add("downArrowDownSkin");
			attrs.Add("downArrowOverSkin");
			attrs.Add("downArrowUpSkin");
			attrs.Add("downIcon");
			attrs.Add("downSkin");
			attrs.Add("dropShadowColor");
			attrs.Add("dropShadowEnabled");
			attrs.Add("errorColor");
			attrs.Add("fillAlphas");
			attrs.Add("fillColors");
			attrs.Add("focusAlpha");
			attrs.Add("focusBlendMode");
			attrs.Add("focusRoundedCorners");
			attrs.Add("focusSkin");
			attrs.Add("focusThickness");
			attrs.Add("fontAntiAliasType");
			attrs.Add("fontFamily");
			attrs.Add("fontGridFitType");
			attrs.Add("fontSharpness");
			attrs.Add("fontSize");
			attrs.Add("fontStyle");
			attrs.Add("fontThickness");
			attrs.Add("fontWeight");
			attrs.Add("fontfamily");
			attrs.Add("headerColors");
			attrs.Add("headerStyleName");
			attrs.Add("highlightAlphas");
			attrs.Add("horizontalAlign");
			attrs.Add("horizontalCenter");
			attrs.Add("horizontalGap");
			attrs.Add("horizontalScrollBarStyleName");
			attrs.Add("icon");
			attrs.Add("iconColor");
			attrs.Add("indeterminateMoveInterval");
			attrs.Add("indeterminateSkin");
			attrs.Add("itemDownSkin");
			attrs.Add("itemOverSkin");
			attrs.Add("itemUpSkin");
			attrs.Add("kerning");
			attrs.Add("labelOffset");
			attrs.Add("labelStyleName");
			attrs.Add("labelWidth");
			attrs.Add("leading");
			attrs.Add("left");
			attrs.Add("letterSpacing");
			attrs.Add("maskSkin");
			attrs.Add("menuStyleName");
			attrs.Add("nextMonthDisabledSkin");
			attrs.Add("nextMonthDownSkin");
			attrs.Add("nextMonthOverSkin");
			attrs.Add("nextMonthSkin");
			attrs.Add("nextMonthUpSkin");
			attrs.Add("nextYearDisabledSkin");
			attrs.Add("nextYearDownSkin");
			attrs.Add("nextYearOverSkin");
			attrs.Add("nextYearSkin");
			attrs.Add("nextYearUpSkin");
			attrs.Add("overIcon");
			attrs.Add("overSkin");
			attrs.Add("paddingBottom");
			attrs.Add("paddingLeft");
			attrs.Add("paddingRight");
			attrs.Add("paddingTop");
			attrs.Add("prevMonthDisabledSkin");
			attrs.Add("prevMonthDownSkin");
			attrs.Add("prevMonthOverSkin");
			attrs.Add("prevMonthSkin ");
			attrs.Add("prevMonthUpSkin");
			attrs.Add("prevYearDisabledSkin");
			attrs.Add("prevYearDownSkin");
			attrs.Add("prevYearOverSkin");
			attrs.Add("prevYearSkin ");
			attrs.Add("prevYearUpSkin");
			attrs.Add("repeatDelay");
			attrs.Add("repeatInterval");
			attrs.Add("right");
			attrs.Add("rollOverColor");
			attrs.Add("rollOverIndicatorSkin");
			attrs.Add("selectedDisabledIcon");
			attrs.Add("selectedDisabledSkin");
			attrs.Add("selectedDownIcon");
			attrs.Add("selectedDownSkin");
			attrs.Add("selectedOverIcon");
			attrs.Add("selectedOverSkin");
			attrs.Add("selectedUpIcon");
			attrs.Add("selectedUpSkin");
			attrs.Add("selectionColor");
			attrs.Add("selectionIndicatorSkin");
			attrs.Add("shadowColor");
			attrs.Add("shadowDirection");
			attrs.Add("shadowDistance");
			attrs.Add("showTrackHighlight");
			attrs.Add("skin");
			attrs.Add("slideDuration");
			attrs.Add("slideEasingFunction");
			attrs.Add("strokeColor");
			attrs.Add("strokeWidth");
			attrs.Add("textAlign");
			attrs.Add("textDecoration");
			attrs.Add("textIndent");
			attrs.Add("textRollOverColor");
			attrs.Add("textSelectedColor");
			attrs.Add("themeColor");
			attrs.Add("thumbDisabledSkin");
			attrs.Add("thumbDownSkin");
			attrs.Add("thumbIcon");
			attrs.Add("thumbOffset");
			attrs.Add("thumbOverSkin");
			attrs.Add("thumbUpSkin");
			attrs.Add("tickColor");
			attrs.Add("tickLength");
			attrs.Add("tickOffset");
			attrs.Add("tickThickness");
			attrs.Add("todayColor");
			attrs.Add("todayIndicatorSkin");
			attrs.Add("todayStyleName");
			attrs.Add("top");
			attrs.Add("tracHighlightSkin");
			attrs.Add("trackColors");
			attrs.Add("trackHeight");
			attrs.Add("trackMargin");
			attrs.Add("trackSkin");
			attrs.Add("upArrowDisabledSkin");
			attrs.Add("upArrowDownSkin");
			attrs.Add("upArrowOverSkin");
			attrs.Add("upArrowUpSkin");
			attrs.Add("upIcon");
			attrs.Add("upSkin");
			attrs.Add("verticalAlign");
			attrs.Add("verticalCenter");
			attrs.Add("verticalGap");
			attrs.Add("verticalScrollBarStyleName");
			attrs.Add("weekDayStyleName");
            groups.Add(new AttrGroup("styles", attrs, MXMLPrettyPrinter.MXML_Sort_AscByCase, MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT, true));
			attrs = new List<String>();
			attrs.Add("addedEffect");
			attrs.Add("completeEffect");
			attrs.Add("creationCompleteEffect");
			attrs.Add("focusInEffect");
			attrs.Add("focusOutEffect");
			attrs.Add("hideEffect");
			attrs.Add("mouseDownEffect");
			attrs.Add("mouseUpEffect");
			attrs.Add("moveEffect");
			attrs.Add("removedEffect");
			attrs.Add("resizeEffect");
			attrs.Add("rollOutEffect");
			attrs.Add("rollOverEffect");
			attrs.Add("showEffect");
            groups.Add(new AttrGroup("effects", attrs, MXMLPrettyPrinter.MXML_Sort_AscByCase, MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT, true));
			return groups;
		}

		public static List<String> GetEvents()
		{
			List<String> attrs = new List<String>();
			attrs.Add("add");
			attrs.Add("added");
			attrs.Add("activate");
			attrs.Add("addedToStage");
			attrs.Add("buttonDown");
			attrs.Add("change");
			attrs.Add("childAdd");
			attrs.Add("childIndexChange");
			attrs.Add("childRemove");
			attrs.Add("clickHandler");
			attrs.Add("clear");
			attrs.Add("click");
			attrs.Add("complete");
			attrs.Add("contextMenu");
			attrs.Add("copy");
			attrs.Add("creationComplete");
			attrs.Add("currentStateChange");
			attrs.Add("currentStateChanging");
			attrs.Add("cut");
			attrs.Add("dataChange");
			attrs.Add("deactivate");
			attrs.Add("doubleClick");
			attrs.Add("dragComplete");
			attrs.Add("dragDrop");
			attrs.Add("dragEnter");
			attrs.Add("dragExit");
			attrs.Add("dragOver");
			attrs.Add("dragStart");
			attrs.Add("effectEnd");
			attrs.Add("effectStart");
			attrs.Add("enterFrame");
			attrs.Add("enterState");
			attrs.Add("exitFrame");
			attrs.Add("exitState");
			attrs.Add("focusIn");
			attrs.Add("focusOut");
			attrs.Add("frameConstructed");
			attrs.Add("hide");
			attrs.Add("httpStatus");
			attrs.Add("init");
			attrs.Add("initialize");
			attrs.Add("invalid");
			attrs.Add("ioError");
			attrs.Add("itemClick");
			attrs.Add("itemRollOut");
			attrs.Add("itemRollOver");
			attrs.Add("keyDown");
			attrs.Add("keyFocusChange");
			attrs.Add("keyUp");
			attrs.Add("menuHide");
			attrs.Add("menuShow");
			attrs.Add("middleClick");
			attrs.Add("middleMouseDown");
			attrs.Add("middleMouseUp");
			attrs.Add("mouseDown");
			attrs.Add("mouseUp");
			attrs.Add("mouseOver");
			attrs.Add("mouseMove");
			attrs.Add("mouseOut");
			attrs.Add("mouseFocusChange");
			attrs.Add("mouseWheel");
			attrs.Add("mouseDownOutside");
			attrs.Add("mouseWheelOutside");
			attrs.Add("move");
			attrs.Add("nativeDragComplete");
			attrs.Add("nativeDragDrop");
			attrs.Add("nativeDragEnter");
			attrs.Add("nativeDragExit");
			attrs.Add("nativeDragOver");
			attrs.Add("nativeDragStart");
			attrs.Add("nativeDragUpdate");
			attrs.Add("open");
			attrs.Add("paste");
			attrs.Add("preinitialize");
			attrs.Add("progress");
			attrs.Add("record");
			attrs.Add("remove");
			attrs.Add("removed");
			attrs.Add("removedFromStage");
			attrs.Add("render");
			attrs.Add("resize");
			attrs.Add("rightClick");
			attrs.Add("rightMouseDown");
			attrs.Add("rightMouseUp");
			attrs.Add("rollOut");
			attrs.Add("rollOver");
			attrs.Add("scroll");
			attrs.Add("securityError");
			attrs.Add("selectAll");
			attrs.Add("show");
			attrs.Add("tabChildrenChange");
			attrs.Add("tabEnabledChange");
			attrs.Add("tabIndexChange");
			attrs.Add("thumbDrag");
			attrs.Add("thumbPress");
			attrs.Add("thumbRelease");
			attrs.Add("toolTipCreate");
			attrs.Add("toolTipEnd");
			attrs.Add("toolTipHide");
			attrs.Add("toolTipShow");
			attrs.Add("toolTipShown");
			attrs.Add("toolTipStart");
			attrs.Add("updateComplete");
			attrs.Add("unload");
			attrs.Add("valid");
			attrs.Add("valueCommit");
			return attrs;
		}
    }

    public enum SortMode
    {
        None = 0,
        UseData = 2
    }

    public enum WrapMode 
    {
		LineLength = 51,
        CountPerLine = 52,
        Default = 54,
        None = 53
    }

    public enum BraceStyle
    {
        OnLine = 4,
        AfterLine = 5,
        Inherit = 100
    }

    public enum WrapType
    {
		None = 1,
		DontProcess = 2,
		ByColumn = 8,
		ByTag = 128
    }

    public enum WrapIndent 
    {
        Normal = 1000,
		WrapElement = 1001
    }

}
