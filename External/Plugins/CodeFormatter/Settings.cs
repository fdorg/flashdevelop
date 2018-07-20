using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using CodeFormatter.Handlers;
using CodeFormatter.Preferences;
using PluginCore.Localization;

namespace CodeFormatter
{
    [Serializable]
    public class Settings
    {
        public const char LineSplitter = '\n';

        /////////////// AS3 /////////////////////////////////////

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
        private bool pref_AS_KeepSLCommentsOnColumn1 = false;
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
        public int pref_AS_BlankLinesBeforeImportBlock = 0;
        public int pref_AS_BlankLinesAtFunctionStart = 0;
        public int pref_AS_BlankLinesAtFunctionEnd = 0;
        public bool pref_AS_WhileOnNewLine = true;
        public int pref_AS_Tweak_SpacesAroundEqualsInMetatags = 1;
        public bool pref_AS_Tweak_UseSpacesAroundEqualsInMetatags = true;
        public int pref_AS_AdvancedSpacesAfterColonsInDeclarations = 1;
        public int pref_AS_AdvancedSpacesBeforeColonsInDeclarations = 1;
        public int pref_AS_AdvancedSpacesAfterColonsInFunctionTypes = 1;
        public int pref_AS_AdvancedSpacesBeforeColonsInFunctionTypes = 1;
        public bool pref_AS_UseLineCommentWrapping = false;
        public bool pref_AS_UseMLCommentWrapping = false;
        public bool pref_AS_MLCommentReflow = true;
        public bool pref_AS_DocCommentReflow = true;
        public bool pref_AS_MLCommentHeaderOnSeparateLine = true;
        public AsteriskMode pref_AS_MLCommentAsteriskMode = AsteriskMode.AsIs;
        public bool pref_AS_UseDocCommentWrapping = false;
        public int pref_AS_DocCommentHangingIndentTabs = 0;
        public bool pref_AS_DocCommentKeepBlankLines = true;
        public bool pref_AS_MLCommentKeepBlankLines = true;
        public bool pref_AS_LeaveSingleLineFunctions = true;
        public bool pref_AS_UnindentExpressionTerminators = false;
        public bool pref_AS_NoNewCRsBeforeBreak = true;
        public bool pref_AS_NoNewCRsBeforeContinue = true;
        public bool pref_AS_NoNewCRsBeforeReturn = true;
        public bool pref_AS_NoNewCRsBeforeThrow = true;
        public bool pref_AS_NoNewCRsBeforeExpression = true;
        public bool pref_AS_KeepRelativeIndentInDocComments = false;
        public int pref_AS_TabsInHangingIndent = 0;
        public int pref_AS_AdvancedSpacesInsideParensInOtherPlaces = 0;
        public int pref_AS_AdvancedSpacesInsideParensInParameterLists = 0;
        public int pref_AS_AdvancedSpacesInsideParensInArgumentLists = 0;
        public int pref_AS_SpacesBeforeFormalParameters = 0;
        public int pref_AS_SpacesBeforeArguments = 0;
        public bool pref_AS_UseGnuBraceIndent = false;
        public bool pref_AS_EnsureLoopsHaveBraces = false;
        public int pref_AS_AddBracesToLoops = 1;
        public bool pref_AS_EnsureSwitchCasesHaveBraces = false;
        public int pref_AS_AddBracesToCases = 1;
        public bool pref_AS_EnsureConditionalsHaveBraces = false;
        public int pref_AS_AddBracesToConditionals = 1;
        public bool pref_AS_DontIndentSwitchCases = true;
        public bool pref_AS_AlignDeclEquals = true;
        public DeclAlignMode pref_AS_AlignDeclMode = DeclAlignMode.Consecutive;
        public bool pref_AS_KeepSpacesBeforeLineComments = true;
        public int pref_AS_AlignLineCommentsAtColumn = 0;
        public bool pref_AS_UseGlobalCRBeforeBrace = false;
        public int pref_AS_AdvancedCRBeforeBraceSettings = 0;
        public bool pref_AS_NewlineBeforeBindableFunction = false;
        public bool pref_AS_NewlineBeforeBindableProperty = false;
        public bool pref_AS_BreakLinesBeforeArithmetic = false;
        public bool pref_AS_BreakLinesBeforeLogical = false;
        public bool pref_AS_BreakLinesBeforeAssignment = false;
        public bool pref_AS_UseAdvancedWrapping = false;
        public int pref_AS_AdvancedWrappingElements = 0;
        public bool pref_AS_AdvancedWrappingEnforceMax = false;
        public bool pref_AS_AdvancedWrappingAllArgs = false;
        public bool pref_AS_AdvancedWrappingAllParms = false;
        public bool pref_AS_AdvancedWrappingFirstArg = false;
        public bool pref_AS_AdvancedWrappingFirstParm = false;
        public bool pref_AS_AdvancedWrappingFirstArrayItem = false;
        public bool pref_AS_AdvancedWrappingFirstObjectItem = false;
        public bool pref_AS_AdvancedWrappingAllArrayItems = false;
        public bool pref_AS_AdvancedWrappingAllObjectItems = false;
        public bool pref_AS_AdvancedWrappingAlignArrayItems = false;
        public bool pref_AS_AdvancedWrappingAlignObjectItems = false;
        public int pref_AS_AdvancedWrappingGraceColumns = 1;
        public bool pref_AS_AdvancedWrappingPreservePhrases = false;
        //
        private string pref_AS_MetaTagsOnSameLineAsTargetFunction = "";
        private string pref_AS_MetaTagsOnSameLineAsTargetProperty = "";

        ////////////////// MXML ///////////////////////////////////////

        private int pref_MXML_SpacesAroundEquals = 0;
        private bool pref_MXML_SortExtraAttrs = false;
        private bool pref_MXML_AddNewlineAfterLastAttr = false;
        private SortMode pref_MXML_SortAttrMode = SortMode.UseData;
        private int pref_MXML_MaxLineLength = 200;
        private WrapMode pref_MXML_AttrWrapMode = WrapMode.CountPerLine;
        private int pref_MXML_AttrsPerLine = 1;
        private bool pref_MXML_KeepBlankLines = true;
        private WrapIndent pref_MXML_WrapIndentStyle = WrapIndent.WrapElement;
        private string pref_MXML_TagsWithBlankLinesBefore = "";
        private int pref_MXML_BlankLinesBeforeTags = 1;
        private bool pref_MXML_UseAttrsToKeepOnSameLine = true;
        private int pref_MXML_AttrsToKeepOnSameLine = 10;
        private int pref_MXML_SpacesBeforeEmptyTagEnd = 1;
        private bool pref_MXML_RequireCDATAForASFormatting = true;
        private bool pref_MXML_AutoFormatStyle = true;
        private bool pref_MXML_DoAutoFormat = true;
        public bool pref_MXML_KeepRelativeIndentInMultilineComments = false;
        public int pref_MXML_BlankLinesBeforeComments = 0;
        public int pref_MXML_BlankLinesAfterSpecificParentTags = 0;
        public int pref_MXML_BlankLinesBetweenSiblingTags = 0;
        public int pref_MXML_BlankLinesAfterParentTags = 0;
        public int pref_MXML_BlankLinesBeforeClosingTags = 0;
        public int pref_MXML_TabsInHangingIndent = 1;
        public bool pref_MXML_UseSpacesInsideAttributeBraces = true;
        public bool pref_MXML_UseFormattingOfBoundAttributes = true;
        public int pref_MXML_SpacesInsideAttributeBraces = 4;
        public int pref_MXML_ScriptCDataIndentTabs = 4;
        public int pref_MXML_ScriptIndentTabs = 4;
        public int pref_MXML_BlankLinesAtCDataStart = 1;
        public bool pref_MXML_KeepScriptCDataOnSameLine = true;
        public bool pref_MXML_IndentTagClose = true;
        public bool pref_MXML_AlwaysUseMaxLineLength = false;
        public bool pref_MXML_UseTagsDoNotFormatInside = true;
        //
        private string pref_MXML_TagsCanFormat = "mx:List,fx:List";
        private string pref_MXML_TagsCannotFormat = "mx:String,fx:String";
        private string pref_MXML_AttrGroups = "";
        private string pref_MXML_TagsWithASContent = "";
        private string pref_MXML_TagsDoNotFormatInside = ".*:Model,.*:XML";
        private string pref_MXML_ParentTagsWithBlankLinesAfter = "";
        private string pref_MXML_SortAttrData = "";

