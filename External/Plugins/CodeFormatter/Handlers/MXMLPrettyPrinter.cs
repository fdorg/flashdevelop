using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Antlr.Runtime;
using CodeFormatter.InfoCollector;
using CodeFormatter.Preferences;
using PluginCore;

#pragma warning disable 414

namespace CodeFormatter.Handlers
{
    public class MXMLPrettyPrinter
    {
        public const int MXML_ATTR_ORDERING_NONE = 0;
        //public static int MXML_ATTR_ORDERING_SORT=1;
        public const int MXML_ATTR_ORDERING_USEDATA = 2;

        public const int MXML_ATTR_WRAP_LINE_LENGTH = 51;
        public const int MXML_ATTR_WRAP_COUNT_PER_LINE = 52;
        public const int MXML_ATTR_WRAP_NONE = 53;
        public const int MXML_ATTR_WRAP_DEFAULT = 54; //for groups; same as main setting

        public const int MXML_Sort_AscByCase = 11;
        public const int MXML_Sort_AscNoCase = 12;
        public const int MXML_Sort_GroupOrder = 13;
        public const int MXML_Sort_None = 14;

        public const String CDataEnd = "]]>";
        public const String CDataStart = "<![CDATA[";

        public const String StateRegexSuffix = "\\..*";

        public const String Attr_Group_Marker = "%";
        public const String Attr_Grouping_Splitter = ",";
        public const String Attr_Group_Other="Special_Group--Other Attributes";

        public const String NewLineFlag = "\\n";

        private int mAttrOrderMode = MXML_ATTR_ORDERING_NONE;
        private int mIndentAmount;
        private String mSource;
        private int mCurrentIndent;
        private int mMaxLineLength;
        private int mAttrsPerLine;
        private int mWrapMode;
        private int mWrapStyle;
        private int mHangingIndentSize;
        private bool mUseSpacesInsideAttrBraces = false; //by default, don't affect the spacing around braces
        private int mSpacesInsideAttrBraces = 0;
        private bool mFormatBoundAttributes = false;
        private bool mKeepRelativeCommentIndent = true;

        //private IPreferenceStore mStore;

        private Dictionary<Int32, ReplacementRange> mReplaceMap;

        private bool mSkipNextIndent;
        private List<TagStackEntry> mTagStack;

        private Point mSelectedRange; //x=start line, y=end line (1-based)(lines 5 and 6 would be 5,6)
        private Point mOutputRange; //x=start offset, y=end offset (0-based offsets into outputbuffer)
        private Point mReplaceRange; //same semantics as mSelectedRange

        private bool mIsPlainXML;

        private bool mRearrangeOnly = false;

        private bool mAllowMultiplePasses = true;
        private bool mNeedAnotherPass = false;
        private int mLastCommentStart = -1;

        private bool mKeepCDataOnSameLine = false;
        private int mCDATAIndentTabs = 0;
        private int mScriptIndentTabs = 0;
        private int mSpacesBetweenSiblingTags = 0;
        private int mSpacesAfterParentTags = 0;
        private int mSpacesBeforeEmptyTagEnd = 0;
        private bool mKeepBlankLines = true;
        private bool mDoFormat;
        private bool mSortOtherAttrs = true;
        private bool mAddNewlineAfterLastAttr = false;
        private bool mIndentCloseTag = true;
        private List<String> mManualAttrSortOrder;
        private Dictionary<String, AttrGroup> mAttrGroups;
        private int mSpacesAroundEquals = 1;
        private bool mUseTabs;
        private int mTabSize;
        private String mEnclosingTagName; //holds name of last open tag temporarily.  Use getEnclosingTag() for the correct data
        private List<String> mTagsWhoseTextContentsCanBeFormatted;
        private List<String> mTagsWhoseTextContentCanNeverBeFormatted;
        private List<String> mTagsWithBlankLinesBeforeThem;
        private List<String> mParentTagsWithBlankLinesAfterThem = new List<String>();

        private bool mUsePrivateTags; //use the list of tags to ignore internal formatting
        private List<String> mPrivateTags; //list of tags whose internal formatting shouldn't be touched

        private List<String> mASScriptTags;
        private bool mRequireCDATAForASContent;

        private int mBlankLinesAtCDataStart = (-1);
        private int mBlankLinesAtCDataEnd = (-1);
        private int mBlankLinesBeforeComments = 0;
        private int mBlankLinesBeforeTags = 0;
        private int mBlankLinesAfterParentTags = 0;
        private int mBlankLinesBeforeCloseTags = 0;
        private bool mUseAttrsToKeepOnSameLine = false;
        private int mAttrsToKeepOnSameLine = 4;
        private bool mAlwaysObeyMaxLineLength = false;
        //private String mResumeFormattingTag=null;

        private String mAddedText;
        private String mRemovedText;

        private Dictionary<String, List<String>> mHashedGroupAttrs = new Dictionary<String, List<String>>();

        public bool isUseTabs()
        {
            return mUseTabs;
        }

        public void setUseTabs(bool useTabs)
        {
            mUseTabs = useTabs;
        }

        public int getTabSize()
        {
            return mTabSize;
        }

        public void setTabSize(int tabSize)
        {
            mTabSize = tabSize;
        }

        private List<Exception> mParseErrors;
        private ASPrettyPrinter mASPrinter;

        private void initialize()
        {
            mASPrinter = new ASPrettyPrinter(false, "");
            mManualAttrSortOrder = new List<String>();
            mIndentAmount = 4;
            mDoFormat = true;
            mWrapStyle = WrapOptions.WRAP_STYLE_INDENT_NORMAL;
            mTagsWhoseTextContentsCanBeFormatted = new List<String>();
            mTagsWhoseTextContentsCanBeFormatted.Add("mx:List");
            mTagsWhoseTextContentsCanBeFormatted.Add("fx:List");
            mTagsWhoseTextContentCanNeverBeFormatted = new List<String>();
            mTagsWhoseTextContentCanNeverBeFormatted.Add("mx:String");
            mTagsWhoseTextContentCanNeverBeFormatted.Add("fx:String");
            mTagsWithBlankLinesBeforeThem = new List<String>();
            mASScriptTags = new List<String>();
            mASScriptTags.Add(".*:Script");
            mASScriptTags.Add("fx:Script");
            mASScriptTags.Add("mx:Script");
            mRequireCDATAForASContent = false;
            mIsPlainXML = false;
        }

        /*public MXMLPrettyPrinter(File f, String charsetName)
        {
            initialize();
            StringBuffer buffer=new StringBuffer();
            BufferedReader br=new BufferedReader(new InputStreamReader(new FileInputStream(f), charsetName));
            try
            {
                while (true)
                {
                    String line=br.readLine();
                    if (line==null)
                        break;
                    buffer += line);
                    buffer += '\n');
                }
            }
            finally
            {
                br.close();
            }
            mSource=buffer.toString();
        }*/

        public MXMLPrettyPrinter(String sourceData)
        {
            initialize();
            mSource = sourceData;
        }

        public String print(int startIndent)
        {
            if (mSource.IndexOfOrdinal(ASPrettyPrinter.mIgnoreFileProcessing) >= 0)
            {
                mParseErrors = new List<Exception>();
                mParseErrors.Add(new Exception("File ignored: Ignore tag exists in file==> " + ASPrettyPrinter.mIgnoreFileProcessing));
                return null;
            }

            //Note: no need to loop here at present, because the mxml stuff never requires more than one pass
            mTagStack = new List<TagStackEntry>();
            mSkipNextIndent = false;
            mNeedAnotherPass = false;
            mAddedText = "";
            mRemovedText = "";
            mSource = InfoCollector.Utilities.convertCarriageReturnsToLineFeeds(mSource);
            mCurrentIndent = startIndent;
            mReplaceMap = null;
            mLastCommentStart = -1;
            //mReplaceRange = null;
            //mOutputRange = null;
            MXMLLexer lex = new MXMLLexer(new ANTLRStringStream(mSource));
            StringBuilder buffer = new StringBuilder();
            lex.mDOCUMENT();
            prettyPrint(lex.GetTokens(), buffer);
            if (!(ASFormatter.validateNonWhitespaceCharCounts(buffer + mRemovedText, mSource + mAddedText)))
            {
                if (mParseErrors == null)
                    mParseErrors = new List<Exception>();
                mParseErrors.Add(new Exception("Internal error: Formatted text doesn't match source. " + buffer + "!=" + mSource));
                return null;
            }
            return buffer.ToString();
        }

        private TagStackEntry getCurrentTag()
        {
            if (mTagStack.Count == 0)
                return null;

            return mTagStack[mTagStack.Count - 1];
        }

        private String getEnclosingTag()
        {
            if (mTagStack.Count == 0)
                return "";

            return mTagStack[mTagStack.Count - 1].getTagName();
        }

        private int getPCDataTokens(List<CommonToken> tokens, int tokenIndex, List<IToken> pcDataTokens, bool[] foundNonWhitespace)
        {
            foundNonWhitespace[0] = false;
            int newTokenIndex = tokenIndex + 1;
            while (true)
            {
                if (newTokenIndex >= tokens.Count)
                    break;

                IToken token = tokens[newTokenIndex];
                if (token.Type == MXMLLexer.PCDATA || token.Type == MXMLLexer.EOL || token.Type == MXMLLexer.WS)
                {
                    pcDataTokens.Add(token);
                    if (AntlrUtilities.asTrim(token.Text).Length > 0)
                        foundNonWhitespace[0] = true;
                }
                else
                {
                    //not a content type we are handling
                    break;
                }

                newTokenIndex++;
            }

            newTokenIndex--; //back up 1 since the main token loop will increment
            return newTokenIndex;
        }

