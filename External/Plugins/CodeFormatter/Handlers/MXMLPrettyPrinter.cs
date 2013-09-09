using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CodeFormatter.InfoCollector;
using CodeFormatter.Preferences;
using Antlr.Runtime.Tree;
using Antlr.Runtime;
using Antlr.Utility;

namespace CodeFormatter.Handlers
{
    public class MXMLPrettyPrinter
    {
        public const int MXML_ATTR_ORDERING_NONE = 0;
        public const int MXML_ATTR_ORDERING_USEDATA = 2;
        public const int MXML_ATTR_WRAP_LINE_Length = 51;
        public const int MXML_ATTR_WRAP_COUNT_PER_LINE = 52;
        public const int MXML_ATTR_WRAP_NONE = 53;
        public const int MXML_ATTR_WRAP_DEFAULT = 54; // For groups; same as main setting
        public const int MXML_Sort_AscByCase = 11;
        public const int MXML_Sort_AscNoCase = 12;
        public const int MXML_Sort_GroupOrder = 13;
        public const int MXML_Sort_None = 14;
        public const string CDataEnd = "]]>";
        public const string CDataStart = "<![CDATA[";

        private int mAttrOrderMode = MXML_ATTR_ORDERING_NONE;
        private int mIndentAmount;
        private string mSource;
        private int mCurrentIndent;
        private int mMaxLineLength;
        private int mAttrsPerLine;
        private int mWrapMode;
        private int mWrapStyle;
        private Dictionary<int, ReplacementRange> mReplaceMap;
        private bool mSkipNextIndent;
        private List<string> mTagStack;
        private Point mSelectedRange; //x=start line, y=end line (1-based)(lines 5 and 6 would be 5,6)
        private Point mOutputRange; //x=start offset, y=end offset (0-based offsets into outputbuffer)
        private Point mReplaceRange; //same semantics as mSelectedRange
        private int mSpacesBeforeEmptyTagEnd = 0;
        private bool mKeepBlankLines = true;
        private bool mDoFormat;
        private bool mSortOtherAttrs = true;
        private bool mAddNewlineAfterLastAttr = false;
        private List<string> mManualAttrSortOrder;
        private Dictionary<string, AttrGroup> mAttrGroups;
        private int mSpacesAroundEquals = 1;
        private bool mUseTabs;
        private int mTabSize;
        private string mEnclosingTagName; //holds name of last open tag temporarily.  Use GetEnclosingTag() for the correct data
        private List<string> mTagsWhoseTextContentsCanBeFormatted;
        private List<string> mTagsWhoseTextContentCanNeverBeFormatted;
        private List<string> mTagsWithBlankLinesBeforeThem;
        private List<string> mASScriptTags;
        private bool mRequireCDATAForASContent;
        private int mBlankLinesBeforeTags;
        private bool mUseAttrsToKeepOnSameLine = false;
        private int mAttrsToKeepOnSameLine = 4;
        private List<Exception> mParseErrors;
        private ASPrettyPrinter mASPrinter;

        private Dictionary<string, List<string>> mHashedGroupAttrs = new Dictionary<string, List<string>>();

        public bool IsUseTabs()
        {
            return mUseTabs;
        }

        public void SetUseTabs(bool useTabs)
        {
            mUseTabs = useTabs;
        }

        public int GetTabSize()
        {
            return mTabSize;
        }

        public void SetTabSize(int tabSize)
        {
            mTabSize = tabSize;
        }

        private void Initialize()
        {
            mASPrinter = new ASPrettyPrinter(false, "");
            mManualAttrSortOrder = new List<string>();
            mIndentAmount = 4;
            mDoFormat = true;
            mWrapStyle = WrapOptions.WRAP_STYLE_INDENT_NORMAL;
            mTagsWhoseTextContentsCanBeFormatted = new List<string>();
            mTagsWhoseTextContentsCanBeFormatted.Add("mx:List");
            mTagsWhoseTextContentsCanBeFormatted.Add("fx:List");
            mTagsWhoseTextContentCanNeverBeFormatted = new List<string>();
            mTagsWhoseTextContentCanNeverBeFormatted.Add("mx:String");
            mTagsWhoseTextContentCanNeverBeFormatted.Add("fx:String");
            mTagsWithBlankLinesBeforeThem = new List<string>();
            mASScriptTags = new List<string>();
            mASScriptTags.Add(".*:Script");
            mASScriptTags.Add("fx:Script");
            mASScriptTags.Add("mx:Script");
            mRequireCDATAForASContent = false;
        }

        public MXMLPrettyPrinter(string sourceData)
        {
            Initialize();
            mSource = sourceData;
        }

        public string Print(int startIndent)
        {
            mTagStack = new List<string>();
            mSkipNextIndent = false;
            mSource = CodeFormatter.InfoCollector.Utilities.ConvertCarriageReturnsToLineFeeds(mSource);
            mCurrentIndent = startIndent;
            mReplaceMap = null;
            MXMLLexer lex = new MXMLLexer(new ANTLRStringStream(mSource));
            StringBuilder buffer = new StringBuilder();
            lex.mDOCUMENT();
            PrettyPrint(lex.GetTokens(), buffer);
            if (!(ASFormatter.ValidateNonWhitespaceCharCounts(buffer.ToString(), mSource)))
            {
                if (mParseErrors == null) mParseErrors = new List<Exception>();
                mParseErrors.Add(new Exception("Internal error: Formatted text doesn't match source. " + buffer.ToString() + "!=" + mSource));
                return null;
            }
            return buffer.ToString();
        }

        private String GetEnclosingTag()
        {
            if (mTagStack.Count == 0) return "";
            return mTagStack.ToArray()[mTagStack.Count - 1];
        }

        private int GetPCDataTokens(List<CommonToken> tokens, int tokenIndex, List<CommonToken> pcDataTokens, bool[] foundNonWhitespace)
        {
            foundNonWhitespace[0] = false;
            int newTokenIndex = tokenIndex + 1;
            while (true)
            {
                if (newTokenIndex >= tokens.Count) break;
                CommonToken token = (CommonToken)tokens.ToArray()[newTokenIndex];
                if (token.Type == MXMLLexer.PCDATA || token.Type == MXMLLexer.EOL || token.Type == MXMLLexer.WS)
                {
                    pcDataTokens.Add(token);
                    if (token.Text.Trim().Length > 0) foundNonWhitespace[0] = true;
                }
                else
                {
                    // Not a content type we are handling
                    break;
                }
                newTokenIndex++;
            }
            newTokenIndex--; // Back up 1 since the main token loop will increment
            return newTokenIndex;
        }

