using System;
using System.Collections.Generic;
using CodeFormatter.Preferences;
using CodeFormatter.Handlers;
using CodeFormatter;

namespace CodeFormatter.Utilities
{
    public class FormatUtility
    {
        public static void ConfigureMXMLPrinter(MXMLPrettyPrinter printer, Settings settings, int tabSize)
        {
            printer.SetAttrSortMode((Int32)settings.Pref_MXML_SortAttrMode);
            printer.SetUseTabs(settings.Pref_Flex_UseTabs);
            printer.SetTabSize(tabSize);
            printer.SetIndentAmount(tabSize);
            List<string> sortAttrData = new List<string>();
            string[] sortAttrDataArray = settings.Pref_MXML_SortAttrData.Split('\n');
            foreach (string attr in sortAttrDataArray)
            {
                sortAttrData.Add(attr);
            }
            printer.SetManualAttrSortData(sortAttrData);
            printer.SetSortOtherAttrs(settings.Pref_MXML_SortExtraAttrs);
            printer.SetSpacesAroundEquals(settings.Pref_MXML_SpacesAroundEquals);
            printer.SetSpacesBeforeEmptyTagEnd(settings.Pref_MXML_SpacesBeforeEmptyTagEnd);
            printer.SetKeepBlankLines(settings.Pref_MXML_KeepBlankLines);
            printer.SetBlankLinesBeforeTags(settings.Pref_MXML_BlankLinesBeforeTags);
            printer.SetWrapStyle((Int32)settings.Pref_MXML_WrapIndentStyle);
            printer.SetMaxLineLength(settings.Pref_MXML_MaxLineLength);
            printer.SetWrapMode((Int32)settings.Pref_MXML_AttrWrapMode);
            printer.SetAttrsPerLine(settings.Pref_MXML_AttrsPerLine);
            printer.SetAddNewlineAfterLastAttr(settings.Pref_MXML_AddNewlineAfterLastAttr);
            printer.SetUseAttrsToKeepOnSameLine(settings.Pref_MXML_UseAttrsToKeepOnSameLine);
            printer.SetRequireCDATAForASContent(settings.Pref_MXML_RequireCDATAForASFormatting);
            printer.SetAttrsToKeepOnSameLine(settings.Pref_MXML_AttrsToKeepOnSameLine);
            String[] tags = settings.Pref_MXML_TagsCannotFormat.Split(',');
            List<String> tagSet = new List<String>();
            foreach (String tag in tags)
            {
                if (tag.Length > 0) tagSet.Add(tag);
            }
            printer.SetTagsThatCannotBeFormatted(tagSet);
            tags = settings.Pref_MXML_TagsCanFormat.Split(',');
            tagSet = new List<String>();
            foreach (String tag in tags)
            {
                if (tag.Length > 0) tagSet.Add(tag);
            }
            printer.SetTagsThatCanBeFormatted(tagSet);
            tags = settings.Pref_MXML_TagsWithBlankLinesBefore.Split(',');
            tagSet = new List<String>();
            foreach (String tag in tags)
            {
                if (tag.Length > 0) tagSet.Add(tag);
            }
            printer.SetTagsWithBlankLinesBeforeThem(tagSet);
            tags = settings.Pref_MXML_TagsWithASContent.Split(',');
            tagSet = new List<String>();
            foreach (String tag in tags)
            {
                if (tag.Length > 0) tagSet.Add(tag);
            }
            printer.SetASScriptTags(tagSet);
            List<AttrGroup> attrGroups = new List<AttrGroup>();
            String groupData = settings.Pref_MXML_AttrGroups;
            String[] groups = groupData.Split(Settings.LineSplitter);
            foreach (String g in groups)
            {
                AttrGroup group = AttrGroup.Load(g);
                if (group != null) attrGroups.Add(group);
            }
            printer.SetAttrGroups(attrGroups);
            ConfigureASPrinter(printer.GetASPrinter(), settings, tabSize);
        }

