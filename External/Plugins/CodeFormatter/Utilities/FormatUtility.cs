using System;
using System.Collections.Generic;
using CodeFormatter.Handlers;
using CodeFormatter.Preferences;
using PluginCore;

namespace CodeFormatter.Utilities
{
    public class FormatUtility
    {
        public static void configureMXMLPrinter(MXMLPrettyPrinter printer, Settings settings)
        {
            Boolean useTabs = PluginBase.Settings.UseTabs;
            Int32 tabSize = PluginBase.Settings.TabWidth;
            Int32 spaceSize = PluginBase.Settings.IndentSize;
            printer.setAttrSortMode((Int32)settings.Pref_MXML_SortAttrMode);
            printer.setIndentAmount(spaceSize);
            printer.setUseTabs(useTabs);
            printer.setTabSize(tabSize);
            printer.setTabSize(tabSize);
            printer.setIndentAmount(tabSize);
            printer.setSortOtherAttrs(settings.Pref_MXML_SortExtraAttrs);
            printer.setSpacesAroundEquals(settings.Pref_MXML_SpacesAroundEquals);
            printer.setSpacesBeforeEmptyTagEnd(settings.Pref_MXML_SpacesBeforeEmptyTagEnd);
            printer.setKeepBlankLines(settings.Pref_MXML_KeepBlankLines);
            printer.setKeepRelativeCommentIndent(settings.Pref_MXML_KeepRelativeIndentInMultilineComments);
            printer.setBlankLinesBeforeComments(settings.Pref_MXML_BlankLinesBeforeComments);
            printer.setBlankLinesBeforeTags(settings.Pref_MXML_BlankLinesBeforeTags);
            printer.setBlankLinesAfterSpecificParentTags(settings.Pref_MXML_BlankLinesAfterSpecificParentTags);
            printer.setSpacesBetweenSiblingTags(settings.Pref_MXML_BlankLinesBetweenSiblingTags);
            printer.setSpacesAfterParentTags(settings.Pref_MXML_BlankLinesAfterParentTags);
            printer.setBlankLinesBeforeCloseTags(settings.Pref_MXML_BlankLinesBeforeClosingTags);
            printer.setWrapStyle((Int32)settings.Pref_MXML_WrapIndentStyle);
            printer.setHangingIndentTabs(settings.Pref_MXML_TabsInHangingIndent);
            printer.setUseSpacesInsideAttrBraces(settings.Pref_MXML_UseSpacesInsideAttributeBraces);
            printer.setFormatBoundAttributes(settings.Pref_MXML_UseFormattingOfBoundAttributes);
            printer.setSpacesInsideAttrBraces(settings.Pref_MXML_SpacesInsideAttributeBraces);
            printer.setCDATAIndentTabs(settings.Pref_MXML_ScriptCDataIndentTabs);
            printer.setScriptIndentTabs(settings.Pref_MXML_ScriptIndentTabs);
            printer.setBlankLinesAtCDataStart(settings.Pref_MXML_BlankLinesAtCDataStart);
            printer.setBlankLinesAtCDataEnd(settings.Pref_MXML_BlankLinesAtCDataStart);
            printer.setKeepCDataOnSameLine(settings.Pref_MXML_KeepScriptCDataOnSameLine);
            printer.setMaxLineLength(settings.Pref_MXML_MaxLineLength);
            printer.setWrapMode((Int32)settings.Pref_MXML_AttrWrapMode);
            printer.setAttrsPerLine(settings.Pref_MXML_AttrsPerLine);
            printer.setAddNewlineAfterLastAttr(settings.Pref_MXML_AddNewlineAfterLastAttr);
            printer.setIndentCloseTag(settings.Pref_MXML_IndentTagClose);
            printer.setUseAttrsToKeepOnSameLine(settings.Pref_MXML_UseAttrsToKeepOnSameLine);
            printer.setRequireCDATAForASContent(settings.Pref_MXML_RequireCDATAForASFormatting);
            printer.setAttrsToKeepOnSameLine(settings.Pref_MXML_AttrsToKeepOnSameLine);
            printer.setObeyMaxLineLength(settings.Pref_MXML_AlwaysUseMaxLineLength);
            String[] tags=settings.Pref_MXML_TagsCannotFormat.Split(',');
            List<String> tagSet=new List<String>();
            foreach (String tag in tags) 
            {
                if (tag.Length>0)
                {
                    tagSet.Add(tag);
                }
            }
            printer.setTagsThatCannotBeFormatted(tagSet);       
            tags=settings.Pref_MXML_TagsCanFormat.Split(',');
            tagSet=new List<String>();
            foreach (String tag in tags) 
            {
                if (tag.Length>0)
                {
                    tagSet.Add(tag);
                }
            }
            printer.setTagsThatCanBeFormatted(tagSet);
            tags = settings.Pref_MXML_TagsWithBlankLinesBefore.Split(',');
            tagSet=new List<String>();
            foreach (String tag in tags)
            {
                if (tag.Length>0)
                {
                    tagSet.Add(tag);
                }
            }
            printer.setTagsWithBlankLinesBeforeThem(tagSet);
            tags = settings.Pref_MXML_ParentTagsWithBlankLinesAfter.Split(',');
            tagSet=new List<String>();
            foreach (String tag in tags) 
            {
                if (tag.Length>0)
                {
                    tagSet.Add(tag);
                }
            }
            printer.setParentTagsWithBlankLinesAfterThem(tagSet);
            tags = settings.Pref_MXML_TagsWithASContent.Split(',');
            tagSet=new List<String>();
            foreach (String tag in tags) 
            {
                if (tag.Length>0)
                {
                    tagSet.Add(tag);
                }
            }
            printer.setASScriptTags(tagSet);
            List<AttrGroup> attrGroups=new List<AttrGroup>();
            String groupData=settings.Pref_MXML_AttrGroups;
            String[] groups = groupData.Split(Settings.LineSplitter);
            foreach (String g in groups) 
            {
                AttrGroup group=AttrGroup.load(g);
                if (group!=null) attrGroups.Add(group);
            }
            printer.setAttrGroups(attrGroups);      
            printer.setUsePrivateTags(settings.Pref_MXML_UseTagsDoNotFormatInside);
            tags = settings.Pref_MXML_TagsDoNotFormatInside.Split(',');
            List<String> tagList=new List<String>();
            foreach (String tag in tags) 
            {
                if (tag.Length>0)
                {
                    tagList.Add(tag);
                }
            }
            printer.setPrivateTags(tagList);
            configureASPrinter(printer.getASPrinter(), settings);
            printer.getASPrinter().setBlankLinesBeforeImports(0); //special case: we only want blank lines before imports in .as files
        }