        private int ProcessPostTagText(List<CommonToken> tokens, int tokenIndex, StringBuilder buffer, int linesToInsert)
        {
            // If this is an actionscript tag, then we need to grab all of the next contents and
            // run it through the parser, if there is not a CData tag
            if (IsASFormattingTag(GetEnclosingTag()))
            {
                bool[] hasChars = new bool[1];
                List<CommonToken> pcDataTokens = new List<CommonToken>();
                int newTokenIndex = GetPCDataTokens(tokens, tokenIndex, pcDataTokens, hasChars);
                if (pcDataTokens.Count > 0)
                {
                    CommonToken tempToken = new CommonToken(pcDataTokens[0]);
                    StringBuilder tokenText = new StringBuilder();
                    foreach (CommonToken token in pcDataTokens)
                    {
                        tokenText.Append(token.Text);
                    }
                    string tokenString = tokenText.ToString();
                    if (tokenString.Trim().Length > 0)
                    {
                        tempToken.Text = tokenString;
                        bool success = ProcessActionScriptBlock(tempToken, buffer);
                        if (success) return newTokenIndex;
                    }
                }
            }
            bool foundNonWhitespace = false;
            // If this is not a tag whose contents can be formatted, then we need to peek at the
            // subsequent pcdata/whitespace tokens to see if there are any non-whitespace contents
            if (!mTagsWhoseTextContentsCanBeFormatted.Contains(GetEnclosingTag()))
            {
                bool[] hasChars = new bool[1];
                List<CommonToken> pcDataTokens = new List<CommonToken>();
                int newTokenIndex = GetPCDataTokens(tokens, tokenIndex, pcDataTokens, hasChars);
                foundNonWhitespace = hasChars[0];
                // If we found characters, indicating that we must preserve the current formatting exactly,
                // or if this is a tag where even whitespace is significant, then output the tokens as is.
                if (foundNonWhitespace || mTagsWhoseTextContentCanNeverBeFormatted.Contains(GetEnclosingTag()))
                {
                    foreach (CommonToken pcdataToken in pcDataTokens)
                    {
                        // Convert to \n delimiters to match the rest of my output
                        string data = pcdataToken.Text.Replace("\r\n", "\n");
                        data = data.Replace("\r", "\n");
                        buffer.Append(data);
                    }
                    mSkipNextIndent = true;
                    return newTokenIndex;
                }
            }
            // We will drop through to here if
            // 1. this is a tag that explicitly allows indenting of text content, or
            // 2. there is only whitespace in the text content, and this tag is not explicitly barred from formatting
            for (int i = 0; i < linesToInsert; i++)
            {
                InsertCR(buffer, false);
            }
            return tokenIndex;
        }

        private void PrettyPrint(List<CommonToken> tokens, StringBuilder buffer)
        {
            for (int tokenIndex = 0; tokenIndex < tokens.Count; tokenIndex++)
            {
                CommonToken token = tokens[tokenIndex];
                switch (token.Type)
                {
                    case MXMLLexer.COMMENT:
                        UpdatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                        string[] commentLines = token.Text.Split('\n');
                        for (int j = 0; j < commentLines.Length; j++)
                        {
                            bool onLastLine = (j == commentLines.Length - 1);
                            int indentAmount = mCurrentIndent;
                            string data = commentLines[j].Trim();
                            if (j > 0)
                            {
                                // On a middle line, indent the text to the right of the <!-- .
                                if (!onLastLine || !data.StartsWith("-->")) indentAmount += 5;
                            }
                            // Only add indent if on an empty line
                            if (data.Length > 0)
                            {
                                if (ASFormatter.IsLineEmpty(buffer))
                                {
                                    buffer.Append(GenerateIndent(indentAmount));
                                }
                                buffer.Append(data);
                            }
                            if (!onLastLine) buffer.Append('\n');
                        }
                        tokenIndex = ProcessPostTagText(tokens, tokenIndex, buffer, 1);
                        break;

                    case MXMLLexer.DECL_START:
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.DECL_STOP);
                        InsertCR(buffer, false);
                        break;

                    case MXMLLexer.TAG_OPEN:
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.TAG_CLOSE);
                        if (mEnclosingTagName != null) mTagStack.Add(mEnclosingTagName);
                        mCurrentIndent += mIndentAmount;
                        tokenIndex = ProcessPostTagText(tokens, tokenIndex, buffer, 1);
                        break;

                    case MXMLLexer.END_TAG_OPEN:
                        mCurrentIndent -= mIndentAmount;
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.TAG_CLOSE);
                        if (mTagStack.Count > 0) mTagStack.RemoveAt(mTagStack.Count - 1);
                        tokenIndex = ProcessPostTagText(tokens, tokenIndex, buffer, 1);
                        break;

                    case MXMLLexer.EMPTY_TAG_OPEN:
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.EMPTYTAG_CLOSE);
                        tokenIndex = ProcessPostTagText(tokens, tokenIndex, buffer, 1);
                        break;

                    case MXMLLexer.CDATA:
                        UpdatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                        // If no enclosing tag, or there is one but it's not one that Contains scripts, then just do regular intra-tag data processing
                        if (!IsASFormattingTag(GetEnclosingTag())) //mEnclosingTagName.endsWith(":Script"))
                        {
                            ProcessPCData(token, buffer);
                            InsertCR(buffer, false);
                            break;
                        }
                        // Handle script
                        bool succeeded = ProcessActionScriptBlock(token, buffer);
                        if (succeeded) break;
                        buffer.Append(token.Text);
                        break;

                    case MXMLLexer.PCDATA:
                        UpdatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                        ProcessPCData(token, buffer);
                        break;

                    case MXMLLexer.EOL:
                        if (!mDoFormat) InsertCR(buffer, true);
                        else if (IsKeepBlankLines())
                        {
                            // Special handling to grab subsequent newlines and determine the proper number of
                            // blank lines.  Alg: walk next tokens until I hit EOF or a non-EOL, non-WS token.
                            // Insert blank lines on each found EOL.
                            tokenIndex++;
                            bool doLoop = true;
                            while (tokenIndex < tokens.Count && doLoop)
                            {
                                token = tokens[tokenIndex];
                                switch (token.Type)
                                {
                                    case MXMLLexer.EOL:
                                        InsertCR(buffer, true);
                                        tokenIndex++;
                                        break;

                                    case MXMLLexer.WS:
                                        tokenIndex++;
                                        // Do nothing...
                                        break;

                                    case MXMLLexer.PCDATA:
                                        string nonWS = token.Text.Trim();
                                        if (nonWS.Length == 0) tokenIndex++;
                                        else doLoop = false;
                                        break;

                                    // Otherwise drop through to default case
                                    default:
                                        // Non-whitespace, need to kick out
                                        tokenIndex--; // Revert back to previous token, whatever it was
                                        doLoop = false; // Kick out of loop
                                        break;
                                }
                            }
                            // If we kick out because we're at the end of the token stream, we should drop out of the "for" loop correctly
                        }
                        break;

                    case MXMLLexer.WS:
                        if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
                        {
                            buffer.Append(token.Text);
                        }
                        break;