        private int processPostTagText(List<CommonToken> tokens, int tokenIndex, StringBuilder buffer, int linesToInsert)
        {
            //if this is an actionscript tag, then we need to grab all of the next contents and 
            //run it through the parser, if there is not a CData tag
            if (!isPlainXML() && isASFormattingTag(getEnclosingTag()))
            {
                bool[] hasChars = new bool[1];
                List<IToken> pcDataTokens = new List<IToken>();
                int newTokenIndex = getPCDataTokens(tokens, tokenIndex, pcDataTokens, hasChars);
                if (pcDataTokens.Count > 0)
                {
                    IToken tempToken = new CommonToken(pcDataTokens[0]);
                    String tokenText = "";
                    foreach (IToken token in pcDataTokens)
                    {
                        tokenText += token.Text;
                    }
                    String tokenString = tokenText;
                    if (AntlrUtilities.asTrim(tokenString).Length > 0)
                    {
                        tempToken.Text = tokenString;
                        bool success = processActionScriptBlock(tempToken, buffer);
                        if (success)
                            return newTokenIndex;
                    }
                }
            }

            bool foundNonWhitespace = false;

            //if this is not a tag whose contents can be formatted, then we need to peek at the 
            //subsequent pcdata/whitespace tokens to see if there are any non-whitespace contents
            if (!mTagsWhoseTextContentsCanBeFormatted.Contains(getEnclosingTag()))
            {
                bool[] hasChars = new bool[1];
                List<IToken> pcDataTokens = new List<IToken>();
                int newTokenIndex = getPCDataTokens(tokens, tokenIndex, pcDataTokens, hasChars);
                foundNonWhitespace = hasChars[0];

                //if we found characters, indicating that we must preserve the current formatting exactly,
                //or if this is a tag where even whitespace is significant, then output the tokens as 
                //is.
                if (foundNonWhitespace || mTagsWhoseTextContentCanNeverBeFormatted.Contains(getEnclosingTag()))
                {
                    foreach (IToken pcdataToken in pcDataTokens)
                    {
                        //convert to \n delimiters to match the rest of my output
                        String data = pcdataToken.Text.Replace("\r\n", "\n");
                        data = data.Replace("\r", "\n");
                        buffer.Append(data);
                    }
                    mSkipNextIndent = true;
                    return newTokenIndex;
                }
            }

            //we will drop through to here if 
            //1. this is a tag that explicitly allows indenting of text content, or
            //2. there is only whitespace in the text content, and this tag is not explicitly barred from formatting
            for (int i = 0; i < linesToInsert; i++)
            {
                insertCR(buffer, false);
            }

            return tokenIndex;
        }

        private void prettyPrint(List<CommonToken> tokens, StringBuilder buffer)
        {
            for (int tokenIndex = 0; tokenIndex < tokens.Count; tokenIndex++)
            {
                IToken token = tokens[tokenIndex];
                //System.out.println(token.Text)+":"+token.Type);
                switch (token.Type)
                {
                    case MXMLLexer.COMMENT:
                        mLastCommentStart = buffer.Length;
                        if (mRearrangeOnly)
                        {
                            buffer.Append(token.Text);
                        }
                        else
                        {
                            updatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                            String[] commentLines = token.Text.Split('\n');

                            //add extra blank lines here if the comment starts on a new line.  First, count existing
                            //blank lines.
                            if (mDoFormat && ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                            {
                                int currentBlankLines = ASFormatter.getNumberOfEmptyLinesAtEnd(buffer);
                                for (int i = currentBlankLines; i < getBlankLinesBeforeComments(); i++)
                                {
                                    buffer.Append('\n');
                                }
                            }

                            //reset this value because we might have added some additional blank lines
                            mLastCommentStart = buffer.Length;

                            int originalIndent = 0;
                            if (isKeepRelativeCommentIndent())
                            {
                                //find original indent
                                String lineData = "";
                                int currentLine = token.Line;
                                IToken aToken = token;
                                int tempTokenIndex = tokenIndex;
                                while (aToken.Line == currentLine)
                                {
                                    lineData.Insert(0, aToken.Text);
                                    if (tempTokenIndex <= 0)
                                        break;
                                    tempTokenIndex--;
                                    aToken = tokens[tempTokenIndex];
                                }
                                originalIndent = ASPrettyPrinter.findIndent(lineData, getTabSize());
                            }
                            for (int j = 0; j < commentLines.Length; j++)
                            {
                                bool onLastLine = (j == commentLines.Length - 1);
                                int indentAmount = mCurrentIndent;
                                String data = AntlrUtilities.asTrim(commentLines[j]);
                                if (j > 0)
                                {
                                    if (isKeepRelativeCommentIndent() && originalIndent >= 0)
                                    {
                                        int existingIndent = ASPrettyPrinter.findIndent(commentLines[j], getTabSize());
                                        indentAmount = Math.Max(0, mCurrentIndent + (existingIndent - originalIndent));
                                    }
                                    else
                                    {
                                        //on a middle line, indent the text to the right of the <!-- .
                                        if (!onLastLine || !data.StartsWithOrdinal("-->"))
                                            indentAmount += 5;
                                    }
                                }

                                //only add indent if on an empty line
                                if (data.Length > 0)
                                {
                                    if (ASFormatter.isLineEmpty(buffer))
                                    {
                                        buffer.Append(generateIndent(indentAmount));
                                    }
                                    buffer.Append(data);
                                }
                                if (!onLastLine)
                                    buffer.Append('\n');
                            }
                        }

                        tokenIndex = processPostTagText(tokens, tokenIndex, buffer, 1);
                        //                  insertCR(buffer, false);
                        break;
                    case MXMLLexer.DECL_START:
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.DECL_STOP);
                        insertCR(buffer, false);
                        break;
                    case MXMLLexer.TAG_OPEN:
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.TAG_CLOSE);
                        if (mEnclosingTagName != null)
                            mTagStack.Add(new TagStackEntry(mEnclosingTagName));
                        mCurrentIndent += mIndentAmount;
                        if (isPrivateTag(mEnclosingTagName))
                        {
                            tokenIndex++;
                            //walk and output item until I see the matching close tag.  Note: one side-effect of this 
                            //code is that unmatched tags inside this tag are legal.  
                            while (tokenIndex < tokens.Count)
                            {
                                IToken testToken = tokens[tokenIndex];
                                //see if this token is then end tag that matches our start tag
                                if (testToken.Type == MXMLLexer.END_TAG_OPEN)
                                {
                                    IToken nextNonWS = getNextNonWSToken(tokens, tokenIndex + 1);
                                    if (nextNonWS != null && nextNonWS.Text == mEnclosingTagName)
                                    {
                                        tokenIndex--;
                                        break;
                                    }
                                }
                                //otherwise emit the tag data and continue
                                buffer.Append(testToken.Text);
                                tokenIndex++;
                            }
                        }
                        else
                        {
                            tokenIndex = processPostTagText(tokens, tokenIndex, buffer, 1);
                        }
                        //insertCR(buffer, false);
                        break;
                    case MXMLLexer.END_TAG_OPEN:
                        mCurrentIndent -= mIndentAmount;
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.TAG_CLOSE);
                        if (mTagStack.Count > 0)
                            mTagStack.RemoveAt(mTagStack.Count - 1);
                        tokenIndex = processPostTagText(tokens, tokenIndex, buffer, 1);
                        //                  insertCR(buffer, false);
                        break;
                    case MXMLLexer.EMPTY_TAG_OPEN:
                        tokenIndex = printTag(tokens, tokenIndex, buffer, MXMLLexer.EMPTYTAG_CLOSE);
                        tokenIndex = processPostTagText(tokens, tokenIndex, buffer, 1);
                        //                  insertCR(buffer, false);
                        break;
                    case MXMLLexer.CDATA:
                        if (AntlrUtilities.asTrim(token.Text).Length > 0)
                            mLastCommentStart = (-1);
                        updatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                        //if no enclosing tag, or there is one but it's not one that contains scripts, then just do regular intra-tag data processing
                        if (isPlainXML() || !isASFormattingTag(getEnclosingTag())) //mEnclosingTagName.endsWith(":Script"))
                        {
                            processPCData(token, buffer);
                            insertCR(buffer, false);
                            break;
                        }

                        //handle script
                        bool succeeded = processActionScriptBlock(token, buffer);
                        if (succeeded)
                            break;

                        buffer.Append(token.Text);
                        break;
                    case MXMLLexer.PCDATA:
                        if (AntlrUtilities.asTrim(token.Text).Length > 0)
                            mLastCommentStart = (-1);
                        updatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
                        processPCData(token, buffer);
                        break;
                    case MXMLLexer.EOL:
                        if (!mDoFormat)
                        {
                            insertCR(buffer, true);
                        }
                        else if (isKeepBlankLines())
                        {
                            //special handling to grab subsequent newlines and determine the proper number of
                            //blank lines.  Alg: walk next tokens until I hit EOF or a non-EOL, non-WS token.
                            //Insert blank lines on each found EOL.
                            tokenIndex++;
                            bool doLoop = true;
                            while (tokenIndex < tokens.Count && doLoop)
                            {
                                token = tokens[tokenIndex];
                                switch (token.Type)
                                {
                                    case MXMLLexer.EOL:
                                        insertCR(buffer, true);
                                        tokenIndex++;
                                        break;
                                    case MXMLLexer.WS:
                                        tokenIndex++;
                                        //do nothing
                                        break;
                                    case MXMLLexer.PCDATA:
                                        String nonWS = AntlrUtilities.asTrim(token.Text);
                                        if (nonWS.Length == 0)
                                        {
                                            tokenIndex++;
                                        }
                                        else doLoop = false;
                                        break;
                                    //otherwise drop through to default case
                                    default:
                                        //non-whitespace, need to kick out
                                        tokenIndex--; //revert back to previous token, whatever it was
                                        doLoop = false; // Kick out of loop
                                        break;
                                }
                            }

                            //if we kick out because we're at the end of the token stream, we should drop out of the "for" loop correctly
                        }
                        break;
                    case MXMLLexer.WS:
                        if (mRearrangeOnly)
                        {
                            buffer.Append(token.Text);
                            break;
                        }

                        if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                        {
                            buffer.Append(token.Text);
                        }
                        break;
                    default:
                        updatePartialFormattingBoundaries(tokens[tokenIndex], tokens[tokenIndex], buffer);
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
                //set the output range to start at the previous line start
                String bufferString = buffer.ToString();
                //if (bufferString[mOutputRange.X] == '\n')
                    //mOutputRange.X++;
                int lastCR = buffer.ToString().LastIndexOf('\n', mOutputRange.X);
                if (lastCR >= 0)
                    mOutputRange.X = lastCR + 1;
                else
                    mOutputRange.X = 0;

                //move outputRange.y to contain the next CR, if we are right on it
                if (mOutputRange.Y < bufferString.Length && bufferString[mOutputRange.Y] == '\n')
                    mOutputRange.Y++;

                //now, find the previous CR, which we will *not* include in the range
                int nextCR = buffer.ToString().LastIndexOf('\n', mOutputRange.Y);
                if (nextCR >= 0)
                    mOutputRange.Y = nextCR; //don't need to actually include the CR character
            }