        public static void configureASPrinter(ASPrettyPrinter printer, Settings settings)
        {
            Boolean useTabs = PluginBase.Settings.UseTabs;
            Int32 tabSize = PluginBase.Settings.TabWidth;
            Int32 spaceSize = PluginBase.Settings.IndentSize;
            Int32 braceStyle = PluginBase.Settings.CodingStyle == CodingStyle.BracesOnLine ? 4 : 5;
            printer.setIndentMultilineComments(settings.Pref_AS_IndentMultilineComments);
            printer.setBlankLinesBeforeFunction(settings.Pref_AS_BlankLinesBeforeFunctions);
            printer.setBlankLinesBeforeClass(settings.Pref_AS_BlankLinesBeforeClasses);
            printer.setBlankLinesBeforeControlStatement(settings.Pref_AS_BlankLinesBeforeControlStatements);
            printer.setBlankLinesBeforeImports(settings.Pref_AS_BlankLinesBeforeImportBlock);
            printer.setBlankLinesBeforeProperties(settings.Pref_AS_BlankLinesBeforeProperties);
            printer.setBlankLinesToStartFunctions(settings.Pref_AS_BlankLinesAtFunctionStart);
            printer.setBlankLinesToEndFunctions(settings.Pref_AS_BlankLinesAtFunctionEnd);
            printer.setBlockIndent(spaceSize);
            printer.setUseTabs(useTabs);
            printer.setTabSize(tabSize);
            printer.setSpacesAfterComma(settings.Pref_AS_SpacesAfterComma);
            printer.setSpacesBeforeComma(settings.Pref_AS_SpacesBeforeComma);
            printer.setCRBeforeOpenBrace(settings.Pref_AS_OpenBraceOnNewLine);
            printer.setCRBeforeCatch(settings.Pref_AS_CatchOnNewLine);
            printer.setCRBeforeElse(settings.Pref_AS_ElseOnNewLine);
            printer.setCRBeforeWhile(settings.Pref_AS_WhileOnNewLine);
            printer.setUseBraceStyleSetting(true);
            printer.setBraceStyleSetting(braceStyle);
            printer.setKeepBlankLines(settings.Pref_AS_KeepBlankLines);
            printer.setBlankLinesToKeep(settings.Pref_AS_BlankLinesToKeep);
            printer.setSpacesAroundAssignment(settings.Pref_AS_SpacesAroundAssignment);
            printer.setAdvancedSpacesAroundAssignmentInOptionalParameters(settings.Pref_AS_Tweak_SpacesAroundEqualsInOptionalParameters);
            printer.setUseAdvancedSpacesAroundAssignmentInOptionalParameters(settings.Pref_AS_Tweak_UseSpacesAroundEqualsInOptionalParameters);
            printer.setAdvancedSpacesAroundAssignmentInMetatags(settings.Pref_AS_Tweak_SpacesAroundEqualsInMetatags);
            printer.setUseAdvancedSpacesAroundAssignmentInMetatags(settings.Pref_AS_Tweak_UseSpacesAroundEqualsInMetatags);
            printer.setSpacesAroundColons(settings.Pref_AS_SpacesAroundColons);
            printer.setAdvancedSpacesAfterColonsInDeclarations(settings.Pref_AS_AdvancedSpacesAfterColonsInDeclarations);
            printer.setAdvancedSpacesBeforeColonsInDeclarations(settings.Pref_AS_AdvancedSpacesBeforeColonsInDeclarations);
            printer.setAdvancedSpacesAfterColonsInFunctionTypes(settings.Pref_AS_AdvancedSpacesAfterColonsInFunctionTypes);
            printer.setAdvancedSpacesBeforeColonsInFunctionTypes(settings.Pref_AS_AdvancedSpacesBeforeColonsInFunctionTypes);
            printer.setUseGlobalSpacesAroundColons(settings.Pref_AS_UseGlobalSpacesAroundColons);
            printer.setMaxLineLength(settings.Pref_AS_MaxLineLength);
            printer.setExpressionSpacesAroundSymbolicOperators(settings.Pref_AS_SpacesAroundSymbolicOperator);
            printer.setKeepElseIfOnSameLine(settings.Pref_AS_ElseIfOnSameLine);
            printer.setKeepSingleLineCommentsAtColumn1(settings.Pref_AS_KeepSLCommentsOnColumn1);
            printer.setUseLineCommentWrapping(settings.Pref_AS_UseLineCommentWrapping);
            printer.setUseMLCommentWrapping(settings.Pref_AS_UseMLCommentWrapping);
            printer.setMLCommentCollapseLines(settings.Pref_AS_MLCommentReflow);
            printer.setDocCommentCollapseLines(settings.Pref_AS_DocCommentReflow);
            printer.setMLTextOnNewLines(settings.Pref_AS_MLCommentHeaderOnSeparateLine);
            printer.setMLAsteriskMode((Int32)settings.Pref_AS_MLCommentAsteriskMode);
            printer.setUseDocCommentWrapping(settings.Pref_AS_UseDocCommentWrapping);
            printer.setDocCommentHangingIndentTabs(settings.Pref_AS_DocCommentHangingIndentTabs);
            printer.setDocCommentKeepBlankLines(settings.Pref_AS_DocCommentKeepBlankLines);
            printer.setMLCommentKeepBlankLines(settings.Pref_AS_MLCommentKeepBlankLines);
            printer.setKeepSingleLineFunctions(settings.Pref_AS_LeaveSingleLineFunctions);
            printer.setNoIndentForTerminators(settings.Pref_AS_UnindentExpressionTerminators);
            printer.setNoCRBeforeBreak(settings.Pref_AS_NoNewCRsBeforeBreak);
            printer.setNoCRBeforeContinue(settings.Pref_AS_NoNewCRsBeforeContinue);
            printer.setNoCRBeforeReturn(settings.Pref_AS_NoNewCRsBeforeReturn);
            printer.setNoCRBeforeThrow(settings.Pref_AS_NoNewCRsBeforeThrow);
            printer.setNoCRBeforeExpressions(settings.Pref_AS_NoNewCRsBeforeExpression);
            printer.setKeepRelativeCommentIndent(settings.Pref_AS_KeepRelativeIndentInDocComments);
            printer.setSpacesInsideParensEtc(settings.Pref_AS_SpacesInsideParens);
            printer.setUseSpacesInsideParensEtc(settings.Pref_AS_UseGlobalSpacesInsideParens);
            printer.setHangingIndentTabs(settings.Pref_AS_TabsInHangingIndent);
            printer.setAdvancedSpacesInsideArrayDeclBrackets(settings.Pref_AS_AdvancedSpacesInsideArrayDeclBrackets);
            printer.setAdvancedSpacesInsideArrayReferenceBrackets(settings.Pref_AS_AdvancedSpacesInsideArrayRefBrackets);
            printer.setAdvancedSpacesInsideObjectBraces(settings.Pref_AS_AdvancedSpacesInsideLiteralBraces);
            printer.setAdvancedSpacesInsideParensInOtherPlaces(settings.Pref_AS_AdvancedSpacesInsideParensInOtherPlaces);
            printer.setAdvancedSpacesInsideParensInParameterLists(settings.Pref_AS_AdvancedSpacesInsideParensInParameterLists);
            printer.setAdvancedSpacesInsideParensInArgumentLists(settings.Pref_AS_AdvancedSpacesInsideParensInArgumentLists);
            printer.setSpacesBetweenControlKeywordsAndParens(settings.Pref_AS_SpacesBeforeOpenControlParen);
            printer.setSpacesBeforeFormalParameters(settings.Pref_AS_SpacesBeforeFormalParameters);
            printer.setSpacesBeforeArguments(settings.Pref_AS_SpacesBeforeArguments);
            printer.setAlwaysGenerateIndent(settings.Pref_AS_AlwaysGenerateIndent);
            printer.setUseGNUBraceIndent(settings.Pref_AS_UseGnuBraceIndent);
            printer.setLoopBraceMode(settings.Pref_AS_EnsureLoopsHaveBraces ? ASPrettyPrinter.Braces_AddIfMissing : ASPrettyPrinter.Braces_NoModify);
            printer.setLoopBraceMode(settings.Pref_AS_AddBracesToLoops);
            printer.setSwitchBraceMode(settings.Pref_AS_EnsureSwitchCasesHaveBraces ? ASPrettyPrinter.Braces_AddIfMissing : ASPrettyPrinter.Braces_NoModify);
            printer.setSwitchBraceMode(settings.Pref_AS_AddBracesToCases);
            printer.setConditionalBraceMode(settings.Pref_AS_EnsureConditionalsHaveBraces ? ASPrettyPrinter.Braces_AddIfMissing : ASPrettyPrinter.Braces_NoModify);
            printer.setConditionalBraceMode(settings.Pref_AS_AddBracesToConditionals);
            printer.setIndentAtPackageLevel(!settings.Pref_AS_DontIndentPackageItems);
            printer.setIndentSwitchCases(!settings.Pref_AS_DontIndentSwitchCases);
            printer.setKeepingExcessDeclWhitespace(settings.Pref_AS_LeaveExtraWhitespaceAroundVarDecls);
            printer.setAlignDeclEquals(settings.Pref_AS_AlignDeclEquals);
            printer.setAlignDeclMode((Int32)settings.Pref_AS_AlignDeclMode);
            printer.setKeepSpacesBeforeLineComments(settings.Pref_AS_KeepSpacesBeforeLineComments);
            printer.setLineCommentColumn(settings.Pref_AS_AlignLineCommentsAtColumn);
            printer.setUseGlobalNewlineBeforeBraceSetting(settings.Pref_AS_UseGlobalCRBeforeBrace);
            printer.setAdvancedNewlineBeforeBraceSettings(settings.Pref_AS_AdvancedCRBeforeBraceSettings);
            printer.setCollapseSpaceForAdjacentParens(settings.Pref_AS_CollapseSpacesForAdjacentParens);
            //printer.setNewlineAfterBindable(settings.Pref_AS_NewlineAfterBindable);
            printer.setNewlineBeforeBindableFunction(settings.Pref_AS_NewlineBeforeBindableFunction);
            printer.setNewlineBeforeBindableProperty(settings.Pref_AS_NewlineBeforeBindableProperty);
            List<String> tags = new List<String>();
            tags.AddRange(settings.Pref_AS_MetaTagsOnSameLineAsTargetFunction.Split(','));
            printer.setMetaTagsToKeepOnSameLineAsFunction(tags);
            tags.Clear();
            tags.AddRange(settings.Pref_AS_MetaTagsOnSameLineAsTargetProperty.Split(','));
            printer.setMetaTagsToKeepOnSameLineAsProperty(tags);
            printer.setTrimTrailingWS(settings.Pref_AS_TrimTrailingWhitespace);
            printer.setSpacesAfterLabel(settings.Pref_AS_SpacesAfterLabel);
            printer.setEmptyStatementsOnNewLine(settings.Pref_AS_PutEmptyStatementsOnNewLine);
            printer.setUseAdvancedWrapping(settings.Pref_AS_UseAdvancedWrapping);
            printer.setAdvancedWrappingElements(settings.Pref_AS_AdvancedWrappingElements);
            printer.setAdvancedWrappingEnforceMax(settings.Pref_AS_AdvancedWrappingEnforceMax);
            printer.setWrapAllArgumentsIfAny(settings.Pref_AS_AdvancedWrappingAllArgs);
            printer.setWrapAllParametersIfAny(settings.Pref_AS_AdvancedWrappingAllParms);
            printer.setWrapFirstArgument(settings.Pref_AS_AdvancedWrappingFirstArg);
            printer.setWrapFirstParameter(settings.Pref_AS_AdvancedWrappingFirstParm);
            printer.setWrapFirstArrayItem(settings.Pref_AS_AdvancedWrappingFirstArrayItem);
            printer.setWrapFirstObjectItem(settings.Pref_AS_AdvancedWrappingFirstObjectItem);
            printer.setWrapAllArrayItemsIfAny(settings.Pref_AS_AdvancedWrappingAllArrayItems);
            printer.setWrapAllObjectItemsIfAny(settings.Pref_AS_AdvancedWrappingAllObjectItems);
            printer.setWrapArrayItemsAlignStart(settings.Pref_AS_AdvancedWrappingAlignArrayItems);
            printer.setWrapObjectItemsAlignStart(settings.Pref_AS_AdvancedWrappingAlignObjectItems);
            printer.setAdvancedWrappingGraceColumns(settings.Pref_AS_AdvancedWrappingGraceColumns);
            printer.setAdvancedWrappingPreservePhrases(settings.Pref_AS_AdvancedWrappingPreservePhrases);
            bool breakBeforeComma = settings.Pref_AS_BreakLinesBeforeComma;
            bool breakBeforeArithmetic = settings.Pref_AS_BreakLinesBeforeArithmetic;
            bool breakBeforeLogical = settings.Pref_AS_BreakLinesBeforeLogical;
            bool breakBeforeAssign = settings.Pref_AS_BreakLinesBeforeAssignment;
            int wrapIndentStyle = (Int32)settings.Pref_AS_WrapIndentStyle;
            WrapOptions options = new WrapOptions((Int32)settings.Pref_AS_WrapArrayDeclMode);
            options.setBeforeSeparator(breakBeforeComma);
            options.setBeforeArithmeticOperator(breakBeforeArithmetic);
            options.setBeforeLogicalOperator(breakBeforeLogical);
            options.setBeforeAssignmentOperator(breakBeforeAssign);
            options.setIndentStyle(wrapIndentStyle);
            printer.setArrayInitWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapMethodCallMode);
            options.setBeforeSeparator(breakBeforeComma);
            options.setBeforeArithmeticOperator(breakBeforeArithmetic);
            options.setBeforeLogicalOperator(breakBeforeLogical);
            options.setBeforeAssignmentOperator(breakBeforeAssign);
            options.setIndentStyle(wrapIndentStyle);
            printer.setMethodCallWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapMethodDeclMode);
            options.setBeforeSeparator(breakBeforeComma);
            options.setIndentStyle(wrapIndentStyle);
            printer.setMethodDeclWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapExpressionMode);
            options.setBeforeSeparator(breakBeforeComma);
            options.setBeforeArithmeticOperator(breakBeforeArithmetic);
            options.setBeforeLogicalOperator(breakBeforeLogical);
            options.setBeforeAssignmentOperator(breakBeforeAssign);
            options.setIndentStyle(wrapIndentStyle);
            printer.setExpressionWrapOptions(options);
            options = new WrapOptions((Int32)settings.Pref_AS_WrapXMLMode);
            options.setBeforeSeparator(breakBeforeComma);
            options.setBeforeArithmeticOperator(breakBeforeArithmetic);
            options.setBeforeLogicalOperator(breakBeforeLogical);
            options.setBeforeAssignmentOperator(breakBeforeAssign);
            options.setIndentStyle(wrapIndentStyle);
            printer.setXMLWrapOptions(options);
        }

    }

}