                    default:
                        UpdatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                        buffer.Append(token.Text);
                        break;
                }
            }
            if (mOutputRange != null && mOutputRange.Y < 0 && mReplaceRange != null)
            {
                mOutputRange.Y = buffer.Length;
                mReplaceRange.Y = mSource.Length;
            }
            if (mOutputRange != null)
            {
                // Set the output range to start at the previous line start
                string bufferString = buffer.ToString();
                if (bufferString[mOutputRange.X] == '\n') mOutputRange.X++;
                int lastCR = buffer.ToString().LastIndexOf("\n", mOutputRange.X);
                if (lastCR >= 0) mOutputRange.X = lastCR + 1;
                else mOutputRange.X = 0;
                // Move outputRange.Y to contain the next CR, if we are right on it
                if (mOutputRange.Y < bufferString.Length && bufferString[mOutputRange.Y] == '\n') mOutputRange.Y++;
                // Now, find the previous CR, which we will *not* include in the range
                int nextCR = buffer.ToString().LastIndexOf("\n", mOutputRange.Y);
                if (nextCR >= 0) mOutputRange.Y = nextCR; // Don't need to actually include the CR character
            }
            if (mReplaceRange != null)
            {
                // Set the Replace range to start at the previous line start
                // if we're already at the end of line, then move forward past the end of line
                if (mSource[mReplaceRange.X] == '\n') mReplaceRange.X++;
                else if (IsCarriageReturnPair(mSource, mReplaceRange.X)) mReplaceRange.X += 2;
                int lastCR = mSource.LastIndexOf('\n', mReplaceRange.X);
                if (lastCR >= 0) mReplaceRange.X = lastCR + 1;
                else mReplaceRange.X = 0;
                // Again, move past the end of line, if we're on it
                if (mReplaceRange.Y < mSource.Length && mSource[mReplaceRange.Y] == '\n') mReplaceRange.Y++;
                else if (IsCarriageReturnPair(mSource, mReplaceRange.Y)) mReplaceRange.Y += 2;
                // Now, go backwards to the previous EOL
                int nextCR = mSource.LastIndexOf('\n', mReplaceRange.Y);
                if (nextCR >= 0 && nextCR > mReplaceRange.X)
                {
                    mReplaceRange.Y = nextCR; // Don't need to actually include the CR
                    if (nextCR > 0 && IsCarriageReturnPair(mSource, mReplaceRange.Y - 1))
                    {
                        // mSource.charAt(mReplaceRange.Y)=='\n' && mSource.charAt(mReplaceRange.Y-1)=='\r')
                        mReplaceRange.Y--; //need to move before CR pair, if it's a pair
                    }
                }
            }
        }

        private bool ProcessActionScriptBlock(CommonToken token, StringBuilder buffer)
        {
            int startIndex = token.Text.IndexOf(CDataStart);
            int endIndex = token.Text.LastIndexOf(CDataEnd);
            // If we found the cdata start and end or if we don't require those
            if (!IsRequireCDATAForASContent() || (startIndex >= 0 && endIndex >= 0))
            {
                int cdataOffset = 0;
                int preTextCRCount = 0;
                string text = token.Text;
                if (endIndex >= 0) text = text.Substring(0, endIndex);
                if (startIndex >= 0)
                {
                    text = text.Substring(startIndex + CDataStart.Length);
                    cdataOffset = CDataStart.Length;
                }
                for (int k = 0; k < text.Length; k++)
                {
                    char c = text[k];
                    if (!Char.IsWhiteSpace(c)) break;
                    if (c == '\n' || c == '\r')
                    {
                        if (IsCarriageReturnPair(text, k)) k++; //skip an extra character
                        preTextCRCount++;
                    }
                }
                // mASPrinter.SetSelectedRange(null);
                //If we are attempting a partial format and we haven't already captured the boundaries
                bool includesEndOfActionScript = false;
                int lineCount = 0;
                if (mDoFormat && mSelectedRange != null && (mOutputRange == null || mOutputRange.Y < 0))
                {
                    // String[] lines=SplitTextOnLineBreaks(text);
                    lineCount = CountLines(text);
                    int startLine = token.Line;
                    int endLine = startLine + lineCount - 1; //lines.Length-1;
                    if (mOutputRange == null)
                    {
                        // If the selected lines start inside the script block
                        if (mSelectedRange.X >= startLine && mSelectedRange.X <= endLine)
                        {
                            // Now, determine whether the selection end is also included and determine the
                            // appropriate selection setting for the actionscript printer selection range
                            if (mSelectedRange.Y < endLine) // Ends during block (< so that it doesn't include last line of block)
                            {
                                mASPrinter.SetSelectedRange(new Point(mSelectedRange.X - startLine + 1, mSelectedRange.Y - startLine + 1));
                                includesEndOfActionScript = false;
                            }
                            else
                            {
                                // Doesn't end during block, so go until end of block
                                mASPrinter.SetSelectedRange(new Point(mSelectedRange.X - startLine + 1, lineCount)); //lines.Length));
                                includesEndOfActionScript = true;
                            }
                        }
                    }
                    else
                    {
                        // We are in the middle of the selected area.  Does the selected area end during
                        // the actionscript block, or does it cover the entire block.
                        if (mSelectedRange.Y <= endLine) //ends during block
                        {
                            mASPrinter.SetSelectedRange(new Point(1, mSelectedRange.Y - startLine + 1));
                            includesEndOfActionScript = false;
                        }
                    }
                }
                mASPrinter.SetDoFormat(mDoFormat);
                mASPrinter.SetData(text);
                int codeStartIndent = mCurrentIndent;
                if (startIndex >= 0) codeStartIndent += mIndentAmount;
                string resultData = mASPrinter.Print(codeStartIndent);
                {
                    if (resultData == null)
                    {
                        mParseErrors = mASPrinter.GetParseErrors();
                        if (mParseErrors != null)
                        {
                            foreach (Exception ex in mParseErrors)
                            {
                                if (ex is RecognitionException)
                                {
                                    RecognitionException rex = (RecognitionException)ex;
                                    CommonToken t = (CommonToken)rex.Token;
                                    int offset = token.Line - 1;
                                    if (t != null)
                                    {
                                        t.Line = t.Line + offset;
                                    }
                                    rex.Line += offset;
                                }
                            }
                        }
                        throw new Exception();
                    }
                    if (startIndex >= 0)
                    {
                        AddIndentIfAtStartOfLine(buffer);
                        buffer.Append(CDataStart);
                    }
                    if (mDoFormat) InsertCR(buffer, false);
                    else
                    {
                        for (int k = 0; k < preTextCRCount; k++)
                        {
                            InsertCR(buffer, true);
                        }
                    }
                    int saveIndent = mCurrentIndent;
                    if (startIndex >= 0) mCurrentIndent += mIndentAmount;
                    AddIndentIfAtStartOfLine(buffer);
                    mCurrentIndent = saveIndent;
                    // TODO: change to determine the amount of leading whitespace first and capture
                    // the previous buffer Length
                    int leadingWhitespaceCount = 0;
                    int leadingCRCount = 0;
                    for (; leadingWhitespaceCount < resultData.Length; leadingWhitespaceCount++)
                    {
                        char c = resultData[leadingWhitespaceCount];
                        if (!Char.IsWhiteSpace(c)) break;
                        if (c == '\n') leadingCRCount++;
                    }
                    int oldLength = buffer.Length;
                    string TrimmedResult = resultData.Trim();
                    buffer.Append(TrimmedResult);
                    // int trailingWhitespaceCount = resultData.Length - TrimmedResult.Length - leadingWhitespaceCount;
                    // Now, patch up the partial format return boundaries if necessary
                    if (mASPrinter.GetSelectedRange() != null)
                    {
                        Point outputRange = mASPrinter.GetOutputRange();
                        Point ReplaceRange = mASPrinter.GetReplaceRange();
                        if (outputRange != null && ReplaceRange != null)
                        {
                            if (mOutputRange == null)
                            {
                                mOutputRange = new Point(0, -1);
                                mReplaceRange = new Point(0, -1);
                                // Establish the beginning boundaries
                                mOutputRange.X = oldLength + outputRange.X - leadingWhitespaceCount;
                                // mReplaceRange.X=token.Line+ReplaceLines.x-1; //adjust for leading CRs and existing whitespace
                                mReplaceRange.X = ((CommonToken)token).StartIndex + cdataOffset + ReplaceRange.X; //-leadingWhitespaceCount;
                                // Selected range starts, possibly also ends inside actionscript block
                                if (includesEndOfActionScript) { } // Goes to end of actionscript; don't need to do anything here
                                else
                                {
                                    // If it stops part way through the block; we need to finish output/Replace boundaries here
                                    mOutputRange.Y = oldLength + Math.Min(TrimmedResult.Length, outputRange.Y - leadingWhitespaceCount); //outputRange.Y-leadingWhitespaceCount;//-trailingWhitespaceCount;
                                    // mReplaceRange.Y=token.Line+ReplaceLines.y-1; //adjust for leading CRs and existing whitespace
                                    mReplaceRange.Y = ((CommonToken)token).StartIndex + cdataOffset + ReplaceRange.Y;
                                }
                            }
                            else
                            {
                                // It stops part way through the block; we need to finish output/Replace boundaries here
                                mOutputRange.Y = oldLength + Math.Min(TrimmedResult.Length, outputRange.Y - leadingWhitespaceCount);
                                // mReplaceRange.Y=token.Line+ReplaceLines.y-1; //adjust for leading CRs and existing whitespace
                                mReplaceRange.Y = ((CommonToken)token).StartIndex + cdataOffset + ReplaceRange.Y; //-leadingWhitespaceCount;
                            }
                        }
                    }
                    InsertCR(buffer, false);
                    AddIndentIfAtStartOfLine(buffer);
                    if (!mDoFormat)
                    {
                        for (int k = text.Length - 1; k >= 0; k--)
                        {
                            char c = text[k];
                            if (!Char.IsWhiteSpace(c)) break;
                            if (c == '\n') InsertCR(buffer, true);
                        }
                        AddIndentIfAtStartOfLine(buffer);
                    }
                    // Update the formatting boundary to catch the case where
                    // the start of the selection only catches the end of the code block.
                    if (mDoFormat && mSelectedRange != null && mOutputRange == null)
                    {
                        if (token.Line + lineCount - 1 >= mSelectedRange.X)
                        {
                            mOutputRange = new Point(buffer.Length, -1);
                            mReplaceRange = new Point(0, -1);
                            mReplaceRange.X = ((CommonToken)token).StartIndex + endIndex;
                        }
                    }
                    if (endIndex >= 0)
                    {
                        buffer.Append(CDataEnd);
                        if (mDoFormat) InsertCR(buffer, false);
                    }
                    else
                    {
                        // If we're not sticking the end CDATA tag on, then just delete the whitespace at the end of the line
                        ASFormatter.TrimWhitespaceOnEndOfBuffer(buffer);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool IsASFormattingTag(string enclosingTagName)
        {
            if (mASScriptTags.Contains(enclosingTagName)) return true;
            foreach (string scriptTag in mASScriptTags)
            {
                if (Regex.Match(enclosingTagName, scriptTag).Success) return true;
            }
            return false;
        }

        private int CountLines(string text)
        {
            int count = 1;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\r')
                {
                    if (IsCarriageReturnPair(text, i)) i++;
                    count++;
                }
                else if (c == '\n') count++;
            }
            return count;
        }

        private bool IsCarriageReturnPair(string source, int loc)
        {
            if (loc + 1 < source.Length)
            {
                if (source[loc] == '\r' && source[loc + 1] == '\n') return true;
            }
            return false;
        }

        private void ProcessPCData(CommonToken token, StringBuilder buffer)
        {
            // If we are inside a CData section and the enclosing tag is not explicitly set to allow formatting,
            // then we just spit out the data.  I'm going to ignore the case of an empty CData section.
            if (!mTagsWhoseTextContentsCanBeFormatted.Contains(GetEnclosingTag()) && token.Text.Trim().StartsWith(CDataStart))
            {
                // Put start of tag on new line and indent it
                if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer)) InsertCR(buffer, false);
                AddIndentIfAtStartOfLine(buffer);
                //Spit out the text
                buffer.Append(token.Text.Replace("\r\n", "\n").Replace('\r', '\n'));
                // TODO: add a carriage return here if we can determine the correct behavior based on format/indent mode
                // and "keep blank lines"
                return;
            }
            if (mDoFormat)
            {
                string[] lines = token.Text.Trim().Split('\n');
                if (lines.Length == 0 && token.Text.Equals("\n")) lines = new string[] { "", "" };
                for (int k = 0; k < lines.Length; k++)
                {
                    string lineData = lines[k];
                    if (mDoFormat)
                    {
                        if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer)) InsertCR(buffer, false);
                        lineData = lineData.Trim();
                        if (lineData.Length == 0) continue;
                        AddIndentIfAtStartOfLine(buffer);
                        buffer.Append(lineData);
                        InsertCR(buffer, false);
                    }
                }
            }
            else
            {
                string text = token.Text;
                bool beforeTextOnLine = ASFormatter.IsOnlyWhitespaceOnLastLine(buffer);
                for (int k = 0; k < text.Length; k++)
                {
                    char c = text[k];
                    if (Char.IsWhiteSpace(c))
                    {
                        if (c == '\n' || c == '\r')
                        {
                            if (IsCarriageReturnPair(text, k)) k++; //advanced extra position to account for \r\n
                            buffer.Append('\n');
                            beforeTextOnLine = true;
                        }
                        else if (!beforeTextOnLine)
                        {
                            buffer.Append(c);
                        }
                    }
                    else
                    {
                        AddIndentIfAtStartOfLine(buffer);
                        beforeTextOnLine = false;
                        buffer.Append(c);
                    }
                }
            }
        }


        private void InsertCR(StringBuilder buffer, bool doOverride)
        {
            if (mDoFormat || doOverride)
            {
                buffer.Append('\n');
            }
        }

        public class Attr : IComparable<Attr>
        {
            public string mName;
            public string mValue;

            public Attr()
            {
                mName = "";
                mValue = "";
            }

            public int CompareTo(Attr other)
            {
                return (mName.CompareTo(other.mName));
            }

            public int HashCode()
            {
                return mName.GetHashCode();
            }
        }

        private void UpdatePartialFormattingBoundaries(CommonToken startToken, CommonToken endToken, StringBuilder buffer)
        {
            if (!mDoFormat) return;
            if (mSelectedRange != null)
            {
                if (mOutputRange == null)
                {
                    if (endToken.Line >= mSelectedRange.X)
                    {
                        mOutputRange = new Point(buffer.Length, -1);
                        mReplaceRange = new Point(0, -1);
                        mReplaceRange.X = ((CommonToken)startToken).StartIndex;
                    }
                }
                else
                {
                    if (mOutputRange.Y < 0 && startToken.Line > mSelectedRange.Y)
                    {
                        mOutputRange.Y = buffer.Length;
                        mReplaceRange.Y = ((CommonToken)startToken).StartIndex;
                    }
                }
            }
        }

        private int printTag(List<CommonToken> tokens, int tokenIndex, StringBuilder buffer, int stopType)
        {
            CommonToken startToken = tokens[tokenIndex];
            bool attrOrderChanged = false;
            if (mSelectedRange != null)
            {
                // Find end token, so I can determine whether I need to capture the formatting boundary at this point
                CommonToken endToken = tokens[tokenIndex];
                int endTokenIndex = tokenIndex;
                while (endTokenIndex < tokens.Count)
                {
                    CommonToken token = tokens[endTokenIndex];
                    if (token.Type == stopType)
                    {
                        endToken = token;
                        break;
                    }
                    endTokenIndex++;
                }
                UpdatePartialFormattingBoundaries(tokens[tokenIndex], endToken, buffer);
            }
            // If we may need to add some blank lines before this tag
            if (mDoFormat && mBlankLinesBeforeTags > 0 && (startToken.Type == MXMLLexer.EMPTY_TAG_OPEN || startToken.Type == MXMLLexer.TAG_OPEN))
            {
                int testTokenIndex = tokenIndex;
                while (testTokenIndex < tokens.Count)
                {
                    CommonToken token = tokens[testTokenIndex];
                    if (token.Type == MXMLLexer.GENERIC_ID)
                    {
                        if (mTagsWithBlankLinesBeforeThem.Contains(token.Text) || matchesRegEx(token.Text, mTagsWithBlankLinesBeforeThem))
                        {
                            // Count blank lines at end of current buffer to see if there are already some there
                            int emptyLinesAlreadyThere = ASFormatter.GetNumberOfEmptyLinesAtEnd(buffer);
                            int linesToAdd = GetBlankLinesBeforeTags() - emptyLinesAlreadyThere;
                            if (linesToAdd > 0)
                            {
                                if (emptyLinesAlreadyThere == 0 && !ASFormatter.IsOnlyWhitespaceOnLastLine(buffer)) linesToAdd++;
                                for (int i = 0; i < linesToAdd; i++)
                                {
                                    InsertCR(buffer, true);
                                }
                            }
                        }
                        break;
                    }
                    testTokenIndex++;
                }
            }
            AddIndentIfAtStartOfLine(buffer);
            int startOfTagInBuffer = buffer.Length; // We need this to be the point right where the first token gets added (it needs to match startToken)
            buffer.Append(tokens[tokenIndex].Text);
            tokenIndex++;
            List<Attr> attrs = null;
            if (mDoFormat && stopType != MXMLLexer.DECL_STOP) attrs = new List<Attr>();
            // bool seenTagName=false;
            string tagName = null;
            string currentAttrName = null;
            CommonToken endToken2 = null;
            string spaceString = ASFormatter.GenerateSpaceString(mSpacesAroundEquals);
            int extraWrappedLineIndent = 0;
            while (tokenIndex < tokens.Count)
            {
                // AddIndentIfAtStartOfLine(buffer);
                CommonToken token = tokens[tokenIndex];
                if (token.Type == stopType)
                {
                    endToken2 = token;
                    if (attrs == null)
                    {
                        AddIndentIfAtStartOfLine(buffer);
                        buffer.Append(token.Text);
                    }
                    break;
                }
                if (mDoFormat)
                {
                    switch (token.Type)
                    {
                        case MXMLLexer.EOL:
                        case MXMLLexer.WS:
                            // Ignore
                            break;

                        case MXMLLexer.EQ:
                            if (attrs == null)
                            {
                                buffer.Append(spaceString);
                                buffer.Append(token.Text);
                                buffer.Append(spaceString);
                            }
                            break;

                        case MXMLLexer.GENERIC_ID:
                        case MXMLLexer.XML:
                            if (attrs == null)
                            {
                                if (tagName != null) buffer.Append(' ');
                                buffer.Append(token.Text);
                            }
                            if (tagName == null) tagName = token.Text;
                            else currentAttrName = token.Text;
                            break;

                        case MXMLLexer.VALUE:
                            if (attrs != null)
                            {
                                if (currentAttrName != null)
                                {
                                    Attr a = new Attr();
                                    a.mName = currentAttrName;
                                    currentAttrName = null;
                                    a.mValue = token.Text;
                                    attrs.Add(a);
                                }
                            }
                            else buffer.Append(token.Text);
                            break;

                        default:
                            if (attrs == null) buffer.Append(token.Text);
                            break;
                    }
                }
                else
                {
                    switch (token.Type)
                    {
                        case MXMLLexer.EOL:
                            buffer.Append(token.Text);
                            break;

                        case MXMLLexer.WS:
                            // Only print if there's text on the line already
                            if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer)) buffer.Append(token.Text);
                            break;

                        default:
                            bool capturedTagName = false;
                            if (tagName == null)
                            {
                                tagName = token.Text;
                                capturedTagName = true;
                            }
                            AddIndentIfAtStartOfLine(buffer);
                            buffer.Append(token.Text);
                            if (capturedTagName)
                            {
                                if (GetWrapStyle() == WrapOptions.WRAP_STYLE_INDENT_TO_WRAP_ELEMENT)
                                {
                                    extraWrappedLineIndent = getLastLineColumnLength(buffer) + 1 - mCurrentIndent;
                                }
                                else extraWrappedLineIndent = GetIndentAmount();
                                mCurrentIndent += extraWrappedLineIndent;
                            }
                            break;
                    }
                }
                tokenIndex++;
            }
            mCurrentIndent -= extraWrappedLineIndent;
            mEnclosingTagName = tagName;
            // Spit out data here if we're formatting
            bool allAttrsArePartOfCustomOrdering = false;
            if (mDoFormat && attrs != null)
            {
                if (tagName != null)
                {
                    buffer.Append(tagName);
                }
                int originalAttrCount = attrs.Count;
                if (attrs.Count > 0)
                {
                    List<string> existingAttrs = new List<string>();
                    foreach (MXMLPrettyPrinter.Attr attr in attrs)
                    {
                        existingAttrs.Add(attr.mName);
                    }
                    buffer.Append(ASFormatter.GenerateSpaceString(1)); // TODO: configurable number of spaces before first attr
                    // Now, order attrs based on user settings
                    int sortStart = 0;
                    if (mAttrOrderMode == MXML_ATTR_ORDERING_USEDATA)
                    {
                        List<Attr> oldAttrOrder = new List<Attr>();
                        oldAttrOrder.AddRange(attrs);
                        List<Attr> newAttrOrder = new List<Attr>();
                        for (int i = 0; i < mManualAttrSortOrder.Count; i++)
                        {
                            string sortItem = mManualAttrSortOrder[i];
                            string[] attrItems = sortItem.Split(',');
                            bool missingAttr = false;
                            bool existingAttr = false;
                            foreach (string attr in attrItems)
                            {
                                string attr2 = attr.Trim();
                                if (attr2.Length == 0) continue;
                                if (!attr2.Equals(AttrGroup.NewLineFlag))
                                {
                                    if (!setsIntersect(existingAttrs, attr2)) missingAttr = true;
                                    else existingAttr = true;
                                }
                            }
                            // If we found at least one attr or none were missing
                            if (existingAttr || !missingAttr)
                            {
                                foreach (string attr in attrItems)
                                {
                                    string attr2 = attr.Trim();
                                    if (attr.Length == 0) continue;
                                    if (attr2.Equals(AttrGroup.NewLineFlag))
                                    {
                                        Attr newLineAttr = new Attr();
                                        newLineAttr.mName = AttrGroup.NewLineFlag;
                                        newAttrOrder.Add(newLineAttr);
                                    }
                                    else
                                    {
                                        string groupName = IsGroupAttr(attr2);
                                        if (groupName != null)
                                        {
                                            AttrGroup group = mAttrGroups[groupName];
                                            if (group != null)
                                            {
                                                List<MXMLPrettyPrinter.Attr> attrsForGroup = new List<Attr>();
                                                switch (group.GetSortMode())
                                                {
                                                    case MXMLPrettyPrinter.MXML_Sort_AscByCase:
                                                    case MXMLPrettyPrinter.MXML_Sort_None:
                                                        // In this case, we want to find items in the tag and keep
                                                        // them in that order
                                                        List<string> groupAttrSet = mHashedGroupAttrs[group.GetName()];
                                                        if (groupAttrSet != null)
                                                        {
                                                            for (int k = attrs.Count - 1; k >= 0; k--)
                                                            {
                                                                string attrName = attrs[k].mName;
                                                                bool matchFound = groupAttrSet.Contains(attrName);
                                                                if (!matchFound)
                                                                {
                                                                    // Check for regular expressions matching attr
                                                                    foreach (string regexAttr in group.GetRegexAttrs())
                                                                    {
                                                                        bool matches = Regex.Match(attrName, regexAttr).Success;
                                                                        if (matches)
                                                                        {
                                                                            matchFound = true;
                                                                            break;
                                                                        }
                                                                    }
                                                                }
                                                                if (matchFound)
                                                                {
                                                                    attrsForGroup.Add(attrs[k]);
                                                                    attrs.RemoveAt(k);
                                                                    existingAttrs.Remove(attrName);
                                                                }
                                                            }
                                                            if (group.GetSortMode() == MXMLPrettyPrinter.MXML_Sort_AscByCase)
                                                            {
                                                                attrsForGroup.Sort();
                                                            }
                                                        }
                                                        break;

                                                    case MXMLPrettyPrinter.MXML_Sort_GroupOrder:
                                                        // This one needs to be done in reverse: walk the group items and find items
                                                        // that match and keep them in group order
                                                        foreach (string attribute in group.GetAttrs())
                                                        {
                                                            migrateAttrToList(attrsForGroup, attrs, existingAttrs, attribute);
                                                        }
                                                        break;
                                                }
                                                if (attrsForGroup.Count > 0)
                                                {
                                                    //add wrap mode
                                                    int wrapMode = group.GetWrapMode();
                                                    if (wrapMode == MXMLPrettyPrinter.MXML_ATTR_WRAP_DEFAULT) wrapMode = GetWrapMode();
                                                    Attr wrapAttr = new Attr();
                                                    wrapAttr.mName = "<Wrap=" + wrapMode + ">";
                                                    newAttrOrder.Add(wrapAttr);
                                                    newAttrOrder.AddRange(attrsForGroup);
                                                    wrapAttr = new Attr();
                                                    wrapAttr.mName = "</Wrap>";
                                                    newAttrOrder.Add(wrapAttr);
                                                }
                                            }
                                        }
                                        else migrateAttrToList(newAttrOrder, attrs, existingAttrs, attr);
                                    }
                                }
                            }
                        }
                        if (attrs.Count == 0)
                        {
                            // Remove extra newlines at end
                            while (newAttrOrder.Count > 0)
                            {
                                Attr attr = newAttrOrder[newAttrOrder.Count - 1];
                                if (attr.mName.Equals(AttrGroup.NewLineFlag))
                                {
                                    newAttrOrder.RemoveAt(newAttrOrder.Count - 1);
                                    allAttrsArePartOfCustomOrdering = true;
                                }
                                else break;
                            }
                        }
                        sortStart = newAttrOrder.Count;
                        newAttrOrder.AddRange(attrs);
                        if (mSortOtherAttrs)
                        {
                            if (sortStart < newAttrOrder.Count - 1)
                            {
                                newAttrOrder.Sort(); // subList(sortStart, newAttrOrder.Count));
                            }
                        }
                        // Compare old attr order and new attr order and see if the attributes are in the same order
                        for (int i = 0, j = 0; i < oldAttrOrder.Count && j < newAttrOrder.Count; )
                        {
                            // Skip newlines and other meta flags
                            string newAttrName = newAttrOrder[j].mName;
                            if (newAttrName.Equals(AttrGroup.NewLineFlag) || newAttrName.StartsWith("<"))
                            {
                                j++;
                                continue;
                            }
                            if (!newAttrOrder[j].mName.Equals(oldAttrOrder[i].mName))
                            {
                                attrOrderChanged = true;
                                break;
                            }
                            i++;
                            j++;
                        }
                        // Replace attributes with the custom ordering
                        attrs = newAttrOrder;
                    }
                }
                // Find last hard newline in attr list (some options only apply after the last newline
                int lastNewLine = (-1);
                if (allAttrsArePartOfCustomOrdering) lastNewLine = attrs.Count;
                int lastAttr = (-1);
                for (int j = 0; j < attrs.Count; j++)
                {
                    Attr attr = attrs[j];
                    if (!allAttrsArePartOfCustomOrdering && attr.mName.Equals(AttrGroup.NewLineFlag)) lastNewLine = j;
                    if (!attr.mName.StartsWith("<")) lastAttr = j;
                }
                // Add the ending newline if we  need it
                if (mAttrOrderMode == MXML_ATTR_ORDERING_USEDATA && mAddNewlineAfterLastAttr && attrs.Count > 0)
                {
                    Attr newLineAttr = new Attr();
                    newLineAttr.mName = AttrGroup.NewLineFlag;
                    attrs.Add(newLineAttr);
                }
                int firstIndent = (-1);
                int attrsOnLine = 0;
                int wrapMode2 = MXML_ATTR_WRAP_NONE;
                bool inGroup = false;
                if (lastNewLine < 0) wrapMode2 = GetWrapMode();
                bool disableWrapping = false;
                if (IsUseAttrsToKeepOnSameLine() && originalAttrCount <= GetAttrsToKeepOnSameLine())
                {
                    disableWrapping = true;
                }
                for (int j = 0; j < attrs.Count; j++)
                {
                    Attr attr = attrs[j];
                    // Check for a group wrap mode
                    if (attr.mName.StartsWith("<"))
                    {
                        if (disableWrapping) continue;
                        wrapMode2 = (j < lastNewLine) ? MXML_ATTR_WRAP_NONE : GetWrapMode();
                        wrapMode2 = GetWrapMode(); //set back to default until I determine otherwise
                        if (attr.mName.StartsWith("<Wrap="))
                        {
                            inGroup = true;
                            string modeString = attr.mName.Substring("<Wrap=".Length, attr.mName.Length - 1);
                            wrapMode2 = Convert.ToInt32(modeString);
                        }
                        else if (attr.mName.Equals("</Wrap>"))
                        {
                            inGroup = false;
                        }
                        continue;
                    }
                    // Establish indent first
                    if (firstIndent < 0)
                    {
                        if (GetWrapStyle() == WrapOptions.WRAP_STYLE_INDENT_TO_WRAP_ELEMENT) firstIndent = getLastLineColumnLength(buffer);
                        else firstIndent = mCurrentIndent + GetIndentAmount();
                    }
                    bool isNewline = attr.mName.Equals(AttrGroup.NewLineFlag);
                    // Go ahead and precalculate the string for the attribute/value pair
                    StringBuilder attrString = new StringBuilder();
                    if (!isNewline)
                    {
                        attrString.Append(attr.mName);
                        attrString.Append(spaceString);
                        attrString.Append('=');
                        attrString.Append(spaceString);
                        attrString.Append(attr.mValue);
                    }
                    bool justWrapped = false;
                    if (!disableWrapping && j <= lastAttr && wrapMode2 != MXML_ATTR_WRAP_NONE)
                    {
                        if (wrapMode2 == MXML_ATTR_WRAP_COUNT_PER_LINE)
                        {
                            if (attrsOnLine > 0 && attrsOnLine >= GetAttrsPerLine())
                            {
                                if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
                                {
                                    buffer.Append('\n');
                                    buffer.Append(GenerateIndent(firstIndent));
                                    attrsOnLine = 0;
                                    justWrapped = true;
                                }
                            }
                        }
                        else if (wrapMode2 == MXML_ATTR_WRAP_LINE_Length && !isNewline)
                        {
                            // We'll add a line break if the next attribute will push the Length beyond max, *unless* we're
                            // already on a new line, in which case we just go ahead and stick the text on this line.
                            int currentLineLength = ASPrettyPrinter.DetermineLastLineLength(buffer, GetTabSize());
                            if (currentLineLength + attrString.Length >= mMaxLineLength)
                            {
                                if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
                                {
                                    buffer.Append('\n');
                                    buffer.Append(GenerateIndent(firstIndent));
                                    attrsOnLine = 0;
                                    justWrapped = true; //I don't think this needs to be here now, because we're checking for isNewline in the 'if'
                                }
                            }
                        }
                    }
                    if (isNewline)
                    {
                        if (disableWrapping) continue;
                        // Reset the default wrap mode if we are beyond the end of the custom wrap mode
                        if (j >= lastNewLine && !inGroup) wrapMode2 = GetWrapMode();
                        if (justWrapped) // Don't add another carriage return if one was added via wrapping
                        {
                            // wrappedLastIteration=false;
                            continue;
                        }
                        buffer.Append('\n');
                        attrsOnLine = 0;
                        buffer.Append(GenerateIndent(firstIndent));
                        continue;
                    }
                    // wrappedLastIteration=false;
                    if (buffer.Length > 0 && !ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
                    {
                        if (buffer[buffer.Length - 1] != ' ') buffer.Append(ASFormatter.GenerateSpaceString(1)); // TODO: configurable number of spaces between attrs?
                    }
                    buffer.Append(attrString);
                    attrsOnLine++;
                }
                // TODO: print configurable number of spaces before tag end?
                AddIndentIfAtStartOfLine(buffer);
                if (endToken2 != null)
                {
                    if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
                    {
                        if (endToken2.Text.Equals("/>"))
                        {
                            buffer.Append(ASFormatter.GenerateSpaceString(GetSpacesBeforeEmptyTagEnd()));
                        }
                    }
                    buffer.Append(endToken2.Text);
                }
                if (attrOrderChanged)
                {
                    if (mReplaceMap == null) mReplaceMap = new Dictionary<int, ReplacementRange>();
                    ReplacementRange range = new ReplacementRange(new Point(startOfTagInBuffer, buffer.Length), new Point(startToken.StartIndex, endToken2.StopIndex + 1));
                    mReplaceMap.Add(startOfTagInBuffer, range);
                }
            }
            return tokenIndex;
        }


        private bool matchesRegEx(string text, List<string> tagsWithBlankLinesBeforeThem)
        {
            foreach (string tag in tagsWithBlankLinesBeforeThem)
            {
                if (AttrGroup.IsRegexString(tag))
                {
                    if (Regex.Match(text, tag).Success) return true;
                }
            }
            return false;
        }

        private void migrateAttrToList(List<Attr> newAttrOrder, List<Attr> attrs, List<string> existingAttrs, string attr)
        {
            if (existingAttrs.Contains(attr))
            {
                for (int k = 0; k < attrs.Count; k++)
                {
                    if (attrs[k].mName.Equals(attr))
                    {
                        existingAttrs.Remove(attr);
                        Attr item = attrs[k];
                        attrs.RemoveAt(k);
                        newAttrOrder.Add(item);
                        return;
                    }
                }
            }
            // Otherwise, try as regex string
            if (AttrGroup.IsRegexString(attr))
            {
                List<Attr> tempNewAttrs = new List<Attr>();
                for (int k = 0; k < attrs.Count; k++)
                {
                    bool matches = Regex.Match(attrs[k].mName, attr).Success;
                    if (matches)
                    {
                        existingAttrs.Remove(attr);
                        Attr item = attrs[k];
                        attrs.RemoveAt(k);
                        k--;
                        tempNewAttrs.Add(item);
                    }
                }
                bool sort = true;
                if (sort) tempNewAttrs.Sort();
                newAttrOrder.AddRange(tempNewAttrs);
            }
        }

        private bool setsIntersect(List<string> existingAttrs, string attr)
        {
            string groupName = IsGroupAttr(attr);
            if (groupName != null)
            {
                AttrGroup group = mAttrGroups[groupName];
                if (group == null) return false;
                // Check each attr in group to see if it is one of the existing attrs
                foreach (string att in group.GetAttrs())
                {
                    if (existingAttrs.Contains(att)) return true;
                }
            }
            else return existingAttrs.Contains(attr);
            return false;
        }

        private int getLastLineColumnLength(StringBuilder buffer)
        {
            int lastCR = buffer.ToString().LastIndexOf('\n');
            string lastLine = null;
            if (lastCR < 0) lastLine = buffer.ToString();
            else lastLine = buffer.ToString().Substring(lastCR + 1);
            int columnCount = 0;
            for (int i = 0; i < lastLine.Length; i++)
            {
                char c = lastLine[i];
                if (c == '\t')
                {
                    int remainder = columnCount % GetTabSize();
                    if (remainder == 0) columnCount += GetTabSize();
                    else columnCount += remainder;
                }
                else columnCount++;
            }
            return columnCount;
        }

        public void AddIndentIfAtStartOfLine(StringBuilder buffer)
        {
            if (!mSkipNextIndent && ASFormatter.IsLineEmpty(buffer))
            {
                buffer.Append(GenerateIndent(mCurrentIndent));
            }
            mSkipNextIndent = false;
        }

        private string GenerateIndent(int spaces)
        {
            return ASFormatter.GenerateIndent(spaces, mUseTabs, mTabSize);
        }

        public List<Exception> GetParseErrors()
        {
            return mParseErrors;
        }

        public void SetDoFormat(bool b)
        {
            mDoFormat = b;
        }

        public ASPrettyPrinter GetASPrinter()
        {
            return mASPrinter;
        }

        public int GetIndentAmount()
        {
            return mIndentAmount;
        }

        public void SetIndentAmount(int indentAmount)
        {
            mIndentAmount = indentAmount;
        }

        public int GetSpacesAroundEquals()
        {
            return mSpacesAroundEquals;
        }

        public void SetSpacesAroundEquals(int spacesAroundEquals)
        {
            mSpacesAroundEquals = spacesAroundEquals;
        }

        public bool IsSortOtherAttrs()
        {
            return mSortOtherAttrs;
        }

        public void SetSortOtherAttrs(bool sortOtherAttrs)
        {
            mSortOtherAttrs = sortOtherAttrs;
        }

        public void SetAttrSortMode(int sortMode)
        {
            mAttrOrderMode = sortMode;
        }

        public int GetAttrSortMode()
        {
            return mAttrOrderMode;
        }

        public void SetManualAttrSortData(List<string> attrOrder)
        {
            if (attrOrder == null) mManualAttrSortOrder.Clear();
            else mManualAttrSortOrder = attrOrder;
        }

        public List<string> GetManualAttrSortData()
        {
            return mManualAttrSortOrder;
        }

        public int GetMaxLineLength()
        {
            return mMaxLineLength;
        }

        public void SetMaxLineLength(int maxLineLength)
        {
            mMaxLineLength = maxLineLength;
        }

        public Point GetSelectedRange()
        {
            return mSelectedRange;
        }

        public void SetSelectedRange(Point selectedRange)
        {
            mSelectedRange = selectedRange;
        }

        public Point GetOutputRange()
        {
            return mOutputRange;
        }

        public Point GetReplaceRange()
        {
            return mReplaceRange;
        }

        public int GetAttrsPerLine()
        {
            return mAttrsPerLine;
        }

        public void SetAttrsPerLine(int attrsPerLine)
        {
            mAttrsPerLine = attrsPerLine;
        }

        public int GetWrapMode()
        {
            return mWrapMode;
        }

        public void SetWrapMode(int wrapMode)
        {
            mWrapMode = wrapMode;
        }

        public bool IsKeepBlankLines()
        {
            return mKeepBlankLines;
        }

        public void SetKeepBlankLines(bool keepBlankLines)
        {
            mKeepBlankLines = keepBlankLines;
        }

        public int GetWrapStyle()
        {
            return mWrapStyle;
        }

        public void SetWrapStyle(int style)
        {
            mWrapStyle = style;
        }

        public void SetTagsThatCanBeFormatted(List<string> tagNames)
        {
            mTagsWhoseTextContentsCanBeFormatted.Clear();
            mTagsWhoseTextContentsCanBeFormatted.AddRange(tagNames);
        }

        public List<string> GetTagsThatCanBeFormatted()
        {
            return mTagsWhoseTextContentsCanBeFormatted;
        }

        public void SetTagsThatCannotBeFormatted(List<string> tagNames)
        {
            mTagsWhoseTextContentCanNeverBeFormatted.Clear();
            mTagsWhoseTextContentCanNeverBeFormatted.AddRange(tagNames);
        }

        public List<string> GetTagsThatCannotBeFormatted()
        {
            return mTagsWhoseTextContentCanNeverBeFormatted;
        }

        public void SetAttrGroups(List<AttrGroup> attrGroups)
        {
            mHashedGroupAttrs = new Dictionary<string, List<string>>();
            mAttrGroups = new Dictionary<string, AttrGroup>();
            foreach (AttrGroup group in attrGroups)
            {
                mAttrGroups.Add(group.GetName(), group);
                List<string> attrSet = new List<string>();
                foreach (string attr in group.GetAttrs())
                {
                    attrSet.Add(attr);
                }
                mHashedGroupAttrs.Add(group.GetName(), attrSet);
            }
        }

        public static string IsGroupAttr(string attr)
        {
            if (attr.Length >= 2 && attr.StartsWith("" + AttrGroup.Attr_Group_Marker) && attr.EndsWith("" + AttrGroup.Attr_Group_Marker))
            {
                return attr.Substring(1, attr.Length - 1);
            }
            return null;
        }

        public void SetAddNewlineAfterLastAttr(bool addNewlineAfterLastAttr)
        {
            mAddNewlineAfterLastAttr = addNewlineAfterLastAttr;
        }

        public Dictionary<int, ReplacementRange> GetReplaceMap()
        {
            return mReplaceMap;
        }

        public bool IsUseAttrsToKeepOnSameLine()
        {
            return mUseAttrsToKeepOnSameLine;
        }

        public void SetUseAttrsToKeepOnSameLine(bool useAttrsToKeepOnSameLine)
        {
            mUseAttrsToKeepOnSameLine = useAttrsToKeepOnSameLine;
        }

        public int GetAttrsToKeepOnSameLine()
        {
            return mAttrsToKeepOnSameLine;
        }

        public void SetAttrsToKeepOnSameLine(int attrsToKeepOnSameLine)
        {
            mAttrsToKeepOnSameLine = attrsToKeepOnSameLine;
        }

        public int GetSpacesBeforeEmptyTagEnd()
        {
            return mSpacesBeforeEmptyTagEnd;
        }

        public void SetSpacesBeforeEmptyTagEnd(int spacesBeforeEmptyTagEnd)
        {
            mSpacesBeforeEmptyTagEnd = spacesBeforeEmptyTagEnd;
        }

        public List<string> GetTagsWithBlankLinesBeforeThem()
        {
            return mTagsWithBlankLinesBeforeThem;
        }

        public void SetTagsWithBlankLinesBeforeThem(List<string> tagsWithBlankLinesBeforeThem)
        {
            mTagsWithBlankLinesBeforeThem.Clear();
            mTagsWithBlankLinesBeforeThem.AddRange(tagsWithBlankLinesBeforeThem);
        }

        public int GetBlankLinesBeforeTags()
        {
            return mBlankLinesBeforeTags;
        }

        public void SetBlankLinesBeforeTags(int blankLinesBeforeTags)
        {
            mBlankLinesBeforeTags = blankLinesBeforeTags;
        }

        public List<string> GetASScriptTags()
        {
            return mASScriptTags;
        }

        public void SetASScriptTags(List<string> scriptTags)
        {
            mASScriptTags.Clear();
            mASScriptTags.AddRange(scriptTags);
        }

        public bool IsRequireCDATAForASContent()
        {
            return mRequireCDATAForASContent;
        }

        public void SetRequireCDATAForASContent(bool requireCDATAForASContent)
        {
            mRequireCDATAForASContent = requireCDATAForASContent;
        }

    }

}