            if (mReplaceRange != null)
            {
                //set the replace range to start at the previous line start
                //if we're already at the end of line, then move forward past the end of line
                if (mSource[mReplaceRange.X] == '\n')
                    mReplaceRange.X++;
                else if (isCarriageReturnPair(mSource, mReplaceRange.X))
                    mReplaceRange.X += 2;
                int lastCR = mSource.LastIndexOf('\n', mReplaceRange.X);
                if (lastCR >= 0)
                    mReplaceRange.X = lastCR + 1;
                else
                    mReplaceRange.X = 0;

                //again, move past the end of line, if we're on it
                if (mReplaceRange.Y < mSource.Length && mSource[mReplaceRange.Y] == '\n')
                    mReplaceRange.Y++;
                else if (isCarriageReturnPair(mSource, mReplaceRange.Y))
                    mReplaceRange.Y += 2;

                //now, go backwards to the previous EOL
                int nextCR = mSource.LastIndexOf('\n', mReplaceRange.Y);
                if (nextCR >= 0 && nextCR > mReplaceRange.X)
                {
                    mReplaceRange.Y = nextCR; //don't need to actually include the CR
                    if (nextCR > 0 && isCarriageReturnPair(mSource, mReplaceRange.Y - 1)) //mSource.charAt(mReplaceRange.y)=='\n' && mSource.charAt(mReplaceRange.y-1)=='\r')
                        mReplaceRange.Y--; //need to move before CR pair, if it's a pair
                }
            }

        }

        private CommonToken getNextNonWSToken(List<CommonToken> tokens, int tokenIndex)
        {
            while (tokenIndex < tokens.Count)
            {
                CommonToken token = tokens[tokenIndex];
                if (AntlrUtilities.asTrim(token.Text).Length > 0)
                    return token;
                tokenIndex++;
            }
            return null;
        }

        private bool isPrivateTag(String tag)
        {
            if (!mUsePrivateTags)
                return false;

            //process list using regex to compare each private tag with endTag
            foreach (String privateTag in mPrivateTags)
            {
                if (Regex.Matches(privateTag, tag).Count > 0)
                    return true;
            }

            return false;
        }