        ////////////////// Others ///////////////////////////////////////

        private string pref_AStyle_CPP = "";
        private string pref_AStyle_Others = "";

        [DefaultValue("")]
        [Category("Others")]
        [DisplayName("Manual CPP Options")]
        [LocalizedDescription("CodeFormatter.Description.AStyle.OptionsCPP")]
        public string Pref_AStyle_CPP
        {
            get { return this.pref_AStyle_CPP; }
            set { this.pref_AStyle_CPP = value; }
        }

        [DefaultValue("")]
        [Category("Others")]
        [DisplayName("Manual Options For Others")]
        [LocalizedDescription("CodeFormatter.Description.AStyle.OptionsOthers")]
        public string Pref_AStyle_Others
        {
            get { return this.pref_AStyle_Others; }
            set { this.pref_AStyle_Others = value; }
        }

        ////////////////// AS3 ///////////////////////////////////////

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Before Comma")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesBeforeComma")]
        public int Pref_AS_SpacesBeforeComma
        {
            get { return this.pref_AS_SpacesBeforeComma; }
            set { this.pref_AS_SpacesBeforeComma = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces After Comma")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAfterComma")]
        public int Pref_AS_SpacesAfterComma
        {
            get { return this.pref_AS_SpacesAfterComma; }
            set { this.pref_AS_SpacesAfterComma = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Around Colons")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAroundColons")]
        public int Pref_AS_SpacesAroundColons
        {
            get { return this.pref_AS_SpacesAroundColons; }
            set { this.pref_AS_SpacesAroundColons = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Use Global Spaces Around Colons")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseGlobalSpacesAroundColons")]
        public bool Pref_AS_UseGlobalSpacesAroundColons
        {
            get { return this.pref_AS_UseGlobalSpacesAroundColons; }
            set { this.pref_AS_UseGlobalSpacesAroundColons = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Before Colons")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedSpacesBeforeColons")]
        public int Pref_AS_AdvancedSpacesBeforeColons
        {
            get { return this.pref_AS_AdvancedSpacesBeforeColons; }
            set { this.pref_AS_AdvancedSpacesBeforeColons = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces After Colons")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedSpacesAfterColons")]
        public int Pref_AS_AdvancedSpacesAfterColons
        {
            get { return this.pref_AS_AdvancedSpacesAfterColons; }
            set { this.pref_AS_AdvancedSpacesAfterColons = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Blank Lines Before Functions")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesBeforeFunctions")]
        public int Pref_AS_BlankLinesBeforeFunctions
        {
            get { return this.pref_AS_BlankLinesBeforeFunctions; }
            set { this.pref_AS_BlankLinesBeforeFunctions = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Blank Lines Before Classes")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesBeforeClasses")]
        public int Pref_AS_BlankLinesBeforeClasses
        {
            get { return this.pref_AS_BlankLinesBeforeClasses; }
            set { this.pref_AS_BlankLinesBeforeClasses = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Blank Lines Before Properties")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesBeforeProperties")]
        public int Pref_AS_BlankLinesBeforeProperties
        {
            get { return this.pref_AS_BlankLinesBeforeProperties; }
            set { this.pref_AS_BlankLinesBeforeProperties = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Blank Lines Before Control Statements")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesBeforeControlStatements")]
        public int Pref_AS_BlankLinesBeforeControlStatements
        {
            get { return this.pref_AS_BlankLinesBeforeControlStatements; }
            set { this.pref_AS_BlankLinesBeforeControlStatements = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Keep Blank Lines")]
        [LocalizedDescription("CodeFormatter.Description.AS.KeepBlankLines")]
        public bool Pref_AS_KeepBlankLines
        {
            get { return this.pref_AS_KeepBlankLines; }
            set { this.pref_AS_KeepBlankLines = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Blank Lines To Keep")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesToKeep")]
        public int Pref_AS_BlankLinesToKeep
        {
            get { return this.pref_AS_BlankLinesToKeep; }
            set { this.pref_AS_BlankLinesToKeep = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Open Brace On New Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.OpenBraceOnNewLine")]
        public bool Pref_AS_OpenBraceOnNewLine
        {
            get { return this.pref_AS_OpenBraceOnNewLine; }
            set { this.pref_AS_OpenBraceOnNewLine = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Else On New Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.ElseOnNewLine")]
        public bool Pref_AS_ElseOnNewLine
        {
            get { return this.pref_AS_ElseOnNewLine; }
            set { this.pref_AS_ElseOnNewLine = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Catch On New Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.CatchOnNewLine")]
        public bool Pref_AS_CatchOnNewLine
        {
            get { return this.pref_AS_CatchOnNewLine; }
            set { this.pref_AS_CatchOnNewLine = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("ElseIf On Same Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.ElseIfOnSameLine")]
        public bool Pref_AS_ElseIfOnSameLine
        {
            get { return this.pref_AS_ElseIfOnSameLine; }
            set { this.pref_AS_ElseIfOnSameLine = value; }
        }

        [DefaultValue(200)]
        [Category("AS3")]
        [DisplayName("Max Line Length")]
        [LocalizedDescription("CodeFormatter.Description.AS.MaxLineLength")]
        public int Pref_AS_MaxLineLength
        {
            get { return this.pref_AS_MaxLineLength; }
            set { this.pref_AS_MaxLineLength = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Around Assignment")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAroundAssignment")]
        public int Pref_AS_SpacesAroundAssignment
        {
            get { return this.pref_AS_SpacesAroundAssignment; }
            set { this.pref_AS_SpacesAroundAssignment = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Around Symbolic Operator")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAroundSymbolicOperator")]
        public int Pref_AS_SpacesAroundSymbolicOperator
        {
            get { return this.pref_AS_SpacesAroundSymbolicOperator; }
            set { this.pref_AS_SpacesAroundSymbolicOperator = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Keep Single Line Comments On Column One")]
        [LocalizedDescription("CodeFormatter.Description.AS.KeepSingleLineCommentsOnColumnOne")]
        public bool Pref_AS_KeepSLCommentsOnColumn1
        {
            get { return this.pref_AS_KeepSLCommentsOnColumn1; }
            set { this.pref_AS_KeepSLCommentsOnColumn1 = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Break Lines Before Comma")]
        [LocalizedDescription("CodeFormatter.Description.AS.BreakLinesBeforeComma")]
        public bool Pref_AS_BreakLinesBeforeComma
        {
            get { return this.pref_AS_BreakLinesBeforeComma; }
            set { this.pref_AS_BreakLinesBeforeComma = value; }
        }

        [DefaultValue(WrapType.None)]
        [Category("AS3")]
        [DisplayName("Wrap Expression Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.WrapExpressionMode")]
        public WrapType Pref_AS_WrapExpressionMode
        {
            get { return this.pref_AS_WrapExpressionMode; }
            set { this.pref_AS_WrapExpressionMode = value; }
        }

        [DefaultValue(WrapType.None)]
        [Category("AS3")]
        [DisplayName("Wrap Method Declaration Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.WrapMethodDeclarationMode")]
        public WrapType Pref_AS_WrapMethodDeclMode
        {
            get { return this.pref_AS_WrapMethodDeclMode; }
            set { this.pref_AS_WrapMethodDeclMode = value; }
        }

        [DefaultValue(WrapType.None)]
        [Category("AS3")]
        [DisplayName("Wrap Method Call Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.WrapMethodCallMode")]
        public WrapType Pref_AS_WrapMethodCallMode
        {
            get { return this.pref_AS_WrapMethodCallMode; }
            set { this.pref_AS_WrapMethodCallMode = value; }
        }

        [DefaultValue(WrapType.None)]
        [Category("AS3")]
        [DisplayName("Wrap Array Declaration Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.WrapArrayDeclarationMode")]
        public WrapType Pref_AS_WrapArrayDeclMode
        {
            get { return this.pref_AS_WrapArrayDeclMode; }
            set { this.pref_AS_WrapArrayDeclMode = value; }
        }

        [DefaultValue(WrapType.DontProcess)]
        [Category("AS3")]
        [DisplayName("Wrap XML Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.WrapXMLMode")]
        public WrapType Pref_AS_WrapXMLMode
        {
            get { return this.pref_AS_WrapXMLMode; }
            set { this.pref_AS_WrapXMLMode = value; }
        }

        [DefaultValue(WrapIndent.Normal)]
        [Category("AS3")]
        [DisplayName("Wrap Indent Style")]
        [LocalizedDescription("CodeFormatter.Description.AS.WrapIndentStyle")]
        public WrapIndent Pref_AS_WrapIndentStyle
        {
            get { return this.pref_AS_WrapIndentStyle; }
            set { this.pref_AS_WrapIndentStyle = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Collapse Spaces For Adjacent Parens")]
        [LocalizedDescription("CodeFormatter.Description.AS.CollapseSpacesForAdjacentParens")]
        public bool Pref_AS_CollapseSpacesForAdjacentParens
        {
            get { return this.pref_AS_CollapseSpacesForAdjacentParens; }
            set { this.pref_AS_CollapseSpacesForAdjacentParens = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces After Label")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAfterLabel")]
        public int Pref_AS_SpacesAfterLabel
        {
            get { return this.pref_AS_SpacesAfterLabel; }
            set { this.pref_AS_SpacesAfterLabel = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Trim Trailing Whitespace")]
        [LocalizedDescription("CodeFormatter.Description.AS.TrimTrailingWhitespace")]
        public bool Pref_AS_TrimTrailingWhitespace
        {
            get { return this.pref_AS_TrimTrailingWhitespace; }
            set { this.pref_AS_TrimTrailingWhitespace = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Empty Statements On New Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.PutEmptyStatementsOnNewLine")]
        public bool Pref_AS_PutEmptyStatementsOnNewLine
        {
            get { return this.pref_AS_PutEmptyStatementsOnNewLine; }
            set { this.pref_AS_PutEmptyStatementsOnNewLine = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Before Open Control Paren")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesBeforeOpenControlParen")]
        public int Pref_AS_SpacesBeforeOpenControlParen
        {
            get { return this.pref_AS_SpacesBeforeOpenControlParen; }
            set { this.pref_AS_SpacesBeforeOpenControlParen = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Always Generate Indent")]
        [LocalizedDescription("CodeFormatter.Description.AS.AlwaysGenerateIndent")]
        public bool Pref_AS_AlwaysGenerateIndent
        {
            get { return this.pref_AS_AlwaysGenerateIndent; }
            set { this.pref_AS_AlwaysGenerateIndent = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Don't Indent Package Items")]
        [LocalizedDescription("CodeFormatter.Description.AS.DontIndentPackageItems")]
        public bool Pref_AS_DontIndentPackageItems
        {
            get { return this.pref_AS_DontIndentPackageItems; }
            set { this.pref_AS_DontIndentPackageItems = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Leave Extra Whitespace Around Var Declarations")]
        [LocalizedDescription("CodeFormatter.Description.AS.LeaveExtraWhitespaceAroundVarDeclarations")]
        public bool Pref_AS_LeaveExtraWhitespaceAroundVarDecls
        {
            get { return this.pref_AS_LeaveExtraWhitespaceAroundVarDecls; }
            set { this.pref_AS_LeaveExtraWhitespaceAroundVarDecls = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Parens")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesInsideParens")]
        public int Pref_AS_SpacesInsideParens
        {
            get { return this.pref_AS_SpacesInsideParens; }
            set { this.pref_AS_SpacesInsideParens = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Use Global Spaces Inside Parens")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseGlobalSpacesInsideParens")]
        public bool Pref_AS_UseGlobalSpacesInsideParens
        {
            get { return this.pref_AS_UseGlobalSpacesInsideParens; }
            set { this.pref_AS_UseGlobalSpacesInsideParens = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Array Declaration Brackets")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedSpacesInsideArrayDeclarationBrackets")]
        public int Pref_AS_AdvancedSpacesInsideArrayDeclBrackets
        {
            get { return this.pref_AS_AdvancedSpacesInsideArrayDeclBrackets; }
            set { this.pref_AS_AdvancedSpacesInsideArrayDeclBrackets = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Array Reference Brackets")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedSpacesInsideArrayReferenceBrackets")]
        public int Pref_AS_AdvancedSpacesInsideArrayRefBrackets
        {
            get { return this.pref_AS_AdvancedSpacesInsideArrayRefBrackets; }
            set { this.pref_AS_AdvancedSpacesInsideArrayRefBrackets = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Literal Braces")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedSpacesInsideLiteralBraces")]
        public int Pref_AS_AdvancedSpacesInsideLiteralBraces
        {
            get { return this.pref_AS_AdvancedSpacesInsideLiteralBraces; }
            set { this.pref_AS_AdvancedSpacesInsideLiteralBraces = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Parens")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedSpacesInsideParens")]
        public int Pref_AS_AdvancedSpacesInsideParens
        {
            get { return this.pref_AS_AdvancedSpacesInsideParens; }
            set { this.pref_AS_AdvancedSpacesInsideParens = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Spaces Around Equals In Optional Parameters")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseSpacesAroundEqualsInOptionalParameters")]
        public bool Pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters
        {
            get { return this.pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters; }
            set { this.pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Around Equals In Optional Parameters")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAroundEqualsInOptionalParameters")]
        public int Pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters
        {
            get { return this.pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters; }
            set { this.pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Indent Multiline Comments")]
        [LocalizedDescription("CodeFormatter.Description.AS.IndentMultilineComments")]
        public bool Pref_AS_IndentMultilineComments
        {
            get { return this.pref_AS_IndentMultilineComments; }
            set { this.pref_AS_IndentMultilineComments = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Blank Lines Before Import Block")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesBeforeImportBlock")]
        public int Pref_AS_BlankLinesBeforeImportBlock
        {
            get { return this.pref_AS_BlankLinesBeforeImportBlock; }
            set { this.pref_AS_BlankLinesBeforeImportBlock = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Blank Lines At Function Start")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesAtFunctionStart")]
        public int Pref_AS_BlankLinesAtFunctionStart
        {
            get { return this.pref_AS_BlankLinesAtFunctionStart; }
            set { this.pref_AS_BlankLinesAtFunctionStart = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Blank Lines At Function End")]
        [LocalizedDescription("CodeFormatter.Description.AS.BlankLinesAtFunctionEnd")]
        public int Pref_AS_BlankLinesAtFunctionEnd
        {
            get { return this.pref_AS_BlankLinesAtFunctionEnd; }
            set { this.pref_AS_BlankLinesAtFunctionEnd = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("While On New Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.WhileOnNewLine")]
        public bool Pref_AS_WhileOnNewLine
        {
            get { return this.pref_AS_WhileOnNewLine; }
            set { this.pref_AS_WhileOnNewLine = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Around Equals In Metatags")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAroundEqualsInMetatags")]
        public int Pref_AS_Tweak_SpacesAroundEqualsInMetatags
        {
            get { return this.pref_AS_Tweak_SpacesAroundEqualsInMetatags; }
            set { this.pref_AS_Tweak_SpacesAroundEqualsInMetatags = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Use Spaces Around Equals In Metatags")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseSpacesAroundEqualsInMetatags")]
        public bool Pref_AS_Tweak_UseSpacesAroundEqualsInMetatags
        {
            get { return this.pref_AS_Tweak_UseSpacesAroundEqualsInMetatags; }
            set { this.pref_AS_Tweak_UseSpacesAroundEqualsInMetatags = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces After Colons In Declarations")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAfterColonsInDeclarations")]
        public int Pref_AS_AdvancedSpacesAfterColonsInDeclarations
        {
            get { return this.pref_AS_AdvancedSpacesAfterColonsInDeclarations; }
            set { this.pref_AS_AdvancedSpacesAfterColonsInDeclarations = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Before Colons In Declarations")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesBeforeColonsInDeclarations")]
        public int Pref_AS_AdvancedSpacesBeforeColonsInDeclarations
        {
            get { return this.pref_AS_AdvancedSpacesBeforeColonsInDeclarations; }
            set { this.pref_AS_AdvancedSpacesBeforeColonsInDeclarations = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces After Colons In Function Types")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesAfterColonsInFunctionTypes")]
        public int Pref_AS_AdvancedSpacesAfterColonsInFunctionTypes
        {
            get { return this.pref_AS_AdvancedSpacesAfterColonsInFunctionTypes; }
            set { this.pref_AS_AdvancedSpacesAfterColonsInFunctionTypes = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Spaces Before Colons In Function Types")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesBeforeColonsInFunctionTypes")]
        public int Pref_AS_AdvancedSpacesBeforeColonsInFunctionTypes
        {
            get { return this.pref_AS_AdvancedSpacesBeforeColonsInFunctionTypes; }
            set { this.pref_AS_AdvancedSpacesBeforeColonsInFunctionTypes = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Line Comment Wrapping")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseLineCommentWrapping")]
        public bool Pref_AS_UseLineCommentWrapping
        {
            get { return this.pref_AS_UseLineCommentWrapping; }
            set { this.pref_AS_UseLineCommentWrapping = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Multiline Comment Wrapping")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseMLCommentWrapping")]
        public bool Pref_AS_UseMLCommentWrapping
        {
            get { return this.pref_AS_UseMLCommentWrapping; }
            set { this.pref_AS_UseMLCommentWrapping = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Multiline Comment Reflow")]
        [LocalizedDescription("CodeFormatter.Description.AS.MLCommentReflow")]
        public bool Pref_AS_MLCommentReflow
        {
            get { return this.pref_AS_MLCommentReflow; }
            set { this.pref_AS_MLCommentReflow = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Doc Comment Reflow")]
        [LocalizedDescription("CodeFormatter.Description.AS.DocCommentReflow")]
        public bool Pref_AS_DocCommentReflow
        {
            get { return this.pref_AS_DocCommentReflow; }
            set { this.pref_AS_DocCommentReflow = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Multiline Comment Header On Separate Line")]
        [LocalizedDescription("CodeFormatter.Description.AS.MLCommentHeaderOnSeparateLine")]
        public bool Pref_AS_MLCommentHeaderOnSeparateLine
        {
            get { return this.pref_AS_MLCommentHeaderOnSeparateLine; }
            set { this.pref_AS_MLCommentHeaderOnSeparateLine = value; }
        }

        [DefaultValue(AsteriskMode.AsIs)]
        [Category("AS3")]
        [DisplayName("Multiline Comment Asterisk Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.MLCommentAsteriskMode")]
        public AsteriskMode Pref_AS_MLCommentAsteriskMode
        {
            get { return this.pref_AS_MLCommentAsteriskMode; }
            set { this.pref_AS_MLCommentAsteriskMode = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Doc Comment Wrapping")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseDocCommentWrapping")]
        public bool Pref_AS_UseDocCommentWrapping
        {
            get { return this.pref_AS_UseDocCommentWrapping; }
            set { this.pref_AS_UseDocCommentWrapping = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Doc Comment Hanging Indent Tabs")]
        [LocalizedDescription("CodeFormatter.Description.AS.DocCommentHangingIndentTabs")]
        public int Pref_AS_DocCommentHangingIndentTabs
        {
            get { return this.pref_AS_DocCommentHangingIndentTabs; }
            set { this.pref_AS_DocCommentHangingIndentTabs = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Doc Comment Keep Blank Lines")]
        [LocalizedDescription("CodeFormatter.Description.AS.DocCommentKeepBlankLines")]
        public bool Pref_AS_DocCommentKeepBlankLines
        {
            get { return this.pref_AS_DocCommentKeepBlankLines; }
            set { this.pref_AS_DocCommentKeepBlankLines = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Multiline Comment Keep Blank Lines")]
        [LocalizedDescription("CodeFormatter.Description.AS.MLCommentKeepBlankLines")]
        public bool Pref_AS_MLCommentKeepBlankLines
        {
            get { return this.pref_AS_MLCommentKeepBlankLines; }
            set { this.pref_AS_MLCommentKeepBlankLines = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Keep Single Line Functions")]
        [LocalizedDescription("CodeFormatter.Description.AS.LeaveSingleLineFunctions")]
        public bool Pref_AS_LeaveSingleLineFunctions
        {
            get { return this.pref_AS_LeaveSingleLineFunctions; }
            set { this.pref_AS_LeaveSingleLineFunctions = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Unindent Expression Terminators")]
        [LocalizedDescription("CodeFormatter.Description.AS.UnindentExpressionTerminators")]
        public bool Pref_AS_UnindentExpressionTerminators
        {
            get { return this.pref_AS_UnindentExpressionTerminators; }
            set { this.pref_AS_UnindentExpressionTerminators = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("No New Newlines Before Break")]
        [LocalizedDescription("CodeFormatter.Description.AS.NoNewCRsBeforeBreak")]
        public bool Pref_AS_NoNewCRsBeforeBreak
        {
            get { return this.pref_AS_NoNewCRsBeforeBreak; }
            set { this.pref_AS_NoNewCRsBeforeBreak = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("No New Newlines Before Continue")]
        [LocalizedDescription("CodeFormatter.Description.AS.NoNewCRsBeforeContinue")]
        public bool Pref_AS_NoNewCRsBeforeContinue
        {
            get { return this.pref_AS_NoNewCRsBeforeContinue; }
            set { this.pref_AS_NoNewCRsBeforeContinue = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("No New Newlines Before Return")]
        [LocalizedDescription("CodeFormatter.Description.AS.NoNewCRsBeforeReturn")]
        public bool Pref_AS_NoNewCRsBeforeReturn
        {
            get { return this.pref_AS_NoNewCRsBeforeReturn; }
            set { this.pref_AS_NoNewCRsBeforeReturn = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("No New Newlines Before Throw")]
        [LocalizedDescription("CodeFormatter.Description.AS.NoNewCRsBeforeThrow")]
        public bool Pref_AS_NoNewCRsBeforeThrow
        {
            get { return this.pref_AS_NoNewCRsBeforeThrow; }
            set { this.pref_AS_NoNewCRsBeforeThrow = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("No New Newlines Before Expression")]
        [LocalizedDescription("CodeFormatter.Description.AS.NoNewCRsBeforeExpression")]
        public bool Pref_AS_NoNewCRsBeforeExpression
        {
            get { return this.pref_AS_NoNewCRsBeforeExpression; }
            set { this.pref_AS_NoNewCRsBeforeExpression = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Keep Relative Indent In Doc Comments")]
        [LocalizedDescription("CodeFormatter.Description.AS.KeepRelativeIndentInDocComments")]
        public bool Pref_AS_KeepRelativeIndentInDocComments
        {
            get { return this.pref_AS_KeepRelativeIndentInDocComments; }
            set { this.pref_AS_KeepRelativeIndentInDocComments = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Tabs In Hanging Indent")]
        [LocalizedDescription("CodeFormatter.Description.AS.TabsInHangingIndent")]
        public int Pref_AS_TabsInHangingIndent
        {
            get { return this.pref_AS_TabsInHangingIndent; }
            set { this.pref_AS_TabsInHangingIndent = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Parens In Other Places")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesInsideParensInOtherPlaces")]
        public int Pref_AS_AdvancedSpacesInsideParensInOtherPlaces
        {
            get { return this.pref_AS_AdvancedSpacesInsideParensInOtherPlaces; }
            set { this.pref_AS_AdvancedSpacesInsideParensInOtherPlaces = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Parens In Parameter Lists")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesInsideParensInParameterLists")]
        public int Pref_AS_AdvancedSpacesInsideParensInParameterLists
        {
            get { return this.pref_AS_AdvancedSpacesInsideParensInParameterLists; }
            set { this.pref_AS_AdvancedSpacesInsideParensInParameterLists = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Inside Parens In Argument Lists")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesInsideParensInArgumentLists")]
        public int Pref_AS_AdvancedSpacesInsideParensInArgumentLists
        {
            get { return this.pref_AS_AdvancedSpacesInsideParensInArgumentLists; }
            set { this.pref_AS_AdvancedSpacesInsideParensInArgumentLists = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Before Formal Parameters")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesBeforeFormalParameters")]
        public int Pref_AS_SpacesBeforeFormalParameters
        {
            get { return this.pref_AS_SpacesBeforeFormalParameters; }
            set { this.pref_AS_SpacesBeforeFormalParameters = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Spaces Before Arguments")]
        [LocalizedDescription("CodeFormatter.Description.AS.SpacesBeforeArguments")]
        public int Pref_AS_SpacesBeforeArguments
        {
            get { return this.pref_AS_SpacesBeforeArguments; }
            set { this.pref_AS_SpacesBeforeArguments = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Gnu Brace Indent")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseGnuBraceIndent")]
        public bool Pref_AS_UseGnuBraceIndent
        {
            get { return this.pref_AS_UseGnuBraceIndent; }
            set { this.pref_AS_UseGnuBraceIndent = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Ensure Loops Have Braces")]
        [LocalizedDescription("CodeFormatter.Description.AS.EnsureLoopsHaveBraces")]
        public bool Pref_AS_EnsureLoopsHaveBraces
        {
            get { return this.pref_AS_EnsureLoopsHaveBraces; }
            set { this.pref_AS_EnsureLoopsHaveBraces = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Add Braces To Loops")]
        [LocalizedDescription("CodeFormatter.Description.AS.AddBracesToLoops")]
        public int Pref_AS_AddBracesToLoops
        {
            get { return this.pref_AS_AddBracesToLoops; }
            set { this.pref_AS_AddBracesToLoops = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Ensure Switch Cases Have Braces")]
        [LocalizedDescription("CodeFormatter.Description.AS.EnsureSwitchCasesHaveBraces")]
        public bool Pref_AS_EnsureSwitchCasesHaveBraces
        {
            get { return this.pref_AS_EnsureSwitchCasesHaveBraces; }
            set { this.pref_AS_EnsureSwitchCasesHaveBraces = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Add Braces To Cases")]
        [LocalizedDescription("CodeFormatter.Description.AS.AddBracesToCases")]
        public int Pref_AS_AddBracesToCases
        {
            get { return this.pref_AS_AddBracesToCases; }
            set { this.pref_AS_AddBracesToCases = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Ensure Conditionals Have Braces")]
        [LocalizedDescription("CodeFormatter.Description.AS.EnsureConditionalsHaveBraces")]
        public bool Pref_AS_EnsureConditionalsHaveBraces
        {
            get { return this.pref_AS_EnsureConditionalsHaveBraces; }
            set { this.pref_AS_EnsureConditionalsHaveBraces = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Add Braces To Conditionals")]
        [LocalizedDescription("CodeFormatter.Description.AS.AddBracesToConditionals")]
        public int Pref_AS_AddBracesToConditionals
        {
            get { return this.pref_AS_AddBracesToConditionals; }
            set { this.pref_AS_AddBracesToConditionals = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Don't Indent Switch Cases")]
        [LocalizedDescription("CodeFormatter.Description.AS.DontIndentSwitchCases")]
        public bool Pref_AS_DontIndentSwitchCases
        {
            get { return this.pref_AS_DontIndentSwitchCases; }
            set { this.pref_AS_DontIndentSwitchCases = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Align Declaration Equals")]
        [LocalizedDescription("CodeFormatter.Description.AS.AlignDeclEquals")]
        public bool Pref_AS_AlignDeclEquals
        {
            get { return this.pref_AS_AlignDeclEquals; }
            set { this.pref_AS_AlignDeclEquals = value; }
        }

        [DefaultValue(DeclAlignMode.Consecutive)]
        [Category("AS3")]
        [DisplayName("Align Declaration Mode")]
        [LocalizedDescription("CodeFormatter.Description.AS.AlignDeclMode")]
        public DeclAlignMode Pref_AS_AlignDeclMode
        {
            get { return this.pref_AS_AlignDeclMode; }
            set { this.pref_AS_AlignDeclMode = value; }
        }

        [DefaultValue(true)]
        [Category("AS3")]
        [DisplayName("Keep Spaces Before Line Comments")]
        [LocalizedDescription("CodeFormatter.Description.AS.KeepSpacesBeforeLineComments")]
        public bool Pref_AS_KeepSpacesBeforeLineComments
        {
            get { return this.pref_AS_KeepSpacesBeforeLineComments; }
            set { this.pref_AS_KeepSpacesBeforeLineComments = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Align Line Comments At Column")]
        [LocalizedDescription("CodeFormatter.Description.AS.AlignLineCommentsAtColumn")]
        public int Pref_AS_AlignLineCommentsAtColumn
        {
            get { return this.pref_AS_AlignLineCommentsAtColumn; }
            set { this.pref_AS_AlignLineCommentsAtColumn = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Global Newline Before Brace")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseGlobalCRBeforeBrace")]
        public bool Pref_AS_UseGlobalCRBeforeBrace
        {
            get { return this.pref_AS_UseGlobalCRBeforeBrace; }
            set { this.pref_AS_UseGlobalCRBeforeBrace = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Newline Before Brace Settings")]
        [LocalizedDescription("CodeFormatter.Description.AS.CRBeforeBraceSettings")]
        public int Pref_AS_AdvancedCRBeforeBraceSettings
        {
            get { return this.pref_AS_AdvancedCRBeforeBraceSettings; }
            set { this.pref_AS_AdvancedCRBeforeBraceSettings = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Newline Before Bindable Function")]
        [LocalizedDescription("CodeFormatter.Description.AS.NewlineBeforeBindableFunction")]
        public bool Pref_AS_NewlineBeforeBindableFunction
        {
            get { return this.pref_AS_NewlineBeforeBindableFunction; }
            set { this.pref_AS_NewlineBeforeBindableFunction = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Newline Before Bindable Property")]
        [LocalizedDescription("CodeFormatter.Description.AS.NewlineBeforeBindableProperty")]
        public bool Pref_AS_NewlineBeforeBindableProperty
        {
            get { return this.pref_AS_NewlineBeforeBindableProperty; }
            set { this.pref_AS_NewlineBeforeBindableProperty = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Break Lines Before Arithmetic")]
        [LocalizedDescription("CodeFormatter.Description.AS.BreakLinesBeforeArithmetic")]
        public bool Pref_AS_BreakLinesBeforeArithmetic
        {
            get { return this.pref_AS_BreakLinesBeforeArithmetic; }
            set { this.pref_AS_BreakLinesBeforeArithmetic = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Break Lines Before Logical")]
        [LocalizedDescription("CodeFormatter.Description.AS.BreakLinesBeforeLogical")]
        public bool Pref_AS_BreakLinesBeforeLogical
        {
            get { return this.pref_AS_BreakLinesBeforeLogical; }
            set { this.pref_AS_BreakLinesBeforeLogical = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Break Lines Before Assignment")]
        [LocalizedDescription("CodeFormatter.Description.AS.BreakLinesBeforeAssignment")]
        public bool Pref_AS_BreakLinesBeforeAssignment
        {
            get { return this.pref_AS_BreakLinesBeforeAssignment; }
            set { this.pref_AS_BreakLinesBeforeAssignment = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Use Advanced Wrapping")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseAdvancedWrapping")]
        public bool Pref_AS_UseAdvancedWrapping
        {
            get { return this.pref_AS_UseAdvancedWrapping; }
            set { this.pref_AS_UseAdvancedWrapping = value; }
        }

        [DefaultValue(0)]
        [Category("AS3")]
        [DisplayName("Wrap Elements")]
        [LocalizedDescription("CodeFormatter.Description.AS.UseAdvancedWrapping")]
        public int Pref_AS_AdvancedWrappingElements
        {
            get { return this.pref_AS_AdvancedWrappingElements; }
            set { this.pref_AS_AdvancedWrappingElements = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Enforce Maximum Wrapping")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingEnforceMax")]
        public bool Pref_AS_AdvancedWrappingEnforceMax
        {
            get { return this.pref_AS_AdvancedWrappingEnforceMax; }
            set { this.pref_AS_AdvancedWrappingEnforceMax = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap All Arguments")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingAllArgs")]
        public bool Pref_AS_AdvancedWrappingAllArgs
        {
            get { return this.pref_AS_AdvancedWrappingAllArgs; }
            set { this.pref_AS_AdvancedWrappingAllArgs = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap All Parameters")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingAllParms")]
        public bool Pref_AS_AdvancedWrappingAllParms
        {
            get { return this.pref_AS_AdvancedWrappingAllParms; }
            set { this.pref_AS_AdvancedWrappingAllParms = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap First Argument")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingFirstArg")]
        public bool Pref_AS_AdvancedWrappingFirstArg
        {
            get { return this.pref_AS_AdvancedWrappingFirstArg; }
            set { this.pref_AS_AdvancedWrappingFirstArg = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap First Parameter")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingFirstParm")]
        public bool Pref_AS_AdvancedWrappingFirstParm
        {
            get { return this.pref_AS_AdvancedWrappingFirstParm; }
            set { this.pref_AS_AdvancedWrappingFirstParm = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("WrapFirst Array Item")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingFirstArrayItem")]
        public bool Pref_AS_AdvancedWrappingFirstArrayItem
        {
            get { return this.pref_AS_AdvancedWrappingFirstArrayItem; }
            set { this.pref_AS_AdvancedWrappingFirstArrayItem = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap First Object Item")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingFirstObjectItem")]
        public bool Pref_AS_AdvancedWrappingFirstObjectItem
        {
            get { return this.pref_AS_AdvancedWrappingFirstObjectItem; }
            set { this.pref_AS_AdvancedWrappingFirstObjectItem = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap All Array Items")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingAllArrayItems")]
        public bool Pref_AS_AdvancedWrappingAllArrayItems
        {
            get { return this.pref_AS_AdvancedWrappingAllArrayItems; }
            set { this.pref_AS_AdvancedWrappingAllArrayItems = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Wrap All Object Items")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingAllObjectItems")]
        public bool Pref_AS_AdvancedWrappingAllObjectItems
        {
            get { return this.pref_AS_AdvancedWrappingAllObjectItems; }
            set { this.pref_AS_AdvancedWrappingAllObjectItems = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Align Array Items With Wrap")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingAlignArrayItems")]
        public bool Pref_AS_AdvancedWrappingAlignArrayItems
        {
            get { return this.pref_AS_AdvancedWrappingAlignArrayItems; }
            set { this.pref_AS_AdvancedWrappingAlignArrayItems = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Align Object Items With Wrap")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingAlignObjectItems")]
        public bool Pref_AS_AdvancedWrappingAlignObjectItems
        {
            get { return this.pref_AS_AdvancedWrappingAlignObjectItems; }
            set { this.pref_AS_AdvancedWrappingAlignObjectItems = value; }
        }

        [DefaultValue(1)]
        [Category("AS3")]
        [DisplayName("Grace Columns With Wrap")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingGraceColumns")]
        public int Pref_AS_AdvancedWrappingGraceColumns
        {
            get { return this.pref_AS_AdvancedWrappingGraceColumns; }
            set { this.pref_AS_AdvancedWrappingGraceColumns = value; }
        }

        [DefaultValue(false)]
        [Category("AS3")]
        [DisplayName("Preserve Phrases With Wrap")]
        [LocalizedDescription("CodeFormatter.Description.AS.AdvancedWrappingPreservePhrases")]
        public bool Pref_AS_AdvancedWrappingPreservePhrases
        {
            get { return this.pref_AS_AdvancedWrappingPreservePhrases; }
            set { this.pref_AS_AdvancedWrappingPreservePhrases = value; }
        }

        [DefaultValue("")]
        [Category("AS3")]
        [DisplayName("Metatags To Keep On Same Line As Target Function")]
        [LocalizedDescription("CodeFormatter.Description.AS.MetaTagsOnSameLineAsTargetFunction")]
        public string Pref_AS_MetaTagsOnSameLineAsTargetFunction
        {
            get { return this.pref_AS_MetaTagsOnSameLineAsTargetFunction; }
            set { this.pref_AS_MetaTagsOnSameLineAsTargetFunction = value; }
        }

        [DefaultValue("")]
        [Category("AS3")]
        [DisplayName("Metatags To Keep On Same Line As Target Property")]
        [LocalizedDescription("CodeFormatter.Description.AS.MetaTagsOnSameLineAsTargetProperty")]
        public string Pref_AS_MetaTagsOnSameLineAsTargetProperty
        {
            get { return this.pref_AS_MetaTagsOnSameLineAsTargetProperty; }
            set { this.pref_AS_MetaTagsOnSameLineAsTargetProperty = value; }
        }

        [Browsable(false)]
        public bool Pref_AS_NewlineAfterBindable
        {
            get { return this.pref_AS_NewlineAfterBindable; }
            set { this.pref_AS_NewlineAfterBindable = value; }
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

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Spaces Around Equals")]
        [LocalizedDescription("CodeFormatter.Description.MXML.SpacesAroundEquals")]
        public int Pref_MXML_SpacesAroundEquals
        {
            get { return this.pref_MXML_SpacesAroundEquals; }
            set { this.pref_MXML_SpacesAroundEquals = value; }
        }

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Sort Extra Attributes")]
        [LocalizedDescription("CodeFormatter.Description.MXML.SortExtraAttributes")]
        public bool Pref_MXML_SortExtraAttrs
        {
            get { return this.pref_MXML_SortExtraAttrs; }
            set { this.pref_MXML_SortExtraAttrs = value; }
        }

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Add New Line After Last Attribute")]
        [LocalizedDescription("CodeFormatter.Description.MXML.AddNewLineAfterLastAttribute")]
        public bool Pref_MXML_AddNewlineAfterLastAttr
        {
            get { return this.pref_MXML_AddNewlineAfterLastAttr; }
            set { this.pref_MXML_AddNewlineAfterLastAttr = value; }
        }

        [DefaultValue(SortMode.UseData)]
        [Category("MXML")]
        [DisplayName("Sort Attribute Mode")]
        [LocalizedDescription("CodeFormatter.Description.MXML.SortAttributeMode")]
        public SortMode Pref_MXML_SortAttrMode
        {
            get { return this.pref_MXML_SortAttrMode; }
            set { this.pref_MXML_SortAttrMode = value; }
        }

        [DefaultValue(200)]
        [Category("MXML")]
        [DisplayName("Max Line Length")]
        [LocalizedDescription("CodeFormatter.Description.MXML.MaxLineLength")]
        public int Pref_MXML_MaxLineLength
        {
            get { return this.pref_MXML_MaxLineLength; }
            set { this.pref_MXML_MaxLineLength = value; }
        }

        [DefaultValue(WrapMode.CountPerLine)]
        [Category("MXML")]
        [DisplayName("Attribute Wrap Mode")]
        [LocalizedDescription("CodeFormatter.Description.MXML.AttributeWrapMode")]
        public WrapMode Pref_MXML_AttrWrapMode
        {
            get { return this.pref_MXML_AttrWrapMode; }
            set { this.pref_MXML_AttrWrapMode = value; }
        }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Attributes Per Line")]
        [LocalizedDescription("CodeFormatter.Description.MXML.AttributesPerLine")]
        public int Pref_MXML_AttrsPerLine
        {
            get { return this.pref_MXML_AttrsPerLine; }
            set { this.pref_MXML_AttrsPerLine = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Keep Blank Lines")]
        [LocalizedDescription("CodeFormatter.Description.MXML.KeepBlankLines")]
        public bool Pref_MXML_KeepBlankLines
        {
            get { return this.pref_MXML_KeepBlankLines; }
            set { this.pref_MXML_KeepBlankLines = value; }
        }

        [DefaultValue(WrapIndent.WrapElement)]
        [Category("MXML")]
        [DisplayName("Wrap Indent Style")]
        [LocalizedDescription("CodeFormatter.Description.MXML.WrapIndentStyle")]
        public WrapIndent Pref_MXML_WrapIndentStyle
        {
            get { return this.pref_MXML_WrapIndentStyle; }
            set { this.pref_MXML_WrapIndentStyle = value; }
        }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines Before Tags")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesBeforeTags")]
        public int Pref_MXML_BlankLinesBeforeTags
        {
            get { return this.pref_MXML_BlankLinesBeforeTags; }
            set { this.pref_MXML_BlankLinesBeforeTags = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Attributes To Keep On Same Line")]
        [LocalizedDescription("CodeFormatter.Description.MXML.UseAttributesToKeepOnSameLine")]
        public bool Pref_MXML_UseAttrsToKeepOnSameLine
        {
            get { return this.pref_MXML_UseAttrsToKeepOnSameLine; }
            set { this.pref_MXML_UseAttrsToKeepOnSameLine = value; }
        }

        [DefaultValue(10)]
        [Category("MXML")]
        [DisplayName("Attributes To Keep On Same Line")]
        [LocalizedDescription("CodeFormatter.Description.MXML.AttributesToKeepOnSameLine")]
        public int Pref_MXML_AttrsToKeepOnSameLine
        {
            get { return this.pref_MXML_AttrsToKeepOnSameLine; }
            set { this.pref_MXML_AttrsToKeepOnSameLine = value; }
        }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Spaces Before Empty Tag End")]
        [LocalizedDescription("CodeFormatter.Description.MXML.SpacesBeforeEmptyTagEnd")]
        public int Pref_MXML_SpacesBeforeEmptyTagEnd
        {
            get { return this.pref_MXML_SpacesBeforeEmptyTagEnd; }
            set { this.pref_MXML_SpacesBeforeEmptyTagEnd = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Require CDATA For AS3 Formatting")]
        [LocalizedDescription("CodeFormatter.Description.MXML.RequireCDATAForAS3Formatting")]
        public bool Pref_MXML_RequireCDATAForASFormatting
        {
            get { return this.pref_MXML_RequireCDATAForASFormatting; }
            set { this.pref_MXML_RequireCDATAForASFormatting = value; }
        }

        [Category("MXML")]
        [DisplayName("Tags That Can Format")]
        [LocalizedDescription("CodeFormatter.Description.MXML.TagsThatCanFormat")]
        public string Pref_MXML_TagsCanFormat
        {
            get { return this.pref_MXML_TagsCanFormat; }
            set { this.pref_MXML_TagsCanFormat = value; }
        }

        [Category("MXML")]
        [DisplayName("Tags That Cannot Format")]
        [LocalizedDescription("CodeFormatter.Description.MXML.TagsThatCannotFormat")]
        public string Pref_MXML_TagsCannotFormat
        {
            get { return this.pref_MXML_TagsCannotFormat; }
            set { this.pref_MXML_TagsCannotFormat = value; }
        }

        [Category("MXML")]
        [DisplayName("Tags With Blank Lines Before")]
        [LocalizedDescription("CodeFormatter.Description.MXML.TagsWithBlankLinesBefore")]
        public string Pref_MXML_TagsWithBlankLinesBefore
        {
            get { return this.pref_MXML_TagsWithBlankLinesBefore; }
            set { this.pref_MXML_TagsWithBlankLinesBefore = value; }
        }

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Keep Relative Indent In Multiline Comments")]
        [LocalizedDescription("CodeFormatter.Description.MXML.KeepRelativeIndentInMultilineComments")]
        public bool Pref_MXML_KeepRelativeIndentInMultilineComments
        {
            get { return this.pref_MXML_KeepRelativeIndentInMultilineComments; }
            set { this.pref_MXML_KeepRelativeIndentInMultilineComments = value; }
        }

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Blank Lines Before Comments")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesBeforeComments")]
        public int Pref_MXML_BlankLinesBeforeComments
        {
            get { return this.pref_MXML_BlankLinesBeforeComments; }
            set { this.pref_MXML_BlankLinesBeforeComments = value; }
        }

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Blank Lines After Specific Parent Tags")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesAfterSpecificParentTags")]
        public int Pref_MXML_BlankLinesAfterSpecificParentTags
        {
            get { return this.pref_MXML_BlankLinesAfterSpecificParentTags; }
            set { this.pref_MXML_BlankLinesAfterSpecificParentTags = value; }
        }

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Blank Lines Between Sibling Tags")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesBetweenSiblingTags")]
        public int Pref_MXML_BlankLinesBetweenSiblingTags
        {
            get { return this.pref_MXML_BlankLinesBetweenSiblingTags; }
            set { this.pref_MXML_BlankLinesBetweenSiblingTags = value; }
        }

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Blank Lines After Parent Tags")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesAfterParentTags")]
        public int Pref_MXML_BlankLinesAfterParentTags
        {
            get { return this.pref_MXML_BlankLinesAfterParentTags; }
            set { this.pref_MXML_BlankLinesAfterParentTags = value; }
        }

        [DefaultValue(0)]
        [Category("MXML")]
        [DisplayName("Blank Lines Before Closing Tags")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesBeforeClosingTags")]
        public int Pref_MXML_BlankLinesBeforeClosingTags
        {
            get { return this.pref_MXML_BlankLinesBeforeClosingTags; }
            set { this.pref_MXML_BlankLinesBeforeClosingTags = value; }
        }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Tabs In Hanging Indent")]
        [LocalizedDescription("CodeFormatter.Description.MXML.TabsInHangingIndent")]
        public int Pref_MXML_TabsInHangingIndent
        {
            get { return this.pref_MXML_TabsInHangingIndent; }
            set { this.pref_MXML_TabsInHangingIndent = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Spaces Inside Attribute Braces")]
        [LocalizedDescription("CodeFormatter.Description.MXML.UseSpacesInsideAttributeBraces")]
        public bool Pref_MXML_UseSpacesInsideAttributeBraces
        {
            get { return this.pref_MXML_UseSpacesInsideAttributeBraces; }
            set { this.pref_MXML_UseSpacesInsideAttributeBraces = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Formatting Of Bound Attributes")]
        [LocalizedDescription("CodeFormatter.Description.MXML.UseFormattingOfBoundAttributes")]
        public bool Pref_MXML_UseFormattingOfBoundAttributes
        {
            get { return this.pref_MXML_UseFormattingOfBoundAttributes; }
            set { this.pref_MXML_UseFormattingOfBoundAttributes = value; }
        }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Spaces Inside Attribute Braces")]
        [LocalizedDescription("CodeFormatter.Description.MXML.SpacesInsideAttributeBraces")]
        public int Pref_MXML_SpacesInsideAttributeBraces
        {
            get { return this.pref_MXML_SpacesInsideAttributeBraces; }
            set { this.pref_MXML_SpacesInsideAttributeBraces = value; }
        }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Script CData Indent Tabs")]
        [LocalizedDescription("CodeFormatter.Description.MXML.ScriptCDataIndentTabs")]
        public int Pref_MXML_ScriptCDataIndentTabs
        {
            get { return this.pref_MXML_ScriptCDataIndentTabs; }
            set { this.pref_MXML_ScriptCDataIndentTabs = value; }
        }

        [DefaultValue(4)]
        [Category("MXML")]
        [DisplayName("Script Indent Tabs")]
        [LocalizedDescription("CodeFormatter.Description.MXML.ScriptIndentTabs")]
        public int Pref_MXML_ScriptIndentTabs
        {
            get { return this.pref_MXML_ScriptIndentTabs; }
            set { this.pref_MXML_ScriptIndentTabs = value; }
        }

        [DefaultValue(1)]
        [Category("MXML")]
        [DisplayName("Blank Lines At CData Start")]
        [LocalizedDescription("CodeFormatter.Description.MXML.BlankLinesAtCDataStart")]
        public int Pref_MXML_BlankLinesAtCDataStart
        {
            get { return this.pref_MXML_BlankLinesAtCDataStart; }
            set { this.pref_MXML_BlankLinesAtCDataStart = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Keep Script CData On Same Line")]
        [LocalizedDescription("CodeFormatter.Description.MXML.KeepScriptCDataOnSameLine")]
        public bool Pref_MXML_KeepScriptCDataOnSameLine
        {
            get { return this.pref_MXML_KeepScriptCDataOnSameLine; }
            set { this.pref_MXML_KeepScriptCDataOnSameLine = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Indent Tag Close")]
        [LocalizedDescription("CodeFormatter.Description.MXML.IndentTagClose")]
        public bool Pref_MXML_IndentTagClose
        {
            get { return this.pref_MXML_IndentTagClose; }
            set { this.pref_MXML_IndentTagClose = value; }
        }

        [DefaultValue(false)]
        [Category("MXML")]
        [DisplayName("Always Use Max Line Length")]
        [LocalizedDescription("CodeFormatter.Description.MXML.AlwaysUseMaxLineLength")]
        public bool Pref_MXML_AlwaysUseMaxLineLength
        {
            get { return this.pref_MXML_AlwaysUseMaxLineLength; }
            set { this.pref_MXML_AlwaysUseMaxLineLength = value; }
        }

        [DefaultValue(true)]
        [Category("MXML")]
        [DisplayName("Use Tags Do Not Format Inside")]
        [LocalizedDescription("CodeFormatter.Description.MXML.UseTagsDoNotFormatInside")]
        public bool Pref_MXML_UseTagsDoNotFormatInside
        {
            get { return this.pref_MXML_UseTagsDoNotFormatInside; }
            set { this.pref_MXML_UseTagsDoNotFormatInside = value; }
        }

        [DefaultValue("")]
        [Category("MXML")]
        [DisplayName("Parent Tags With Blank Lines After")]
        [LocalizedDescription("CodeFormatter.Description.MXML.ParentTagsWithBlankLinesAfter")]
        public string Pref_MXML_ParentTagsWithBlankLinesAfter
        {
            get { return this.pref_MXML_ParentTagsWithBlankLinesAfter; }
            set { this.pref_MXML_ParentTagsWithBlankLinesAfter = value; }
        }

        [DefaultValue("")]
        [Category("MXML")]
        [DisplayName("Tags Do Not Format Inside")]
        [LocalizedDescription("CodeFormatter.Description.MXML.TagsDoNotFormatInside")]
        public string Pref_MXML_TagsDoNotFormatInside
        {
            get { return this.pref_MXML_TagsDoNotFormatInside; }
            set { this.pref_MXML_TagsDoNotFormatInside = value; }
        }

        [DefaultValue("")]
        [Category("MXML")]
        [DisplayName("Tags With ActionScript Content")]
        [LocalizedDescription("CodeFormatter.Description.MXML.TagsWithASContent")]
        public string Pref_MXML_TagsWithASContent
        {
            get { return this.pref_MXML_TagsWithASContent; }
            set { this.pref_MXML_TagsWithASContent = value; }
        }

        [DefaultValue("")]
        [Category("MXML")]
        [DisplayName("Attribute Sort Data")]
        [LocalizedDescription("CodeFormatter.Description.MXML.SortAttrData")]
        public string Pref_MXML_SortAttrData
        {
            get { return this.pref_MXML_SortAttrData; }
            set { this.pref_MXML_SortAttrData = value; }
        }

        [DefaultValue("")]
        [Category("MXML")]
        [DisplayName("Attribute Groups")]
        [LocalizedDescription("CodeFormatter.Description.MXML.AttrGroups")]
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
            Pref_MXML_TagsDoNotFormatInside = ".*:Model,.*:XML";
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

    public enum AsteriskMode
    {
        AsIs = 0,
        All = 1,
        None = 2
    }

    public enum DeclAlignMode
    {
        Consecutive = 1,
        Scope = 2,
        Global = 3
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

    public enum WrapType
    {
        None = 1,
        DontProcess = 2,
        FormatNoNewlines = 4,
        ByColumn = 8,
        ByColumnOnlyAddNewlines = 16,
        ByTag = 128
    }

    public enum WrapIndent 
    {
        Normal = 1000,
        WrapElement = 1001
    }

}