        public static void ConfigureASPrinter(ASPrettyPrinter printer, Settings settings, int tabSize)
        {
            printer.SetIndentMultilineComments(settings.Pref_AS_IndentMultilineComments);
            printer.SetBlankLinesBeforeFunction(settings.Pref_AS_BlankLinesBeforeFunctions);
            printer.SetBlankLinesBeforeClass(settings.Pref_AS_BlankLinesBeforeClasses);
            printer.SetBlankLinesBeforeControlStatement(settings.Pref_AS_BlankLinesBeforeControlStatements);
            printer.SetBlankLinesBeforeProperties(settings.Pref_AS_BlankLinesBeforeProperties);
            printer.SetBlockIndent(tabSize);
            printer.SetUseTabs(settings.Pref_Flex_UseTabs);
            printer.SetTabSize(tabSize);
            printer.SetSpacesAfterComma(settings.Pref_AS_SpacesAfterComma);
            printer.SetSpacesBeforeComma(settings.Pref_AS_SpacesBeforeComma);
            printer.SetCRBeforeOpenBrace(settings.Pref_AS_OpenBraceOnNewLine);
            printer.SetCRBeforeCatch(settings.Pref_AS_CatchOnNewLine);
            printer.SetCRBeforeElse(settings.Pref_AS_ElseOnNewLine);
            printer.SetUseBraceStyleSetting(settings.Pref_AS_UseBraceStyle);
            printer.SetBraceStyleSetting((Int32)settings.Pref_AS_BraceStyle);
            printer.SetKeepBlankLines(settings.Pref_AS_KeepBlankLines);
            printer.SetBlankLinesToKeep(settings.Pref_AS_BlankLinesToKeep);
            printer.SetSpacesAroundAssignment(settings.Pref_AS_SpacesAroundAssignment);
            printer.SetAdvancedSpacesAroundAssignmentInOptionalParameters(settings.Pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters);
            printer.SetUseAdvancedSpacesAroundAssignmentInOptionalParameters(settings.Pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters);
            printer.SetSpacesAroundColons(settings.Pref_AS_SpacesAroundColons);
            printer.SetAdvancedSpacesAfterColons(settings.Pref_AS_AdvancedSpacesAfterColons);
            printer.SetAdvancedSpacesBeforeColons(settings.Pref_AS_AdvancedSpacesBeforeColons);
            printer.SetUseGlobalSpacesAroundColons(settings.Pref_AS_UseGlobalSpacesAroundColons);
            printer.SetMaxLineLength(settings.Pref_AS_MaxLineLength);
            printer.SetExpressionSpacesAroundSymbolicOperators(settings.Pref_AS_SpacesAroundSymbolicOperator);
            printer.SetKeepElseIfOnSameLine(settings.Pref_AS_ElseIfOnSameLine);
            printer.SetKeepSingleLineCommentsAtColumn1(settings.Pref_AS_KeepSLCommentsOnColumn1);
            printer.SetSpacesInsideParensEtc(settings.Pref_AS_SpacesInsideParens);
            printer.SetUseSpacesInsideParensEtc(settings.Pref_AS_UseGlobalSpacesInsideParens);
            printer.SetAdvancedSpacesInsideArrayDeclBrackets(settings.Pref_AS_AdvancedSpacesInsideArrayDeclBrackets);
            printer.SetAdvancedSpacesInsideArrayReferenceBrackets(settings.Pref_AS_AdvancedSpacesInsideArrayRefBrackets);
            printer.SetAdvancedSpacesInsideObjectBraces(settings.Pref_AS_AdvancedSpacesInsideLiteralBraces);
            printer.SetAdvancedSpacesInsideParens(settings.Pref_AS_AdvancedSpacesInsideParens);
            printer.SetSpacesBetweenControlKeywordsAndParens(settings.Pref_AS_SpacesBeforeOpenControlParen);
            printer.SetAlwaysGenerateIndent(settings.Pref_AS_AlwaysGenerateIndent);
            printer.SetIndentAtPackageLevel(!settings.Pref_AS_DontIndentPackageItems);
            printer.SetKeepingExcessDeclWhitespace(settings.Pref_AS_LeaveExtraWhitespaceAroundVarDecls);
            printer.SetCollapseSpaceForAdjacentParens(settings.Pref_AS_CollapseSpacesForAdjacentParens);
            printer.SetNewlineAfterBindable(settings.Pref_AS_NewlineAfterBindable);
            printer.SetTrimTrailingWS(settings.Pref_AS_TrimTrailingWhitespace);
            printer.SetSpacesAfterLabel(settings.Pref_AS_SpacesAfterLabel);
            printer.SetEmptyStatementsOnNewLine(settings.Pref_AS_PutEmptyStatementsOnNewLine);
            bool breakBeforeComma = settings.Pref_AS_BreakLinesBeforeComma;
            int wrapIndentStyle = (Int32)settings.Pref_AS_WrapIndentStyle;
            WrapOptions options = new WrapOptions((Int32)settings.Pref_AS_WrapArrayDeclMode);
            options.BeforeSeparator = breakBeforeComma;
            options.IndentStyle = wrapIndentStyle;
            printer.SetArrayInitWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapMethodCallMode);
            options.BeforeSeparator = breakBeforeComma;
            options.IndentStyle = wrapIndentStyle;
            printer.SetMethodCallWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapMethodDeclMode);
            options.BeforeSeparator = breakBeforeComma;
            options.IndentStyle = wrapIndentStyle;
            printer.SetMethodDeclWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapExpressionMode);
            options.BeforeSeparator = breakBeforeComma;
            options.IndentStyle = wrapIndentStyle;
            printer.SetExpressionWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapXMLMode);
            options.BeforeSeparator = breakBeforeComma;
            options.IndentStyle = wrapIndentStyle;
            printer.SetXMLWrapOptions(options);
        }

    }

}