        private bool processActionScriptBlock(IToken token, StringBuilder buffer)
        {
            int methodSavedIndent = mCurrentIndent;
            try
            {
                int startIndex = token.Text.IndexOfOrdinal(CDataStart);
                int endIndex = token.Text.LastIndexOfOrdinal(CDataEnd);

                if (mKeepCDataOnSameLine)
                {
                    //get rid of carriage return
                    ASFormatter.trimAllWhitespaceOnEndOfBuffer(buffer);
                    mCurrentIndent -= mIndentAmount;
                }
                else
                {
                    if (startIndex >= 0)
                        mCurrentIndent += ((mCDATAIndentTabs - 1) * getTabSize());  //-1 because we had already adjusted the indent in the caller method
                    else
                        mCurrentIndent -= mIndentAmount;
                }

                //if we found the cdata start and end or if we don't require those
                if (!isRequireCDATAForASContent() || (startIndex >= 0 && endIndex >= 0))
                {
                    int cdataOffset = 0;
                    int preTextCRCount = 0;
                    String text = token.Text;
                    if (endIndex >= 0)
                        text = text.Substring(0, endIndex);
                    if (startIndex >= 0)
                    {
                        text = text.Substring(startIndex + CDataStart.Length);
                        cdataOffset = CDataStart.Length;
                    }
                    for (int k = 0; k < text.Length; k++)
                    {
                        char c = text[k];
                        if (!AntlrUtilities.isASWhitespace(c))
                            break;
                        if (c == '\n' || c == '\r')
                        {
                            if (isCarriageReturnPair(text, k))
                                k++; //skip an extra character
                            preTextCRCount++;
                        }
                    }

                    //mASPrinter.setSelectedRange(null);

                    //if we are attempting a partial format and we haven't already captured the boundaries
                    bool includesEndOfActionScript = false;
                    int lineCount = 0;
                    if (mDoFormat && mSelectedRange != null && (mOutputRange == null || mOutputRange.Y < 0))
                    {
                        //                              String[] lines=splitTextOnLineBreaks(text);
                        lineCount = countLines(text);
                        int startLine = token.Line;
                        int endLine = startLine + lineCount - 1; //lines.length-1;
                        if (mOutputRange == null)
                        {
                            //if the selected lines start inside the script block
                            if (mSelectedRange.X >= startLine && mSelectedRange.X <= endLine)
                            {
                                //now, determine whether the selection end is also included and determine the
                                //appropriate selection setting for the actionscript printer selection range

                                if (mSelectedRange.Y < endLine) //ends during block (< so that it doesn't include last line of block)
                                {
                                    mASPrinter.setSelectedRange(new Point(mSelectedRange.X - startLine + 1, mSelectedRange.Y - startLine + 1));
                                    includesEndOfActionScript = false;
                                }
                                else
                                {
                                    //doesn't end during block, so go until end of block 
                                    mASPrinter.setSelectedRange(new Point(mSelectedRange.X - startLine + 1, lineCount)); //lines.length));
                                    includesEndOfActionScript = true;
                                }
                            }
                        }
                        else
                        {
                            //we are in the middle of the selected area.  Does the selected area end during
                            //the actionscript block, or does it cover the entire block.
                            if (mSelectedRange.Y <= endLine) //ends during block
                            {
                                mASPrinter.setSelectedRange(new Point(1, mSelectedRange.Y - startLine + 1));
                                includesEndOfActionScript = false;
                            }
                        }
                    }

                    
                    //Handle rearranging.  Only do this if we are in format mode and we are performing the change on the entire document
                    bool markBlockAsValidated = false;
                    String addedText = "";
                    String removedText = "";
                    bool changesMade = false;
                    
                    /*
                    if (mRearrangeOnly || (mDoFormat && mASPrinter.getSelectedRange() == null && settings.Pref_AS_RearrangeAsPartOfFormat))
                    {
                        ASRearranger rearranger = new ASRearranger(mStore);
                        IDocument doc = new Document(text);
                        bool success = rearranger.rearrangeCode(doc, new List<MarkerAnnotation>(), true);
                        if (!success)
                        {
                            //System.out.println("Failed to rearrange: "+text);
                            if (!rearranger.isSoftFailure())
                                return false;
                        }
                        changesMade = rearranger.hasChanges();
                        mAddedText+= rearranger.getAddedText();
                        mRemovedText += rearranger.getRemovedText();
                        addedText = rearranger.getAddedText();
                        removedText = rearranger.getRemovedText();
                        text = doc.get();
                        markBlockAsValidated = true;
                    }
                    */

                    String trimmedResult = AntlrUtilities.asTrim(text);
                    int oldLength = 0;
                    int leadingWhitespaceCount = 0;
                    int codeStartIndent = mCurrentIndent;
                    //          if (startIndex>=0)
                    codeStartIndent += mIndentAmount * mScriptIndentTabs;
                    String resultData = trimmedResult;
                    if (!mRearrangeOnly)
                    {
                        mASPrinter.setDoFormat(mDoFormat);
                        mASPrinter.setData(text);
                        resultData = mASPrinter.print(codeStartIndent);
                        if (resultData == null)
                        {
                            mParseErrors = mASPrinter.getParseErrors();
                            if (mParseErrors != null)
                            {
                                //translate exception positions to main document
                                foreach (Exception ex in mParseErrors)
                                {
                                    if (ex is RecognitionException)
                                    {
                                        RecognitionException rex = (RecognitionException)ex;
                                        IToken t = rex.Token;
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

                        //otherwise, capture the extra characters that might have been added by the format
                        mAddedText += mASPrinter.getAddedText();
                        mRemovedText += mASPrinter.getRemovedText();
                        addedText += mASPrinter.getAddedText();
                        removedText += mASPrinter.getRemovedText();
                        if (mDoFormat)
                        {
                            mNeedAnotherPass |= mASPrinter.needAnotherPass(); //there might be multiple script blocks
                        }
                        if (!markBlockAsValidated && (mASPrinter.getAddedText().Length > 0 || mASPrinter.getRemovedText().Length > 0))
                        {
                            markBlockAsValidated = true; //too complicated to determine the exact character positions, at least in the original doc

                            //TODO: Need to take into account the leading ws trimming and the correct buffer length
                            //grab replace maps too, and update positions if necessary. Whitespace also might be 
                            //adjusted at the start of the block.  So this code needs to be done where the main
                            //replace block code is done, AND more adjustments need to be made.
                            //                  Map<Integer, ReplacementRange> asRanges=mASPrinter.getReplaceMap();
                            //                  if (asRanges!=null)
                            //                  {
                            //                      if (mReplaceMap==null)
                            //                          mReplaceMap=new HashMap<Integer, ReplacementRange>();
                            //                      CommonToken ct=(CommonToken)token;
                            //                      for (Map.Entry<Integer, ReplacementRange> entry : asRanges.entrySet()) {
                            //                          entry.getValue().mRangeInOriginalDoc.x+=ct.StartIndex);
                            //                          entry.getValue().mRangeInOriginalDoc.y+=ct.StartIndex);
                            //                          entry.getValue().mRangeInFormattedDoc.x+=buffer.Length;
                            //                          entry.getValue().mRangeInFormattedDoc.y+=buffer.Length;
                            //                          mReplaceMap.put(entry.getKey()+ct.StartIndex), entry.getValue());
                            //                      }
                            //                  }
                        }
                    }

                    if (startIndex >= 0)
                    {
                        addIndentIfAtStartOfLine(buffer, true);
                        buffer.Append(CDataStart);
                    }
                    if (mDoFormat)
                    {
                        insertCR(buffer, false);
                        for (int i = 0; i < mBlankLinesAtCDataStart; i++)
                            insertCR(buffer, false);
                    }
                    else
                    {
                        for (int k = 0; k < preTextCRCount; k++)
                        {
                            insertCR(buffer, true);
                        }
                    }
                    int saveIndent = mCurrentIndent;
                    //          if (startIndex>=0)
                    mCurrentIndent = codeStartIndent; //+=mIndentAmount;
                    addIndentIfAtStartOfLine(buffer, true);
                    mCurrentIndent = saveIndent;

                    //TODO: change to determine the amount of leading whitespace first and capture
                    //the previous buffer length
                    int leadingCRCount = 0;
                    for (; leadingWhitespaceCount < resultData.Length; leadingWhitespaceCount++)
                    {
                        char c = resultData[leadingWhitespaceCount];
                        if (!AntlrUtilities.isASWhitespace(c))
                        {
                            break;
                        }
                        if (c == '\n')
                            leadingCRCount++;
                    }

                    oldLength = buffer.Length;
                    trimmedResult = AntlrUtilities.asTrim(resultData);

                    if (markBlockAsValidated && (AntlrUtilities.asTrim(addedText).Length > 0 || AntlrUtilities.asTrim(removedText).Length > 0 || changesMade))
                    {
                        if (mReplaceMap == null)
                            mReplaceMap = new Dictionary<Int32, ReplacementRange>();
                        int replacementStartIndex = ((CommonToken)token).StartIndex;
                        if (startIndex >= 0)
                            replacementStartIndex += (startIndex + CDataStart.Length);
                        int replacementEndIndex = ((CommonToken)token).StartIndex;
                        if (endIndex >= 0)
                            replacementEndIndex += endIndex;
                        else
                            replacementEndIndex += ((CommonToken)token).Text.Length;
                        ReplacementRange range = new ReplacementRange(new Point(buffer.Length, buffer.Length + trimmedResult.Length), new Point(replacementStartIndex, replacementEndIndex));
                        range.setChangedText(addedText, removedText);
                        mReplaceMap.Add(buffer.Length, range);
                    }

                    buffer.Append(trimmedResult);

                    //now, patch up the partial format return boundaries if necessary
                    if (mASPrinter.getSelectedRange() != null)
                    {
                        Point outputRange = mASPrinter.getOutputRange();
                        Point replaceRange = mASPrinter.getReplaceRange();
                        if (outputRange != null && replaceRange != null)
                        {
                            if (mOutputRange == null)
                            {
                                mOutputRange = new Point(0, -1);
                                mReplaceRange = new Point(0, -1);

                                //establish the beginning boundaries
                                mOutputRange.X = oldLength + outputRange.X - leadingWhitespaceCount;
                                //mReplaceRange.x=token.getLine()+replaceLines.x-1; //adjust for leading CRs and existing whitespace
                                mReplaceRange.X = ((CommonToken)token).StartIndex + cdataOffset + replaceRange.X; //-leadingWhitespaceCount;

                                //selected range starts, possibly also ends inside actionscript block
                                if (includesEndOfActionScript)
                                {
                                    //goes to end of actionscript; don't need to do anything here
                                }
                                else
                                {
                                    //if it stops part way through the block; we need to finish output/replace boundaries here
                                    mOutputRange.Y = oldLength + Math.Min(trimmedResult.Length, outputRange.Y - leadingWhitespaceCount); //outputRange.y-leadingWhitespaceCount;//-trailingWhitespaceCount;
                                    //                                          mReplaceRange.y=token.getLine()+replaceLines.y-1; //adjust for leading CRs and existing whitespace
                                    mReplaceRange.Y = ((CommonToken)token).StartIndex + cdataOffset + replaceRange.Y;
                                }
                            }
                            else
                            {
                                //it stops part way through the block; we need to finish output/replace boundaries here
                                mOutputRange.Y = oldLength + Math.Min(trimmedResult.Length, outputRange.Y - leadingWhitespaceCount);
                                //                                      mReplaceRange.y=token.getLine()+replaceLines.y-1; //adjust for leading CRs and existing whitespace
                                mReplaceRange.Y = ((CommonToken)token).StartIndex + cdataOffset + replaceRange.Y; //-leadingWhitespaceCount;
                            }
                        }
                    }

                    insertCR(buffer, false);

                    if (mDoFormat)
                    {
                        for (int i = 0; i < mBlankLinesAtCDataEnd; i++)
                        {
                            insertCR(buffer, false);
                        }
                    }

                    addIndentIfAtStartOfLine(buffer, true);
                    if (!mDoFormat)
                    {
                        for (int k = text.Length - 1; k >= 0; k--)
                        {
                            char c = text[k];
                            if (!AntlrUtilities.isASWhitespace(c))
                                break;
                            if (c == '\n')
                            {
                                insertCR(buffer, true);
                            }
                        }
                        addIndentIfAtStartOfLine(buffer, true);
                    }

                    //update the formatting boundary to catch the case where
                    //the start of the selection only catches the end of the code block.
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
                        if (mDoFormat && !isKeepCDataOnSameLine())
                            insertCR(buffer, false);
                    }
                    else
                    {
                        //if we're not sticking the end CDATA tag on, then just delete the whitespace at the end of the line
                        ASFormatter.trimWhitespaceOnEndOfBuffer(buffer);
                    }

                    return true;
                }

                return false;
            }
            finally
            {
                mCurrentIndent = methodSavedIndent;
            }
        }

        private bool isASFormattingTag(String enclosingTagName)
        {
            if (mASScriptTags.Contains(enclosingTagName))
                return true;

            foreach (String scriptTag in mASScriptTags)
            {
                if (Regex.Matches(scriptTag, enclosingTagName).Count > 0)
                    return true;
            }

            return false;
        }

        private int countLines(String text)
        {
            int count = 1;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\r')
                {
                    if (isCarriageReturnPair(text, i))
                        i++;
                    count++;
                }
                else if (c == '\n')
                    count++;

            }
            return count;
        }

        private bool isCarriageReturnPair(String source, int loc)
        {
            if (loc + 1 < source.Length)
            {
                if (source[loc] == '\r' && source[loc + 1] == '\n')
                    return true;
            }

            return false;
        }

        private void processPCData(IToken token, StringBuilder buffer)
        {
            if (mRearrangeOnly)
            {
                buffer.Append(token.Text);
                return;
            }

            //if we are inside a CData section and the enclosing tag is not explicitly set to allow formatting,
            //then we just spit out the data.  I'm going to ignore the case of an empty CData section.
            if (!mTagsWhoseTextContentsCanBeFormatted.Contains(getEnclosingTag()) && AntlrUtilities.asTrim(token.Text).StartsWithOrdinal(CDataStart))
            {
                //put start of tag on new line and indent it
                if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                    insertCR(buffer, false);
                addIndentIfAtStartOfLine(buffer, false);

                //spit out the text
                buffer.Append(token.Text.Replace("\r\n", "\n").Replace('\r', '\n'));

                //TODO: add a carriage return here if we can determine the correct behavior based on format/indent mode
                //and "keep blank lines"
                return;
            }

            if (mDoFormat)
            {
                String[] lines = AntlrUtilities.asTrim(token.Text).Split('\n');
                if (lines.Length == 0 && token.Text == "\n")
                    lines = new String[] { "", "" };
                //if all whitespace but no carriage returns, then we don't want to go through the loop
                else if (token.Text.IndexOf('\n') < 0 && AntlrUtilities.asTrim(token.Text).Length == 0)
                    lines = new String[] { };
                for (int k = 0; k < lines.Length; k++)
                {
                    String lineData = lines[k];
                    if (mDoFormat)
                    {
                        if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                            insertCR(buffer, false);
                        lineData = AntlrUtilities.asTrim(lineData);
                        if (lineData.Length == 0)
                            continue;
                        addIndentIfAtStartOfLine(buffer, false);
                        buffer.Append(lineData);
                        insertCR(buffer, false);
                    }
                }
            }
            else
            {
                String text = token.Text;
                bool beforeTextOnLine = ASFormatter.isOnlyWhitespaceOnLastLine(buffer);
                for (int k = 0; k < text.Length; k++)
                {
                    char c = text[k];
                    if (AntlrUtilities.isASWhitespace(c))
                    {
                        if (c == '\n' || c == '\r')
                        {
                            if (isCarriageReturnPair(text, k))
                                k++; //advanced extra position to account for \r\n
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
                        addIndentIfAtStartOfLine(buffer, false);
                        beforeTextOnLine = false;
                        buffer.Append(c);
                    }
                }
            }
        }


        private void insertCR(StringBuilder buffer, bool overridE)
        {
            if (mDoFormat || overridE)
            {
                buffer.Append('\n');
            }
        }

        private void movePartialFormattingBoundaries(int position, int count)
        {
            if (!mDoFormat)
                return;

            if (mSelectedRange != null)
            {
                if (mOutputRange != null)
                {
                    if (mOutputRange.X >= position)
                        mOutputRange.X += count;
                    if (mOutputRange.Y > position)
                        mOutputRange.Y += count;
                }
            }
        }

        private void updatePartialFormattingBoundaries(IToken startToken, IToken endToken, StringBuilder buffer)
        {
            if (!mDoFormat)
                return;

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
                //find end token, so I can determine whether I need to capture the formatting boundary at this point
                CommonToken endToken2 = tokens[tokenIndex];
                int endTokenIndex = tokenIndex;
                while (endTokenIndex < tokens.Count)
                {
                    CommonToken token = tokens[endTokenIndex];
                    if (token.Type == stopType)
                    {
                        endToken2 = token;
                        break;
                    }

                    endTokenIndex++;
                }

                updatePartialFormattingBoundaries(tokens[tokenIndex], endToken2, buffer);
            }

            /////////////////////////////////////////////////////////////////////////////////////////
            /////handle blanks lines before/between tags
            //we have special handling if the tag is a start tag (either regular open or an empty tag)
            bool isOpenTag = (startToken.Type == MXMLLexer.EMPTY_TAG_OPEN || startToken.Type == MXMLLexer.TAG_OPEN);
            bool hasSeenFirstChild = false;
            //      if (isOpenTag)
            {
                //This is part of blank lines between sibling tags.
                //if this is an open tag, then see if we have already seen the first child of the parent tag.
                TagStackEntry entry = getCurrentTag();
                if (entry != null)
                {
                    hasSeenFirstChild = entry.isSeenFirstChild();
                    if (isOpenTag)
                        entry.setSeenFirstChild();
                }
            }

            //We may need to add some blank lines before this tag.  If we are formatting AND it's an open tag AND
            //either we have spaces between sibling tags or we have blank lines before arbitrary tags.
            if (mDoFormat && (getBlankLinesBeforeTags() > 0 || getBlankLinesAfterParentTags() > 0 || (getSpacesBetweenSiblingTags() > 0 && hasSeenFirstChild) || (getSpacesAfterParentTags() > 0 && !hasSeenFirstChild)) && isOpenTag)
            {
                //walk through tokens to find the tag name, which we need for the "blanks before tags" option.
                int testTokenIndex = tokenIndex;
                while (testTokenIndex < tokens.Count)
                {
                    CommonToken token = tokens[testTokenIndex];
                    if (token.Type == MXMLLexer.GENERIC_ID)
                    {
                        //we've walked through the whitespace tokens and arrived at the tag name.  We'll handle the blank lines between
                        //sibling tags here as well so that we can determine the correct number of lines between the two settings.
                        int blankLinesToEnsure = 0;
                        //if the tag is one that should have blank lines before it, then capture that number of lines
                        if (mTagsWithBlankLinesBeforeThem.Contains(token.Text) || matchesRegEx(token.Text, mTagsWithBlankLinesBeforeThem))
                            blankLinesToEnsure = Math.Max(blankLinesToEnsure, getBlankLinesBeforeTags());

                        if (getCurrentTag() != null && !hasSeenFirstChild && (mParentTagsWithBlankLinesAfterThem.Contains(getCurrentTag().getTagName()) || matchesRegEx(getCurrentTag().getTagName(), mParentTagsWithBlankLinesAfterThem)))
                            blankLinesToEnsure = Math.Max(blankLinesToEnsure, getBlankLinesAfterParentTags());

                        //if we've previously seen the first child of the tag, then get the blank lines between sibling tags
                        if (hasSeenFirstChild)
                            blankLinesToEnsure = Math.Max(blankLinesToEnsure, getSpacesBetweenSiblingTags());
                        else if (getCurrentTag() != null) //if we haven't seen the first child, then this is the first child, and we are interested in blank lines after parent
                            blankLinesToEnsure = Math.Max(blankLinesToEnsure, getSpacesAfterParentTags());

                        //if we need to ensure any blank lines, then continue
                        if (blankLinesToEnsure > 0)
                        {
                            addBlankLines(buffer, blankLinesToEnsure);
                        }
                        break;
                    }
                    testTokenIndex++;
                }
            }
            else if (mDoFormat && !isOpenTag && hasSeenFirstChild && getBlankLinesBeforeCloseTags() > 0)
            {
                addBlankLines(buffer, getBlankLinesBeforeCloseTags());
            }
            /////////////////////////////////////////////////////////////////////////////////////////

            mLastCommentStart = (-1);

            addIndentIfAtStartOfLine(buffer, false);
            int startOfTagInBuffer = buffer.Length; //we need this to be the point right where the first token gets added (it needs to match startToken)
            buffer.Append(tokens[tokenIndex].Text);
            tokenIndex++;
            List<Attr> attrs = null;
            if (mDoFormat && stopType != MXMLLexer.DECL_STOP)
                attrs = new List<Attr>();
            //      bool seenTagName=false;
            String tagName = null;
            String currentAttrName = null;
            CommonToken endToken = null;
            String spaceString = ASFormatter.generateSpaceString(mSpacesAroundEquals);
            int extraWrappedLineIndent = 0;

            while (tokenIndex < tokens.Count)
            {
                //addIndentIfAtStartOfLine(buffer);
                CommonToken token = tokens[tokenIndex];
                if (token.Type == stopType)
                {
                    endToken = token;
                    if (attrs == null)
                    {
                        addIndentIfAtStartOfLine(buffer, false);
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
                            //ignore
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

                            if (tagName == null)
                                tagName = token.Text;
                            else
                            {
                                currentAttrName = token.Text;
                            }

                            //                  if (seenTagName)
                            //                      buffer += ' ');
                            //                  buffer += token.Text));
                            //                  seenTagName=true;
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
                                    if (isUseSpacesInsideAttrBraces())
                                    {
                                        fixupBindingExpressions(a);
                                    }
                                    attrs.Add(a);
                                }
                            }
                            else
                            {
                                buffer.Append(token.Text);
                            }
                            break;
                        default:
                            if (attrs == null)
                            {
                                buffer.Append(token.Text);
                            }
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
                            if (mRearrangeOnly)
                            {
                                buffer.Append(token.Text);
                                break;
                            }

                            //only print if there's text on the line already
                            if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                                buffer.Append(token.Text);
                            break;
                        default:
                            bool capturedTagName = false;
                            if (tagName == null)
                            {
                                tagName = token.Text;
                                capturedTagName = true;
                            }
                            addIndentIfAtStartOfLine(buffer, false);
                            buffer.Append(token.Text);
                            if (capturedTagName)
                            {
                                if (getWrapStyle() == WrapOptions.WRAP_STYLE_INDENT_TO_WRAP_ELEMENT)
                                    extraWrappedLineIndent = getLastLineColumnLength(buffer) + 1 - mCurrentIndent;
                                else
                                    extraWrappedLineIndent = mHangingIndentSize * getIndentAmount();
                                mCurrentIndent += extraWrappedLineIndent;
                            }
                            break;
                    }
                }

                tokenIndex++;
            }

            mCurrentIndent -= extraWrappedLineIndent;

            mEnclosingTagName = tagName;

            //spit out data here if we're formatting
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
                    buffer.Append(ASFormatter.generateSpaceString(1)); //TODO: configurable number of spaces before first attr

                    List<Attr> oldAttrOrder = new List<Attr>();
                    oldAttrOrder.AddRange(attrs);

                    //now, order attrs based on user settings
                    int sortStart = 0;
                    if (mAttrOrderMode == MXML_ATTR_ORDERING_USEDATA)
                    {
                        //rewrite to capture all attributes in groups on a first pass.  Then walk attributes on the second pass
                        //to populate the newAttrOrder list
                        Dictionary<String, List<AttrMapping>> groupMap = new Dictionary<String, List<AttrMapping>>(); //hold items in each group in group order
                        Dictionary<String, List<AttrMapping>> literalMap = new Dictionary<String, List<AttrMapping>>();

                        //1st pass captures the attributes in groups
                        //2nd pass performs reordering etc.
                        for (int i = 0; i < mManualAttrSortOrder.Count; i++)
                        {
                            String sortItem = mManualAttrSortOrder[i];
                            String[] attrItems = sortItem.Split(',');

                            //if we found at least one attr or none were missing
                            foreach (String attr in attrItems)
                            {
                                String attr2 = AntlrUtilities.asTrim(attr);
                                if (attr.Length == 0)
                                    continue;
                                if (!(attr2 == NewLineFlag))
                                {
                                    String groupName = isGroupAttr(attr2);
                                    if (groupName != null)
                                    {
                                        AttrGroup group = mAttrGroups[groupName];
                                        if (group != null)
                                        {
                                            List<AttrMapping> attrsForGroup = new List<AttrMapping>();
                                            groupMap[groupName] = attrsForGroup;
                                            //in this case, we want to find items in the tag and keep
                                            //them in that order
                                            List<String> groupAttrSet = mHashedGroupAttrs[group.getName()];
                                            if (groupAttrSet != null && groupAttrSet.Count > 0)
                                            {
                                                for (int k = attrs.Count - 1; k >= 0; k--)
                                                {
                                                    String attrName = attrs[k].mName;
                                                    bool matchFound = groupAttrSet.Contains(attrName);
                                                    String attrSpec = attrName;
                                                    if (!matchFound)
                                                    {
                                                        //check for regular expressions matching attr
                                                        foreach (String regexAttr in group.getRegexAttrs())
                                                        {
                                                            matchFound = matchesSpec(attrName, regexAttr, true, false); //group.isIncludeStates());
                                                            if (matchFound)
                                                            {
                                                                attrSpec = regexAttr;
                                                                break;
                                                            }
                                                        }
                                                    }

                                                    if (matchFound)
                                                    {
                                                        attrsForGroup.Add(new AttrMapping(attrs[k], attrSpec));
                                                        attrs.RemoveAt(k);
                                                    }
                                                }
                                                AttrMapping[] reversed = attrsForGroup.ToArray();
                                                Array.Reverse(reversed);
                                                attrsForGroup.Clear();
                                                attrsForGroup.AddRange(reversed);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        List<AttrMapping> attrsForGroup = new List<AttrMapping>();
                                        literalMap[attr] = attrsForGroup;
                                        for (int k = attrs.Count - 1; k >= 0; k--)
                                        {
                                            String attrName = attrs[k].mName;
                                            if (attrName == attr)
                                            {
                                                //add in reverse order to maintain file order
                                                attrsForGroup.Insert(0, new AttrMapping(attrs[k], attr));
                                                attrs.RemoveAt(k);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (groupMap.ContainsKey(Attr_Group_Other))
                        {
                            List<AttrMapping> attrsForGroup = new List<AttrMapping>();
                            groupMap[Attr_Group_Other] = attrsForGroup;
                            for (int k = 0; k < attrs.Count; k++)
                            {
                                String attrName = attrs[k].mName;
                                attrsForGroup.Add(new AttrMapping(attrs[k], ""));
                            }
                            attrs.Clear();
                        }

                        List<Attr> newAttrOrder = new List<Attr>();
                        for (int i = 0; i < mManualAttrSortOrder.Count; i++)
                        {
                            String sortItem = mManualAttrSortOrder[i];
                            String[] attrItems = sortItem.Split(',');
                            bool missingAttr = false;
                            bool existingAttr = false;

                            foreach (String attrSpec in attrItems)
                            {
                                String attrSpec2 = AntlrUtilities.asTrim(attrSpec);
                                if (attrSpec2.Length == 0)
                                    continue;
                                if (!(attrSpec2 == NewLineFlag))
                                {
                                    bool found = false;
                                    String groupName = isGroupAttr(attrSpec);
                                    if (groupName != null)
                                    {
                                        List<AttrMapping> cachedAttrs = groupMap[groupName];
                                        if (cachedAttrs != null && cachedAttrs.Count > 0) //AttrMapping.hasAttr(cachedAttrs, attrSpec))
                                            found = true;
                                    }
                                    else
                                    {
                                        List<AttrMapping> cachedAttrs = literalMap[attrSpec];
                                        found = (cachedAttrs != null && cachedAttrs.Count > 0);
                                    }

                                    if (!found)
                                        missingAttr = true;
                                    else
                                        existingAttr = true;
                                }
                            }

                            //if we found at least one attr or none were missing
                            if (existingAttr || !missingAttr)
                            {
                                foreach (String attr in attrItems)
                                {
                                    String attr2 = AntlrUtilities.asTrim(attr);
                                    if (attr2.Length == 0)
                                        continue;
                                    if (attr2 == NewLineFlag)
                                    {
                                        Attr newLineAttr = new Attr();
                                        newLineAttr.mName = NewLineFlag;
                                        newAttrOrder.Add(newLineAttr);
                                    }
                                    else
                                    {
                                        String groupName = isGroupAttr(attr);
                                        if (groupName != null)
                                        {
                                            AttrGroup group = mAttrGroups[groupName];
                                            if (group != null)
                                            {
                                                //make a copy of mappings so I can remove them in one
                                                //of the codepaths below
                                                List<AttrMapping> mappingsForGroup = new List<AttrMapping>();
                                                mappingsForGroup.AddRange(groupMap[groupName]);
                                                List<Attr> attrsForGroup = new List<Attr>();

                                                switch (group.getSortMode())
                                                {
                                                    case MXML_Sort_None:
                                                    case MXML_Sort_AscByCase:
                                                        foreach (AttrMapping mapping in mappingsForGroup)
                                                        {
                                                            attrsForGroup.Add(mapping.mAttr);
                                                        }
                                                        if (group.getSortMode() == MXML_Sort_AscByCase)
                                                        {
                                                            attrsForGroup.Sort();
                                                        }
                                                        break;
                                                    case MXML_Sort_GroupOrder:
                                                        //this one needs to be done in reverse: walk the group items and find items
                                                        //that match and keep them in group order
                                                        List<String> groupAttrs = group.getAttrs();
                                                        foreach (String attrSpec in groupAttrs)
                                                        {
                                                            List<Attr> newAttrs = new List<Attr>();
                                                            for (int j = mappingsForGroup.Count - 1; j >= 0; j--)
                                                            {
                                                                AttrMapping mapping = mappingsForGroup[j];
                                                                if (mapping.mAttrSpec == attrSpec || mapping.mAttrSpec == attrSpec + StateRegexSuffix)
                                                                {
                                                                    newAttrs.Add(mapping.mAttr);
                                                                    mappingsForGroup.RemoveAt(j);
                                                                }
                                                            }
                                                            newAttrs.Sort();
                                                            attrsForGroup.AddRange(newAttrs);
                                                        }
                                                        break;
                                                }

                                                if (attrsForGroup.Count > 0)
                                                {
                                                    //add wrap mode
                                                    insertAttrs(-1, group, attrsForGroup, newAttrOrder);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            List<AttrMapping> mappingsForItem = literalMap[attr];
                                            if (mappingsForItem != null)
                                            {
                                                foreach (AttrMapping attrMapping in mappingsForItem)
                                                {
                                                    newAttrOrder.Add(attrMapping.mAttr);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //if we didn't have any leftover attrs, then clean up extra newlines
                        if (attrs.Count == 0)
                        {
                            //remove extra newlines at end
                            while (newAttrOrder.Count > 0)
                            {
                                Attr attr = newAttrOrder[newAttrOrder.Count - 1];
                                if (attr.mName == NewLineFlag)
                                {
                                    newAttrOrder.RemoveAt(newAttrOrder.Count - 1);
                                    allAttrsArePartOfCustomOrdering = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        sortStart = newAttrOrder.Count;
                        newAttrOrder.AddRange(attrs);
                        if (mSortOtherAttrs)
                        {
                            if (sortStart < newAttrOrder.Count - 1)
                            {
                                // TODO: Is this correct?

                                Attr[] temp = newAttrOrder.ToArray();
                                newAttrOrder.CopyTo(temp, newAttrOrder.Count);
                                Array.Sort(temp);
                                newAttrOrder.Clear();
                                newAttrOrder.AddRange(temp);

                                //Collections.sort(newAttrOrder.subList(sortStart, newAttrOrder.Count));
                            }
                        }

                        //compare old attr order and new attr order and see if the attributes are in the same order
                        for (int i = 0, j = 0; i < oldAttrOrder.Count && j < newAttrOrder.Count; )
                        {
                            //skip newlines and other meta flags
                            String newAttrName = newAttrOrder[j].mName;
                            if (newAttrName == NewLineFlag || newAttrName.StartsWith('<'))
                            {
                                j++;
                                continue;
                            }

                            if (!(newAttrOrder[j].mName == oldAttrOrder[i].mName))
                            {
                                attrOrderChanged = true;
                                break;
                            }
                            i++;
                            j++;
                        }

                        //replace attributes with the custom ordering
                        attrs = newAttrOrder;
                    }

                }

                //find last hard newline in attr list (some options only apply after the last newline
                int lastNewLine = (-1);
                if (allAttrsArePartOfCustomOrdering)
                    lastNewLine = attrs.Count;
                int lastAttr = (-1);
                for (int j = 0; j < attrs.Count; j++)
                {
                    Attr attr = attrs[j];
                    if (!allAttrsArePartOfCustomOrdering && attr.mName == NewLineFlag)
                        lastNewLine = j;
                    if (!attr.mName.StartsWith('<'))
                        lastAttr = j;
                }

                //add the ending newline if we  need it
                if (mAttrOrderMode == MXML_ATTR_ORDERING_USEDATA && mAddNewlineAfterLastAttr && attrs.Count > 0)
                {
                    Attr newLineAttr = new Attr();
                    newLineAttr.mName = NewLineFlag;
                    attrs.Add(newLineAttr);
                }

                int firstIndent = (-1);
                int attrsOnLine = 0;
                int wrapMode = MXML_ATTR_WRAP_NONE;
                int attrsPerLine = getAttrsPerLine();
                bool inGroup = false;
                if (lastNewLine < 0)
                    wrapMode = getWrapMode();
                bool disableWrapping = false;
                if (isUseAttrsToKeepOnSameLine() && originalAttrCount <= getAttrsToKeepOnSameLine())
                {
                    disableWrapping = true;
                    //we use the getter here for wrapmode to get the default wrap mode instead of the wrapmode
                    //we would use if we were processing some hardcode attribute rows at the start
                    if (mAlwaysObeyMaxLineLength && getWrapMode() == MXML_ATTR_WRAP_LINE_LENGTH)
                    {
                        //attempt to pre-calculate the width so that I can reverse the disableWrapping mode if the
                        //line is too long
                        //                  bool seenFirstAttr=false;
                        int currentLineLength = ASPrettyPrinter.determineLastLineLength(buffer, getTabSize());
                        foreach (Attr attr in attrs)
                        {
                            if (attr.mName.StartsWith('<'))
                            {
                                //do nothing
                                continue;
                            }
                            else if (attr.mName == NewLineFlag)
                            {
                                //do nothing
                                continue;
                            }

                            currentLineLength++; //for space before first attrs and between attrs (it's hardcoded to 1 space right now)
                            currentLineLength += (attr.mName.Length);
                            currentLineLength += spaceString.Length;
                            currentLineLength += 1; //'='
                            currentLineLength += spaceString.Length;
                            currentLineLength += attr.mValue.Length;
                        }

                        //if the line length we've calculated beyond the max length, then reverse the disableWrapping flag.
                        if (currentLineLength >= mMaxLineLength)
                            disableWrapping = false;

                    }
                }
                for (int j = 0; j < attrs.Count; j++)
                {
                    Attr attr = attrs[j];

                    //check for a group wrap mode
                    if (attr.mName.StartsWith('<'))
                    {
                        if (disableWrapping)
                            continue;

                        //                  wrapMode=(j<lastNewLine) ? MXML_ATTR_WRAP_NONE : getWrapMode();
                        wrapMode = getWrapMode(); //set back to default until I determine otherwise
                        if (attr.mName.StartsWithOrdinal("<Wrap="))
                        {
                            inGroup = true;
                            String dataString = attr.mName.Substring("<Wrap=".Length, (attr.mName.Length - 1) - ("<Wrap=".Length));
                            int commaPos = dataString.IndexOf(',');
                            String modeString = dataString;
                            if (commaPos > 0)
                            {
                                modeString = dataString.Substring(0, commaPos);
                                String nPerLineString = dataString.Substring(commaPos + 1);
                                try
                                {
                                    attrsPerLine = Int32.Parse(nPerLineString);
                                    if (attrsPerLine == AttrGroup.Wrap_Data_Use_Default) attrsPerLine = getAttrsPerLine();
                                }
                                catch { attrsPerLine = getAttrsPerLine(); }
                            }
                            try { wrapMode = Int32.Parse(modeString); }
                            catch {}
                        }
                        else if (attr.mName == "</Wrap>")
                        {
                            inGroup = false;
                            attrsPerLine = getAttrsPerLine(); //set count back to the default
                        }
                        continue;
                    }

                    //establish indent first
                    if (firstIndent < 0)
                    {
                        if (getWrapStyle() == WrapOptions.WRAP_STYLE_INDENT_TO_WRAP_ELEMENT)
                            firstIndent = getLastLineColumnLength(buffer);
                        else
                            firstIndent = mCurrentIndent + mHangingIndentSize * getIndentAmount();
                    }

                    bool isNewline = attr.mName == NewLineFlag;

                    //go ahead and precalculate the string for the attribute/value pair
                    String attrString = "";
                    if (!isNewline)
                    {
                        attrString += attr.mName;
                        attrString += spaceString;
                        attrString += '=';
                        attrString += spaceString;
                        attrString += attr.mValue;
                    }

                    bool justWrapped = false;
                    if (!disableWrapping && j <= lastAttr && wrapMode != MXML_ATTR_WRAP_NONE)
                    {
                        if (wrapMode == MXML_ATTR_WRAP_COUNT_PER_LINE)
                        {
                            if (attrsOnLine > 0 && attrsOnLine >= attrsPerLine)
                            {
                                if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                                {
                                    buffer.Append('\n');
                                    buffer.Append(generateIndent(firstIndent));
                                    attrsOnLine = 0;
                                    justWrapped = true;
                                }
                            }
                        }
                        else if (wrapMode == MXML_ATTR_WRAP_LINE_LENGTH && !isNewline)
                        {
                            //we'll add a line break if the next attribute will push the length beyond max, *unless* we're 
                            //already on a new line, in which case we just go ahead and stick the text on this line.
                            int currentLineLength = ASPrettyPrinter.determineLastLineLength(buffer, getTabSize());
                            if (currentLineLength + attrString.Length >= mMaxLineLength)
                            {
                                if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                                {
                                    buffer.Append('\n');
                                    buffer.Append(generateIndent(firstIndent));
                                    attrsOnLine = 0;
                                    justWrapped = true; //I don't think this needs to be here now, because we're checking for isNewline in the 'if'
                                }
                            }
                        }
                    }

                    if (isNewline)
                    {
                        if (disableWrapping)
                            continue;

                        //reset the default wrap mode if we are beyond the end of the custom wrap mode
                        if (j >= lastNewLine && !inGroup)
                            wrapMode = getWrapMode();

                        if (justWrapped) //don't add another carriage return if one was added via wrapping
                        {
                            //                      wrappedLastIteration=false;
                            continue;
                        }

                        buffer.Append('\n');
                        attrsOnLine = 0;
                        buffer.Append(generateIndent(firstIndent));
                        continue;
                    }

                    //              wrappedLastIteration=false;

                    if (buffer.Length > 0 && !ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                    {
                        if (buffer[buffer.Length - 1] != ' ')
                            buffer.Append(ASFormatter.generateSpaceString(1)); //TODO: configurable number of spaces between attrs?
                    }

                    buffer.Append(attrString);

                    attrsOnLine++;
                }

                //if newline before end tag, BUT we don't want the indent to align with the first attribute, then
                //we clear the whitespace on the current line.  It appears that we always add the indent to the
                //first attribute after we add a newline, even in this case where the newline was at the end
                //of the attributes.
                if (mAddNewlineAfterLastAttr && !mIndentCloseTag && ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                    ASFormatter.trimWhitespaceOnEndOfBuffer(buffer);
                addIndentIfAtStartOfLine(buffer, false); //add an indent to the tag level if there wasn't one.
                if (endToken != null)
                {
                    if (!ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                    {
                        if (endToken.Text == "/>")
                        {
                            //print configurable number of spaces before empty tag end
                            buffer.Append(ASFormatter.generateSpaceString(getSpacesBeforeEmptyTagEnd()));
                        }
                    }
                    buffer.Append(endToken.Text);
                }

                if (attrOrderChanged)
                {
                    if (mReplaceMap == null)
                        mReplaceMap = new Dictionary<Int32, ReplacementRange>();
                    ReplacementRange range = new ReplacementRange(new Point(startOfTagInBuffer, buffer.Length), new Point(startToken.StartIndex, endToken.StopIndex + 1));
                    mReplaceMap[startOfTagInBuffer] = range;
                }
            }

            return tokenIndex;
        }

        private void addBlankLines(StringBuilder buffer, int blankLinesToEnsure)
        {
            //count blank lines at end of current buffer (or before last comment) to see if there are already some there
            int emptyLinesAlreadyThere = ASFormatter.getNumberOfEmptyLinesAtEnd(buffer, mLastCommentStart);
            int linesToAdd = blankLinesToEnsure - emptyLinesAlreadyThere;
            if (linesToAdd > 0)
            {
                if (emptyLinesAlreadyThere == 0 && !ASFormatter.isOnlyWhitespaceOnLastLine(buffer))
                    linesToAdd++;
                for (int i = 0; i < linesToAdd; i++)
                {
                    if (mLastCommentStart < 0)
                        insertCR(buffer, true);
                    else
                    {
                        //if we are inserting before the comment, then we have to do that manually
                        buffer.Insert(mLastCommentStart, "\n");
                        movePartialFormattingBoundaries(mLastCommentStart, 1);
                    }
                }
            }
        }


        private void insertAttrs(int insertLocation, AttrGroup group, List<Attr> attrsForGroup, List<Attr> newAttrOrder)
        {
            int wrapMode = group.getWrapMode();
            if (wrapMode == MXML_ATTR_WRAP_DEFAULT)
                wrapMode = getWrapMode();
            Attr wrapAttr = new Attr();
            //  wrapAttr.mName="<Wrap="+wrapMode+">";
            wrapAttr.mName = "<Wrap=" + wrapMode;
            if (group.getWrapMode() == MXML_ATTR_WRAP_COUNT_PER_LINE)
            {
                wrapAttr.mName += "," + group.getData();
            }
            wrapAttr.mName += ">";
            List<Attr> newItems = new List<Attr>();
            newItems.Add(wrapAttr);
            newItems.AddRange(attrsForGroup);
            wrapAttr = new Attr();
            wrapAttr.mName = "</Wrap>";
            newItems.Add(wrapAttr);
            newAttrOrder.InsertRange(insertLocation >= 0 ? insertLocation : newAttrOrder.Count, newItems);
        }

        //  private void findOtherAttrs(List<Attr> codeAttrs, Map<String, Set<String>> hashedGroupAttrs, Collection<String> outputAttrs) 
        //  {
        //      mainLoop: for (Attr attr : codeAttrs) {
        //          for (String groupName : hashedGroupAttrs.keySet()) {
        //              AttrGroup group=mAttrGroups.get(groupName);
        //              if (group==null)
        //                  continue;
        //              for (String groupAttrSpec : group.getAttrs()) {
        //                  bool isRegex=AttrGroup.isRegexString(groupAttrSpec);
        //                  if (matchesSpec(attr.mName, groupAttrSpec, isRegex, group.isIncludeStates()))
        //                  {
        //                      outputAttrs.add(attr.mName);
        //                      continue mainLoop;
        //                  }
        //              }
        //          }
        //      }
        //  }

        private void fixupBindingExpressions(Attr a)
        {
            //Add the spacing within braces if this appears to be a binding to a method call.  There could be multiple
            //binding expressions, so we should look for brace pairs in the string.

            //don't process for non-mxml files
            if (isPlainXML())
                return;

            if (a.mValue == null)
                return;

            //kick out quickly if we don't have any potential brace pairs at all
            int startBrace = a.mValue.IndexOf('{');
            if (startBrace < 0)
                return;

            String output = "";
            for (int i = 0; i < a.mValue.Length; i++)
            {
                char c = a.mValue[i];
                output += c; //always append the character
                if (c == '{')
                {
                    //find end brace and process between them
                    int endPos = (-1);
                    for (int k = i + 1; k < a.mValue.Length; k++)
                    {
                        char newC = a.mValue[k];
                        if (newC == '}')
                        {
                            endPos = k;
                            break;
                        }
                        else if (newC == '\\')
                        {
                            k++; //skip over next char
                        }
                    }

                    //only if we found an end pos do we need to do anything
                    if (endPos >= 0)
                    {
                        String data = a.mValue.Substring(i + 1, endPos - (i + 1));
                        data = AntlrUtilities.asTrim(data);

                        //format data string here to handle internal spacing
                        if (isFormatBoundAttributes() && mDoFormat)
                        {
                            try
                            {
                                //NOTE: the validation code below will fail if braces are added.  Hopefully people won't be doing
                                //any control structures in an attribute string.
                                mASPrinter.setDoFormat(mDoFormat);
                                mASPrinter.setData(data);
                                String newValue = mASPrinter.print(0);
                                if ((mASPrinter.getParseErrors() == null || mASPrinter.getParseErrors().Count == 0) && newValue != null && newValue.Length > 0 && newValue.IndexOf('\n') < 0) //I don't really want to add carriage returns
                                {
                                    data = AntlrUtilities.asTrim(newValue); //we don't want to keep any whitespace on the ends; only internal whitespace
                                }
                            }
                            catch {}
                        }

                        String spaceString = ASFormatter.generateSpaceString(mSpacesInsideAttrBraces);
                        output += spaceString;
                        output += data;
                        output += spaceString;
                        output += '}';
                        i = endPos; //reset i so that the whole block will be skipped.
                    }
                }
                else if (c == '\\')
                {
                    i++; //skip over next char; might be an escaped '{' or something else
                    output += a.mValue[i]; //append next char too
                }
            }

            //verify that the strings are equal
            if (ASFormatter.validateNonWhitespaceIdentical(a.mValue, output))
            {
                a.mValue = output;
            }

        }

        private bool matchesRegEx(String text, List<String> tagsWithBlankLinesBeforeThem)
        {
            foreach (String tag in tagsWithBlankLinesBeforeThem)
            {
                if (AttrGroup.isRegexString(tag))
                {
                    if (Regex.Matches(tag, text).Count > 0)
                        return true;
                }
            }

            return false;
        }

        private bool matchesSpec(String testAttrName, String groupAttrSpec, bool isRegex, bool includeStateAttributes)
        {
            try
            {
                bool matches = testAttrName == groupAttrSpec;
                if (!matches && isRegex)
                {
                    matches = Regex.Matches(groupAttrSpec, testAttrName).Count > 0;
                }
                if (!matches && includeStateAttributes)
                {
                    matches = Regex.Matches(groupAttrSpec + StateRegexSuffix, testAttrName).Count > 0;
                }
                return matches;
            }
            catch {}

            return false;
        }

        private int getLastLineColumnLength(StringBuilder buffer)
        {
            int lastCR = buffer.ToString().LastIndexOf('\n');
            String lastLine = null;
            if (lastCR < 0)
                lastLine = buffer.ToString();
            else
                lastLine = buffer.ToString().Substring(lastCR + 1);
            int columnCount = 0;
            for (int i = 0; i < lastLine.Length; i++)
            {
                char c = lastLine[i];
                if (c == '\t')
                {
                    int remainder = columnCount % getTabSize();
                    if (remainder == 0)
                        columnCount += getTabSize();
                    else
                    {
                        columnCount += remainder;
                    }
                }
                else
                {
                    columnCount++;
                }
            }

            return columnCount;
        }

        public void addIndentIfAtStartOfLine(StringBuilder buffer, bool force)
        {
            if (mRearrangeOnly && !force)
                return;

            if (!mSkipNextIndent && ASFormatter.isLineEmpty(buffer))
            {
                buffer.Append(generateIndent(mCurrentIndent));
            }

            mSkipNextIndent = false;
        }

        private String generateIndent(int spaces)
        {
            return ASFormatter.generateIndent(spaces, mUseTabs, mTabSize);
        }

        public List<Exception> getParseErrors()
        {
            List<Exception> allErrors = new List<Exception>();
            if (mParseErrors != null)
                allErrors.AddRange(mParseErrors);
            if (mASPrinter.getParseErrors() != null)
                allErrors.AddRange(mASPrinter.getParseErrors());
            return allErrors;
        }

        public void setDoFormat(bool b)
        {
            mDoFormat = b;
        }

        public bool isDoFormat()
        {
            return mDoFormat;
        }

        public ASPrettyPrinter getASPrinter()
        {
            return mASPrinter;
        }

        public int getIndentAmount()
        {
            return mIndentAmount;
        }

        public void setIndentAmount(int indentAmount)
        {
            mIndentAmount = indentAmount;
        }

        public int getSpacesAroundEquals()
        {
            return mSpacesAroundEquals;
        }

        public void setSpacesAroundEquals(int spacesAroundEquals)
        {
            mSpacesAroundEquals = spacesAroundEquals;
        }

        public bool isSortOtherAttrs()
        {
            return mSortOtherAttrs;
        }

        public void setSortOtherAttrs(bool sortOtherAttrs)
        {
            mSortOtherAttrs = sortOtherAttrs;
        }

        public void setAttrSortMode(int sortMode)
        {
            mAttrOrderMode = sortMode;
        }

        public int getAttrSortMode()
        {
            return mAttrOrderMode;
        }

        public void setManualAttrSortData(List<String> attrOrder)
        {
            if (attrOrder == null)
                mManualAttrSortOrder.Clear();
            else
                mManualAttrSortOrder = attrOrder;
        }

        public List<String> getManualAttrSortData()
        {
            return mManualAttrSortOrder;
        }
        public int getMaxLineLength()
        {
            return mMaxLineLength;
        }

        public void setMaxLineLength(int maxLineLength)
        {
            mMaxLineLength = maxLineLength;
        }

        public Point getSelectedRange()
        {
            return mSelectedRange;
        }

        public void setSelectedRange(Point selectedRange)
        {
            mSelectedRange = selectedRange;
        }

        public Point getOutputRange()
        {
            return mOutputRange;
        }

        public Point getReplaceRange()
        {
            return mReplaceRange;
        }

        public int getAttrsPerLine()
        {
            return mAttrsPerLine;
        }

        public void setAttrsPerLine(int attrsPerLine)
        {
            mAttrsPerLine = attrsPerLine;
        }

        public int getWrapMode()
        {
            return mWrapMode;
        }

        public void setWrapMode(int wrapMode)
        {
            mWrapMode = wrapMode;
        }

        public bool isKeepBlankLines()
        {
            return mKeepBlankLines;
        }

        public void setKeepBlankLines(bool keepBlankLines)
        {
            mKeepBlankLines = keepBlankLines;
        }
        public int getWrapStyle()
        {
            return mWrapStyle;
        }

        public void setWrapStyle(int style)
        {
            mWrapStyle = style;
        }

        public void setTagsThatCanBeFormatted(List<String> tagNames)
        {
            mTagsWhoseTextContentsCanBeFormatted.Clear();
            mTagsWhoseTextContentsCanBeFormatted.AddRange(tagNames);
        }

        public List<String> getTagsThatCanBeFormatted()
        {
            return mTagsWhoseTextContentsCanBeFormatted;
        }

        public void setTagsThatCannotBeFormatted(List<String> tagNames)
        {
            mTagsWhoseTextContentCanNeverBeFormatted.Clear();
            mTagsWhoseTextContentCanNeverBeFormatted.AddRange(tagNames);
        }

        public List<String> getTagsThatCannotBeFormatted()
        {
            return mTagsWhoseTextContentCanNeverBeFormatted;
        }

        public void setAttrGroups(List<AttrGroup> attrGroups)
        {
            mHashedGroupAttrs = new Dictionary<String, List<String>>();
            mAttrGroups = new Dictionary<String, AttrGroup>();
            foreach (AttrGroup group in attrGroups)
            {
                mAttrGroups.Add(group.getName(), group);
                List<String> attrSet = new List<String>();
                foreach (String attr in group.getAttrs())
                {
                    attrSet.Add(attr);
                }
                mHashedGroupAttrs.Add(group.getName(), attrSet);
            }


        }

        public static String isGroupAttr(String attr)
        {
            if (attr.Length >= 2 && attr.StartsWithOrdinal(Attr_Group_Marker) && attr.EndsWithOrdinal(Attr_Group_Marker))
                return attr.Substring(1, (attr.Length - 1) - 1);
            return null;
        }

        public void setAddNewlineAfterLastAttr(bool addNewlineAfterLastAttr)
        {
            mAddNewlineAfterLastAttr = addNewlineAfterLastAttr;
        }

        public void setIndentCloseTag(bool value)
        {
            mIndentCloseTag = value;
        }

        public Dictionary<Int32, ReplacementRange> getReplaceMap()
        {
            return mReplaceMap;
        }

        public bool isUseAttrsToKeepOnSameLine()
        {
            return mUseAttrsToKeepOnSameLine;
        }

        public void setUseAttrsToKeepOnSameLine(bool useAttrsToKeepOnSameLine)
        {
            mUseAttrsToKeepOnSameLine = useAttrsToKeepOnSameLine;
        }

        public int getAttrsToKeepOnSameLine()
        {
            return mAttrsToKeepOnSameLine;
        }

        public void setAttrsToKeepOnSameLine(int attrsToKeepOnSameLine)
        {
            mAttrsToKeepOnSameLine = attrsToKeepOnSameLine;
        }

        public int getSpacesBeforeEmptyTagEnd()
        {
            return mSpacesBeforeEmptyTagEnd;
        }

        public void setSpacesBeforeEmptyTagEnd(int spacesBeforeEmptyTagEnd)
        {
            mSpacesBeforeEmptyTagEnd = spacesBeforeEmptyTagEnd;
        }

        public List<String> getTagsWithBlankLinesBeforeThem()
        {
            return mTagsWithBlankLinesBeforeThem;
        }

        public void setTagsWithBlankLinesBeforeThem(List<String> tagsWithBlankLinesBeforeThem)
        {
            mTagsWithBlankLinesBeforeThem.Clear();
            mTagsWithBlankLinesBeforeThem.AddRange(tagsWithBlankLinesBeforeThem);
        }

        public int getBlankLinesBeforeTags()
        {
            return mBlankLinesBeforeTags;
        }

        public void setBlankLinesBeforeTags(int blankLinesBeforeTags)
        {
            mBlankLinesBeforeTags = blankLinesBeforeTags;
        }

        public int getBlankLinesAfterParentTags()
        {
            return mBlankLinesAfterParentTags;
        }

        public void setBlankLinesAfterSpecificParentTags(int blankLinesAfterTags)
        {
            mBlankLinesAfterParentTags = blankLinesAfterTags;
        }

        public int getBlankLinesBeforeCloseTags()
        {
            return mBlankLinesBeforeCloseTags;
        }

        public void setBlankLinesBeforeCloseTags(int count)
        {
            mBlankLinesBeforeCloseTags = count;
        }

        public List<String> getASScriptTags()
        {
            return mASScriptTags;
        }

        public void setASScriptTags(List<String> scriptTags)
        {
            mASScriptTags.Clear();
            mASScriptTags.AddRange(scriptTags);
        }

        public bool isRequireCDATAForASContent()
        {
            return mRequireCDATAForASContent;
        }

        public void setRequireCDATAForASContent(bool requireCDATAForASContent)
        {
            mRequireCDATAForASContent = requireCDATAForASContent;
        }

        public int getSpacesBetweenSiblingTags()
        {
            return mSpacesBetweenSiblingTags;
        }

        public void setSpacesBetweenSiblingTags(int spacesBetweenSiblingTags)
        {
            mSpacesBetweenSiblingTags = spacesBetweenSiblingTags;
        }

        public int getSpacesAfterParentTags()
        {
            return mSpacesAfterParentTags;
        }

        public void setSpacesAfterParentTags(int spacesAfterParentTags)
        {
            mSpacesAfterParentTags = spacesAfterParentTags;
        }

        public bool isPlainXML()
        {
            return mIsPlainXML;
        }

        public void setPlainXML(bool isPlainXML)
        {
            mIsPlainXML = isPlainXML;
        }

        public List<String> getParentTagsWithBlankLinesAfterThem()
        {
            return mParentTagsWithBlankLinesAfterThem;
        }

        public void setParentTagsWithBlankLinesAfterThem(List<String> parentTagsWithBlankLinesAfterThem)
        {
            mParentTagsWithBlankLinesAfterThem = parentTagsWithBlankLinesAfterThem;
        }

        /*public IPreferenceStore getStore() {
            return mStore;
        }

        public void setStore(IPreferenceStore store) {
            mStore = store;
        }*/

        public void setObeyMaxLineLength(bool obey)
        {
            mAlwaysObeyMaxLineLength = obey;
        }

        public void setRearrangeOnlyMode(bool rearrangeOnly)
        {
            mRearrangeOnly = rearrangeOnly;
        }

        public void disableMultiPassMode()
        {
            mAllowMultiplePasses = false;
            mASPrinter.disableMultiPassMode();
        }

        public void setData(String data)
        {
            mSource = data;
        }

        public bool needAnotherPass()
        {
            //no notion of needing another pass at mxml level, but as printer might require it
            return mNeedAnotherPass;
        }

        public void setHangingIndentTabs(int tabCount)
        {
            mHangingIndentSize = tabCount;
        }

        public int getBlankLinesBeforeComments()
        {
            return mBlankLinesBeforeComments;
        }

        public void setBlankLinesBeforeComments(int mBlankLinesBeforeComments)
        {
            this.mBlankLinesBeforeComments = mBlankLinesBeforeComments;
        }

        public bool isUsePrivateTags()
        {
            return mUsePrivateTags;
        }

        public void setUsePrivateTags(bool usePrivateTags)
        {
            this.mUsePrivateTags = usePrivateTags;
        }

        public List<String> getPrivateTags()
        {
            return mPrivateTags;
        }

        public void setPrivateTags(List<String> privateTags)
        {
            this.mPrivateTags = privateTags;
        }

        public bool isUseSpacesInsideAttrBraces()
        {
            return mUseSpacesInsideAttrBraces;
        }

        public void setUseSpacesInsideAttrBraces(bool useSpacesInsideAttrBraces)
        {
            mUseSpacesInsideAttrBraces = useSpacesInsideAttrBraces;
        }

        public int getSpacesInsideAttrBraces()
        {
            return mSpacesInsideAttrBraces;
        }

        public void setSpacesInsideAttrBraces(int spacesInsideAttrBraces)
        {
            mSpacesInsideAttrBraces = spacesInsideAttrBraces;
        }

        public bool isFormatBoundAttributes()
        {
            return mFormatBoundAttributes;
        }

        public void setFormatBoundAttributes(bool formatBoundAttributes)
        {
            mFormatBoundAttributes = formatBoundAttributes;
        }

        public bool isKeepRelativeCommentIndent()
        {
            return mKeepRelativeCommentIndent;
        }

        public void setKeepRelativeCommentIndent(bool value)
        {
            mKeepRelativeCommentIndent = value;
        }

        public int getCDATAIndentTabs()
        {
            return mCDATAIndentTabs;
        }

        public void setCDATAIndentTabs(int tabs)
        {
            mCDATAIndentTabs = tabs;
        }

        public int getScriptIndentTabs()
        {
            return mScriptIndentTabs;
        }

        public void setScriptIndentTabs(int tabs)
        {
            mScriptIndentTabs = tabs;
        }

        public bool isKeepCDataOnSameLine()
        {
            return mKeepCDataOnSameLine;
        }

        public void setKeepCDataOnSameLine(bool keep)
        {
            mKeepCDataOnSameLine = keep;
        }

        public int getBlankLinesAtCDataStart()
        {
            return mBlankLinesAtCDataStart;
        }

        public void setBlankLinesAtCDataStart(int blankLinesAtCDataStart)
        {
            mBlankLinesAtCDataStart = blankLinesAtCDataStart;
        }

        public int getBlankLinesAtCDataEnd()
        {
            return mBlankLinesAtCDataEnd;
        }

        public void setBlankLinesAtCDataEnd(int blankLinesAtCDataEnd)
        {
            mBlankLinesAtCDataEnd = blankLinesAtCDataEnd;
        }

    }

    public class TagStackEntry
    {
        private String mTagName;
        private bool mSeenFirstChild;
        public TagStackEntry(String tagName)
        {
            mTagName = tagName;
            mSeenFirstChild = false;
        }
        public String getTagName()
        {
            return mTagName;
        }
        public bool isSeenFirstChild()
        {
            return mSeenFirstChild;
        }
        public void setSeenFirstChild()
        {
            mSeenFirstChild = true;
        }
    }

    public class Attr : IComparable<Attr>
    {
        public String mName;
        public String mValue;
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

    //holds mapping of an attribute and the attribute spec that matched it.
    public class AttrMapping
    {
        public AttrMapping(Attr attr, String spec)
        {
            mAttr = attr;
            mAttrSpec = spec;
        }
        public static bool hasAttr(List<AttrMapping> cachedAttrs, String attrSpec)
        {
            foreach (AttrMapping attrMapping in cachedAttrs)
            {
                if (attrMapping.mAttrSpec == attrSpec)
                    return true;
            }
            return false;
        }

        public Attr mAttr;
        public String mAttrSpec;
    }

}