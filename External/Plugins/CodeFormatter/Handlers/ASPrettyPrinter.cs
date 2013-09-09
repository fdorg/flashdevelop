using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using CodeFormatter.InfoCollector;
using Antlr.Runtime.Tree;
using Antlr.Runtime;
using Antlr.Utility;
using System.Text.RegularExpressions;

namespace CodeFormatter.Handlers
{
	public class ASPrettyPrinter
	{
		private const int AddCRIfBeyondMaxCol = 8;
		private const int AddCR = 9;
		private const int AddCR_BlankLine = 10;
		private const int ADD_WS = 11;
		public const int INDENT_LIKE_SUBITEM = 16;
		public const int INDENT_TO_WRAP_ELEMENT = 32;
		public const int FORMAT_ALL = 100;
		public const int FORMAT_INDENT = 101;
		public const int FORMAT_NOCRs = 102;
		public const int FORMAT_OnlyAddCRs = 103; // Not implemented yet
		public const int BraceStyle_Sun = 4;
		public const int BraceStyle_Adobe = 5;

        private bool mIndentMultilineComments = true;
		private bool mIsKeepingExcessDeclWhitespace = false;
		private bool mInParameterDecl = false;
		private int mIndentStyle = INDENT_LIKE_SUBITEM;
		private int mBlockIndent;
		private bool mCRBeforeOpenBrace;
		private bool mDoFormat;
		private bool mTrimTrailingWS = true;
		private int mSpacesAfterLabel = 1;
		private int mSpacesAroundAssignment = 1;
		private int mAdvancedSpacesAroundAssignmentInOptionalParameters = 1;
		private bool mUseAdvancedSpacesAroundAssignmentInOptionalParameters = false;
		private int mSpacesAroundColons = 0;
		private int mAdvancedSpacesBeforeColons = 0;
		private int mAdvancedSpacesAfterColons = 0;
		private bool mUseGlobalSpacesAroundColons;
		private int mSpacesBeforeComma = 0;
		private int mSpacesAfterComma = 1;
		private int mSpacesInsideParensEtc = 1;
		private bool mAlwaysGenerateIndent = false;
		private bool mIndentAtPackageLevel = true; // If true, normal indent; false means don't indent imports and class declarations, but leave them at column 0
        private bool mUseBraceStyleSetting = true;
        private int mBraceStyleSetting = BraceStyle_Adobe;
        private bool mUseSpacesInsideParensEtc = true;
        private int mAdvancedSpacesInsideParens = 1;
        private int mAdvancedSpacesInsideArrayDeclBrackets = 1;
        private int mAdvancedSpacesInsideArrayReferenceBrackets = 1;
        private int mAdvancedSpacesInsideObjectBraces = 1;
		private bool mMultipleBindableItems;
        private int mBlankLinesToKeep = 0;
        private bool mLastBindableWasConditionalTag = false;
        private bool mProcessingBindableTag = false;
        private bool mCollapseSpaceForAdjacentParens = true;
        private int mTabSize = 3;
        private bool mUseTabs = true;
        private bool mKeepBlankLines = true;
        private int mExpressionSpacesAroundSymbolicOperators = 1;
        private bool mKeepElseIfOnSameLine = true;
        private int mMaxLineLength = (-1);
        private int mBlankLinesBeforeFunction = 1;
        private int mBlankLinesBeforeClass = 1;
        private int mBlankLinesBeforeControlStatement = 1;
        private int mBlankLinesBeforeProperties = 0;
        private bool mKeepSingleLineCommentsAtColumn1 = true;
        private bool mCRBeforeElse = false;
        private bool mCRBeforeCatch = false;
        private String mSourceData = null;
        private String mWorkingSource = null;
        private bool mEmptyStatementsOnNewLine = true;
		private List<int> mAddedCRs;
		private CommonTokenStream mRawTokens;
        private CommonToken mLastToken = null;
        private bool mIsNewlineAfterBindable = true;
		private List<Exception> mParseErrors;
		private StringBuilder mOutputBuffer;
		private List<IndentType> mIndentStack;
		private List<int> mFormatTypeStack;
        private WrapOptions mArrayInitWrapOptions = null;
        private WrapOptions mMethodCallWrapOptions = null;
        private WrapOptions mMethodDeclWrapOptions = null;
        private WrapOptions mExpressionWrapOptions = null;
        private WrapOptions mXMLWrapOptions = null;
        private bool mCanAddCRsAtBlockStart = false;
        private int mSpacesBetweenControlKeywordsAndParens = 1;
		private bool mFirstTokenBeforeFormatIndent;
		private bool mBindableMode;
		private int mBindablePos;
		private bool mNextTokenInElseIfState;
		private bool mElseIfNeedToLoseIndent;
		private int mExcludeStackCount;
		private int mLazyIndentType;
		private int mLazyFormatType;
		private Point mSelectedRange; //=null; //x=start line, y=end line (1-based)(lines 5 and 6 would be 5,6)
		private Point mOutputRange; //=null; //x=start offset, y=end offset (0-based offsets into outputbuffer)
		private Point mReplaceRange; //=null; //same semantics as mOutputRange
		private bool mInE4XText;
        private Regex mReLine = new Regex("[^\n]+$", RegexOptions.RightToLeft);
	
		private void InitSettings()
		{
			mBlockIndent = 3;
			mCRBeforeOpenBrace = true;
			mDoFormat = true;
			mArrayInitWrapOptions = new WrapOptions(WrapOptions.WRAP_NONE);
			mMethodCallWrapOptions = new WrapOptions(WrapOptions.WRAP_NONE);
			mMethodDeclWrapOptions = new WrapOptions(WrapOptions.WRAP_NONE);
			mExpressionWrapOptions = new WrapOptions(WrapOptions.WRAP_NONE);
			mXMLWrapOptions = new WrapOptions(WrapOptions.WRAP_DONT_PROCESS);
		}
		
		public ASPrettyPrinter(bool isCompleteFile, String sourceData)
		{
			InitSettings();
			mSourceData = sourceData;
		}
		
		public int GetBlockIndent()
		{
			return mBlockIndent;
		}

		public String Print(int startIndent)
		{
			mMultipleBindableItems = false;
			mBindablePos = (-1);
			mBindableMode = false;
			mFirstTokenBeforeFormatIndent = false;
			mNextTokenInElseIfState = false;
			mElseIfNeedToLoseIndent = false;
			mLazyIndentType = (-1);
			mLazyFormatType = (-1);
			mFormatTypeStack = new List<int>();
			if (mDoFormat) PushFormatMode(FORMAT_ALL);
			else PushFormatMode(FORMAT_INDENT);
			mInE4XText = false;
			mSourceData = CodeFormatter.InfoCollector.Utilities.ConvertCarriageReturnsToLineFeeds(mSourceData);
			bool addedEndCRs = false;
			if (GetEndEOLs(mSourceData) < 2)
			{
				mSourceData += "\n\n";
				addedEndCRs = true;
			}
			mAddedCRs = new List<int>();
			mParseErrors = null;
			mIndentStack = new List<IndentType>();
			mIndentStack.Add(new IndentType(INITIAL_INDENT, startIndent));
			mExcludeStackCount = 0;
			mOutputBuffer = new StringBuilder();
			try
			{
				mWorkingSource = mSourceData;
				ANTLRStringStream stream = new ANTLRStringStream(mWorkingSource);
				AS3_exLexer l2 = new AS3_exLexer(stream);
				mRawTokens = new CommonTokenStream(l2);
				AS3_exParser p2 = new AS3_exParser(this, mRawTokens);
				bool isPackageFound=false;
				while (true)
				{
					CommonToken t = (CommonToken)l2.NextToken();
					if (t == null) break;
					if (t.Channel == Token.DEFAULT_CHANNEL)
					{
						if (t.Type == AS3_exParser.PACKAGE) isPackageFound=true;
						break;
					}
				}
				l2.Reset();
				p2.TokenStream = mRawTokens;
				if (isPackageFound)
				{
					AS3_exParser.fileContents_return retVal = p2.fileContents();
				}
				else p2.mxmlEmbedded();
				// Handle any remaining hidden tokens
				CommonToken tk = new CommonToken(AS3_exParser.EOF, "");
				tk.TokenIndex = mRawTokens.Count;
				Emit(tk);
				String resultText = mOutputBuffer.ToString();
				if (resultText == null) return null;
				if (ASFormatter.ValidateNonWhitespaceIdentical(resultText, mWorkingSource))
				{
					// Trim extra newlines at the end of result text
					if (mDoFormat)
					{
						int oldEndLines = GetEndEOLs(mWorkingSource) - (addedEndCRs ? 2 : 0); // I may have added two above
						int newEndLines = GetEndEOLs(resultText);
						if (newEndLines > oldEndLines)
						{
							resultText = resultText.Substring(0, resultText.Length - (newEndLines - oldEndLines));
							if (mOutputRange != null && mOutputRange.Y > resultText.Length)
							{
								mOutputRange.Y = resultText.Length;
							}
						}
					}
					// If we are trying to capture the output range but haven't captured it yet
					if (mOutputRange != null && mOutputRange.Y < 0)
					{
						mOutputRange.Y = resultText.Length;
						mReplaceRange.Y = mWorkingSource.Length;
						if (mDoFormat && addedEndCRs) mReplaceRange.Y -= "\n\n".Length;
					}
					return resultText;
				}
				else
				{
					mParseErrors = new List<Exception>();
					mParseErrors.Add(new Exception("Internal error: Formatted text doesn't match source. " + resultText + "!=" + mWorkingSource));
				}
			}
			finally {}
			return null;
		}
		
		private static int GetEndEOLs(String source)
		{
			int count = 0;
			for (int i = source.Length - 1; i >= 0; i--)
			{
				if (source[i] == '\n') count++;
				else if (Char.IsWhiteSpace(source[i])) continue;
				else break;
			}
			return count;
		}
		
		private void PrintNecessaryWS(StringBuilder buffer)
		{
			int currentBlankLines = ASFormatter.GetNumberOfEmptyLinesAtEnd(buffer);
			foreach (int crCode in mAddedCRs)
			{
				if (crCode == AddCR)
				{
					if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
					{
						InsertCRIntoBuffer(buffer);
					}
				}
				else if (crCode == AddCR_BlankLine)
				{
					if (currentBlankLines>0) currentBlankLines--;
					else
					{
						if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
						{
							InsertCRIntoBuffer(buffer);
						}
						InsertCRIntoBuffer(buffer);
					}
				}
				else if (crCode == ADD_WS)
				{
					if (!ASFormatter.IsOnlyWhitespaceOnLastLine(buffer))
					{
						buffer.Append(' ');
					}
				}
				else if (crCode==AddCRIfBeyondMaxCol)
				{
					if (GetMaxLineLength() > 0 && DetermineLastLineLength(buffer, GetTabSize()) >= GetMaxLineLength())
					{
						InsertCRIntoBuffer(buffer);
					}
				}
			}
			mAddedCRs.Clear();
		}
		
		private void InsertCRIntoBuffer(StringBuilder buffer)
		{
			if (mTrimTrailingWS && GetFormatMode() != FORMAT_INDENT)
			{
				// This is code to trim trailing whitespace on lines.
				for (int i = buffer.Length - 1; i >= 0; i--)
				{
                    char c = buffer[i];
					if (c == ' ' || c == '\t') buffer.Remove(i,1);
					else break;
				}
			}
			if (IsAlwaysGenerateIndent())
			{
				// If current line is blank, then try to generate what would be the next indent
				AddIndentIfAtStartOfLine(buffer);
			}
			buffer.Append('\n');
		}

		public static int DetermineLastLineLength(StringBuilder buffer, int tabSize)
		{
            int i = buffer.Length - 1;
			while (i >= 0)
			{
                char c = buffer[i];
				if (c == '\n' || c == '\r')
				{
					i++;
					break;
				}
				i--;
			}
            if (i < 0 || i >= buffer.Length) return 0;
			String lastLine = buffer.ToString().Substring(i);
			int lastLineLength = 0;
			for (i = 0; i < lastLine.Length; i++)
			{
				char c = lastLine[i];
				if (c == '\t')
				{
					int leftOver = lastLineLength % tabSize;
					if (leftOver == 0) lastLineLength += tabSize;
					else lastLineLength += leftOver;
				}
				else lastLineLength++;
			}
			return lastLineLength;
		}

		public void InsertWS(int amt)
		{
			if (GetFormatMode() == FORMAT_INDENT) return;
			for (int i = 0; i < amt; i++)
			{
				mAddedCRs.Add(ADD_WS);
			}
		}
		
		public void InsertCRBeyondMaxCol()
		{
			if (GetFormatMode() != FORMAT_ALL && GetFormatMode() != FORMAT_OnlyAddCRs) return;
			mAddedCRs.Add(AddCRIfBeyondMaxCol);
		}
		
		public void InsertCR(bool doOverride)
		{
			if (GetFormatMode() != FORMAT_ALL && GetFormatMode() != FORMAT_OnlyAddCRs) return;
			int i = AddCR;
			if (doOverride) i = AddCR_BlankLine;
			mAddedCRs.Add(i);
		}

		String GenerateIndent(int spaces)
		{
			return ASFormatter.GenerateIndent(spaces, mUseTabs, mTabSize);
		}
		
		public int DetermineLastIndent(StringBuilder buffer)
		{
            String raw = buffer.ToString();
            int spaceCount = 0;
            int len = raw.Length;
            do
            {
                Match m = mReLine.Match(raw, 0, len);
                if (!m.Success) break;
                len = m.Index;
                String line = m.Value;
                // Skip to next (i.e. previous) line if this line starts with a line comment
                if (line.Trim().StartsWith("//")) continue;
                // count indent
                for (int k = 0; k < line.Length; k++)
                {
                    char c = line[k];
                    if (!Char.IsWhiteSpace(c))
                    {
                        return spaceCount;
                    }
                    else
                    {
                        if (c == ' ') spaceCount++;
                        else if (c == '\t')
                        {
                            int remainder = spaceCount % GetTabSize();
                            if (remainder == 0) spaceCount += GetTabSize();
                            else spaceCount += remainder;
                        }
                    }
                }
            }
            while (true);

			return GetCurrentIndent();
		}

		public int GetIndentForNextLine(StringBuilder buffer)
		{
			int lastIndent = DetermineLastIndent(buffer);
			int currentIndent = GetCurrentIndent();
			int newIndent = currentIndent;
			//String text = buffer.ToString().Trim(); // TODO: something more efficient
			//if (text.Length>0)
            if (buffer.Length > currentIndent)
			{
				newIndent = Math.Min(currentIndent, lastIndent);
				if (lastIndent < currentIndent)
				{
					newIndent = lastIndent + GetBlockIndent(); // (mUnindent ? 0 : mPrinter.GetBlockIndent());
				}
			}
			return newIndent;
		}
		
		
		public void AddIndentIfAtStartOfLine(StringBuilder buffer)
		{
			if (ASFormatter.IsLineEmpty(buffer))
			{
				int newIndent=GetIndentForNextLine(buffer);
				buffer.Append(GenerateIndent(newIndent));
			}
		}
		
		public void SetDoFormat(bool doFormat)
		{
			mDoFormat = doFormat;
		}
		
		public List<Exception> GetParseErrors() 
        {
			return mParseErrors;
		}

		public void SetData(String text) 
        {
			mSourceData=text;
		}

		public bool IsCRBeforeOpenBrace() 
        {
			if (IsUseBraceStyleSetting())
			{
				if (GetBraceStyleSetting()==BraceStyle_Sun) return false;
				else return true;
			}
			return mCRBeforeOpenBrace;
		}

		public void SetCRBeforeOpenBrace(bool braceOnNewLine) 
        {
			mCRBeforeOpenBrace = braceOnNewLine;
		}
		
		public const String mStartExcludeProcessing = "{CodeFormatter:Off}";
        public const String mStopExcludeProcessing = "{CodeFormatter:On}";
		
		public void Emit(CommonToken tok)
		{
			// Look at hidden tokens to see if the tag to turn off formatting is seen.  If so, then just Emit all the hidden tokens and
			// the main token.  Want to let operations happen that don't insert text and maintain state that we need to keep.
			int currentBufferOffset = mOutputBuffer.Length;
			mLastToken = tok;
			// Handle whitespace
			CommonTokenStream tokens = mRawTokens; // (CommonTokenStream)getTokenStream();
			int currentTokenIndex = ((CommonToken)tok).TokenIndex - 1;
			List<CommonToken> hiddenTokens=new List<CommonToken>();
			// Collect all of the hidden tokens since the last non-whitespace token
			while (currentTokenIndex>=0)
			{
				CommonToken t = (CommonToken)tokens.Get(currentTokenIndex);
				if (t.Channel == Token.DEFAULT_CHANNEL) break;
				hiddenTokens.Add(t);
				currentTokenIndex--;
			}
			hiddenTokens.Reverse();
			if (!mProcessingBindableTag && mBindableMode && mBindablePos >= 0 && GetFormatMode() == FORMAT_ALL)
			{
				//TODO: figure out how a partial range will figure into this.  I may have to shift the position if it's already been set
                while (mBindablePos < mOutputBuffer.Length)
				{
                    char c = mOutputBuffer[mBindablePos];
					if (!Char.IsWhiteSpace(c)) break;
					mBindablePos++;
				}
				while (mBindablePos>0)
				{
                    char c = mOutputBuffer[mBindablePos - 1];
					if (c == '\n') break;
					mBindablePos--;
				}
                String bindableText = mOutputBuffer.ToString().Substring(mBindablePos);
                mOutputBuffer.Remove(mBindablePos, mOutputBuffer.Length - mBindablePos);
				// This should spit out the blank lines before function and similar whitespace
				PrintNecessaryWS(mOutputBuffer);
				mOutputBuffer.Append(bindableText);
				mBindableMode = false;
				mBindablePos = (-1);
				// Check for one of the next hidden tokens being a comment.  If so, we have to add a newline.
				bool commentFollows = false;
				foreach (CommonToken token in hiddenTokens) 
                {
					if (token.Text.Trim().Length>0)
					{
						commentFollows=true;
						break;
					}
				}
				// Now, spit out either newline or space to separate bindable tag from bound item
				if (IsNewlineAfterBindable() || mMultipleBindableItems || commentFollows || mLastBindableWasConditionalTag)
				{
					InsertCR(false);
				}
				else InsertWS(1);
			}
			// Handle user selection range
			if (mSelectedRange != null && !mBindableMode)
			{
				if (mOutputRange == null)
				{
					if (tok.Line >= mSelectedRange.X)
					{
						mOutputRange=new Point(mOutputBuffer.Length, -1);
						mReplaceRange=new Point(0, -1);
						CommonToken firstToken = tok;
						if (hiddenTokens.Count > 0)
						{
							firstToken = (hiddenTokens.ToArray())[0];
						}
						mReplaceRange.X = ((CommonToken)firstToken).StartIndex;
					}
				}
				else
				{
					if (mOutputRange.Y < 0 && tok.Line > mSelectedRange.Y)
					{
						mOutputRange.Y = mOutputBuffer.Length;
						CommonToken firstToken=tok;
						if (hiddenTokens.Count>0)
						{
							firstToken = (hiddenTokens.ToArray())[0];
						}
						mReplaceRange.Y = ((CommonToken)firstToken).StartIndex;
					}
				}
			}
			bool onBlankLine = ASFormatter.IsOnlyWhitespaceOnLastLine(mOutputBuffer); // I think I can only be on a blank line at the start of the document
			int blankLines = 0;
			bool needToEnsureNewLine = false;
			foreach (int ws in mAddedCRs) 
            {
				if (ws == AddCR || ws == AddCR_BlankLine)
				{
					needToEnsureNewLine=true;
					break;
				}
			}
			// Process hidden tokens (whitespace and comments) before this token
			bool seenCR = false;
			bool commentTextOnLastLine = false;
			foreach (CommonToken t in hiddenTokens)
			{
				switch (t.Channel)
				{
					case AS3_exParser.CHANNEL_EOL:
						// If we're not formatting at all, then just add the cr.
						if (IsFormatterOff())
						{
							InsertCRIntoBuffer(mOutputBuffer);
							break;
						}
						seenCR = true;
						// We only want to preserve blank lines (or possibly not preserve blank lines) if we're in format mode where
						// we're taking control of the whitespace.  However, the format mode may have already been changed to one that deletes
						// newlines, but shouldn't take effect until after the first token is seen.  This is just an artifact of the
						// whitespace tokens not being handled in the parser until the next parser token is seen.
						if (GetFormatMode() == FORMAT_ALL || (IsDoFormat() && IsFirstTokenBeforeFormatIndent() && (!commentTextOnLastLine))) // We want to process newlines if we are doing full format or
						{
							if (IsKeepBlankLines() || GetBlankLinesToKeep() > blankLines)
							{
								if (onBlankLine)
								{
									// We got a carriage return but we were already on a new line.  Don't add the initial carriage
									// return. This happens at the start of the file or after a line comment
									if (blankLines == 0 && !ASFormatter.IsOnlyWhitespaceOnLastLine(mOutputBuffer))
									{
										InsertCRIntoBuffer(mOutputBuffer);
									}
									blankLines++;
									InsertCRIntoBuffer(mOutputBuffer);
								}
								onBlankLine = true;
							}
						}
						else
						{
							// We're indenting (or formatting but without removing newlines), so just don't lose the CR
							InsertCRIntoBuffer(mOutputBuffer);
							onBlankLine = true;
						}
						commentTextOnLastLine = false;
						break;

					case AS3_exParser.CHANNEL_SLCOMMENT:
						// NOTE: the single line comment contains a carriage return, so we are guaranteed to
						// be on a new line afterward
						// Handling of "exclude from formatting" tags
						if (t.Text.IndexOf(mStartExcludeProcessing) >= 0)
						{
							PushFormatterOff();
						}
						else if (t.Text.IndexOf(mStopExcludeProcessing) >= 0)
						{
							PopFormatterOff();
						}
						// If not in format mode at all, then just Append the comment text, Clear any accumulated whitespace and exit case
						if (IsFormatterOff())
						{
							if (IsDoFormat() && !ASFormatter.IsLineEmpty(mOutputBuffer)) InsertCRIntoBuffer(mOutputBuffer);
							mOutputBuffer.Append(t.Text.Replace("\r\n", "\n"));
							mAddedCRs.Clear();
							break;
						}
						// If we've seen a carriage return in the whitespace, but we haven't added one yet (because the SLComment was the next line),
						// then we need to add a CR.
						if (onBlankLine) InsertCR(false);
						// Here, we check the mDoFormat flag, NOT the format stack, because we want to know if it's legal to add spaces, not
						// whether we are currently in a "don't format" block.  We might care if we were in code, but since this
						// is for comments I think it makes sense.
						if (IsDoFormat() && !seenCR) //if no carriage return before this comment, then it's on the same line with other text
						{
							if (LastCharIsNonWhitespace(mOutputBuffer)) mOutputBuffer.Append(' ');
							mOutputBuffer.Append(t.Text.Replace("\r\n", "\n"));
						}
						else
						{
							// Add a lazy space so that there will be one space before the '//' if there are preceding
							// chars on the line.  I don't know if this can ever have an effect, but it seems safest to leave it.
							InsertWS(1);
							// Otherwise, we add the accumulated whitespace, which may include multiple blank lines, as we push
							// the comment down to be with the following statement
							PrintNecessaryWS(mOutputBuffer);
							if (IsKeepSingleLineCommentsAtColumn1() && t.CharPositionInLine == 0)
							{
								// Do nothing, hopefully we've not added any whitespace to a blank line
							}
							else
							{
								bool addSpecialIndent = false;
								if (tok.Type==AS3_exParser.RCURLY) addSpecialIndent = true;
								if (addSpecialIndent) PushIndent(STATEMENT_INDENT);
								AddIndentIfAtStartOfLine(mOutputBuffer);
								if (addSpecialIndent) PopIndent();
							}
							mOutputBuffer.Append(t.Text.Replace("\r\n", "\n"));
						}
						onBlankLine = true;
						blankLines = 0; // Make sure we note that we have just seen a carriage return
						seenCR = true;
						commentTextOnLastLine = false;
						break;

					case AS3_exParser.CHANNEL_MLCOMMENT:
						// If not in format mode at all, then just Append the comment text, Clear any accumulated whitespace and exit switch
						if (IsFormatterOff())
						{
							mOutputBuffer.Append(t.Text.Replace("\r\n", "\n"));
							mAddedCRs.Clear();
							break;
						}
						// Here, we check the mDoFormat flag, NOT the format stack, because we want to know if it's legal to add spaces, not
						// whether we are currently in a "don't format" block.  We might care if we were in code, but since this is for comments I think it makes sense.
						// if the comment is at the end of a line of text, and the comment doesn't span multiple lines (and we're in a mode that allows space insertion)
						if (IsDoFormat() && !seenCR && t.Text.IndexOf('\n')<0)
						{
							if (LastCharIsNonWhitespace(mOutputBuffer)) mOutputBuffer.Append(' ');
							mOutputBuffer.Append(t.Text);
						}
						else
						{
							seenCR = true;
							// Add a lazy space so that there will be one space before the '/*' if there are preceding
							// chars on the line.  I don't even know if this can ever have an effect, but it seems safest to leave it.
							InsertWS(1);
							PrintNecessaryWS(mOutputBuffer);
							// TODO: something else completely, which might involve adding newlines, wrapping lines, etc.
							// when I wrote this for Java, I did some fancy code to determine the relative offset of
							// the original lines to maintain the indenting within the comment.
							String[] commentLines = t.Text.Split('\n');
							for (int j = 0; j < commentLines.Length; j++)
							{
								bool onLastLine = (j == commentLines.Length - 1);
								int indentAmount = GetIndentForNextLine(mOutputBuffer);
								String data = commentLines[j].Trim(); // This removes the \r, if it exists
								if (onLastLine && j > 0) // If we're on the last line and there's more than one line
								{
                                    if (mIndentMultilineComments) indentAmount++;
								}
								else if (j > 0)
								{
                                    // FIXED: Multiline comment indent support added
									String nextLine = data;
                                    if (nextLine.Length > 0 && nextLine[0] == '*')
                                    {
                                        if (mIndentMultilineComments) indentAmount += 1;
                                        else indentAmount += 0;
                                    }
                                    else indentAmount += 3;
								}
								// Only add indent if on an empty line
								if (data.Length > 0)
								{
									if (ASFormatter.IsLineEmpty(mOutputBuffer))
									{
										mOutputBuffer.Append(GenerateIndent(indentAmount));
									}
									mOutputBuffer.Append(data);
								}
								if (!onLastLine)
								{
									InsertCRIntoBuffer(mOutputBuffer);
								}
							}
						}
						onBlankLine = false;
						blankLines = 0;
						commentTextOnLastLine = true;
						break;

					case AS3_exParser.CHANNEL_WHITESPACE:
						if (IsFormatterOff())
						{
							// Just spit the text out
							mOutputBuffer.Append(t.Text);
						}
						else if (GetFormatMode() == FORMAT_INDENT)
						{
							// We don't care about whitespace if we are on the first token in an format-indent element,
                            // we only care about whitespace within the element
                            if ((commentTextOnLastLine || !IsFirstTokenBeforeFormatIndent()) && !ASFormatter.IsOnlyWhitespaceOnLastLine(mOutputBuffer))
                            {
                                mOutputBuffer.Append(t.Text);
                            }
						}
						else
						{
							// Do nothing, we'll add our own whitespace
						}
						break;

					default: break;
				}
			}
			mFirstTokenBeforeFormatIndent = false;
			// If still in non-formatting mode, then Clear any whitespace that has been added to our list
			if (IsFormatterOff()) mAddedCRs.Clear();
			PrintNecessaryWS(mOutputBuffer);
			if (!IsFormatterOff())
			{
				if ((GetFormatMode() == FORMAT_ALL || GetFormatMode() == FORMAT_OnlyAddCRs || (GetFormatMode() == FORMAT_INDENT && IsFirstTokenBeforeFormatIndent())) && needToEnsureNewLine)
				{
					if (!ASFormatter.IsOnlyWhitespaceOnLastLine(mOutputBuffer))
					{
						InsertCRIntoBuffer(mOutputBuffer);
					}
				}
				if (ASFormatter.IsLineEmpty(mOutputBuffer))
				{
					int indentAmount = GetCurrentIndent();
					if (GetIndentType(0) != EXPRESSION_INDENT && (tok.Type == AS3_exParser.LCURLY || tok.Type == AS3_exParser.RCURLY))
					{
						if (!IsHardIndent()) indentAmount -= GetBlockIndent();
					}
					String indentString = GenerateIndent(indentAmount);
					mOutputBuffer.Append(indentString);
				}
                if (!IsE4XTextContent() && mOutputBuffer.Length > 0 && IsIdentifierPart(mOutputBuffer[mOutputBuffer.Length - 1]))
                {
                    // If the start of the token is a java identifier char (any letter or number or '_')
                    // or if the token doesn't start with whitespace, but isn't an operator either ('+=' is an operator, so we wouldn't add space, but we would add space for a string literal)
                    if ((tok.Text.Length > 0 && IsIdentifierPart(tok.Text[0])) || IsSymbolTokenThatShouldHaveSpaceBeforeIt(tok.Type))
                    {
                        mOutputBuffer.Append(' ');
                    }
                }
				// Remove extra spaces before token for paren/brace/bracket case (if we're in a format mode)
				if (GetFormatMode() != FORMAT_INDENT)
				{
					if (tok.Text.Length == 1)
					{
						char c = tok.Text[0];
						if (IsGroupingChar(c))
						{
							int i = mOutputBuffer.Length - 1;
							int spaceCount = 0;
							while (i >= 0)
							{
                                if (mOutputBuffer[i] == ' ')
								{
									spaceCount++;
									i--;
								}
								else break;
							}
							if (i >= 0 && spaceCount > 0) // Only care if there are some spaces
							{
                                char prevChar = mOutputBuffer[i];
								int nestingType = DetermineMatchingGroupingCharType(c, prevChar);
								if (nestingType != Nesting_Unrelated)
								{
									int amtToKeep = 0;
									// TODO: Not sure how this will work with advanced spacing settings
                                    if (!IsCollapseSpaceForAdjacentParens() && nestingType != Nesting_Opposite)
                                    {
                                        if (prevChar == '(' || c == ')') amtToKeep = GetAdvancedSpacesInsideParens();
                                        else if (prevChar == '{' || c == '}') amtToKeep = GetAdvancedSpacesInsideObjectBraces();
                                        else if (prevChar == '[' || c == ']')
                                        {
                                            amtToKeep = GetAdvancedSpacesInsideArrayDeclBrackets();
                                        }
                                    }
                                    else if (prevChar == ')' && nestingType == Nesting_Deeper) amtToKeep = 1;

									if (amtToKeep < spaceCount)
									{
										mOutputBuffer.Remove(mOutputBuffer.Length - (spaceCount - amtToKeep), spaceCount - amtToKeep);
									}
								}
							}
						}
					}
				}
			}
			// If we have a lazy indent, push it after we've processed the token
			if (mLazyIndentType > 0)
			{
				if (mLazyIndentType == EXPRESSION_INDENT_NEXTITEM)
				{
					PushExpressionIndent(DetermineLastLineLength(mOutputBuffer, GetTabSize()));
				}
				else PushIndent(mLazyIndentType);
				mLazyIndentType = 0;
			}
			if (mLazyFormatType > 0)
			{
				PushFormatMode(mLazyFormatType);
				mLazyFormatType = 0;
			}
			// Special case for handling the need to lose 1 indent to support 'else-if' on the same line.  The grammar has code to
			// pop an indent and re-add later, but it can't know whether it needs to until we get to this point and determine whether
			// the 'if' is actually on the same line with the else or not (based on else-if setting, blank lines settings, indent vs. format, etc.)
			if (mNextTokenInElseIfState)
			{
				mNextTokenInElseIfState = false;
				String newText = mOutputBuffer.ToString().Substring(currentBufferOffset);
				if (!newText.Contains("\n"))
				{
					mElseIfNeedToLoseIndent=true;
				}
			}
			mOutputBuffer.Append(tok.Text);
		}
		
		public bool IsIdentifierPart(char ch)
		{
			return Char.IsLetterOrDigit(ch) || ch == '_' || ch == '$';
		}
		
		private bool IsFirstTokenBeforeFormatIndent()
		{
			return mFirstTokenBeforeFormatIndent;
		}
		
		public void SetFirstTokenBeforeFormatIndent()
		{
            if (mFormatTypeStack.Count == 1)
            {
                mFirstTokenBeforeFormatIndent = true;
            }
		}

		private const int Nesting_Opposite = 1;
		private const int Nesting_Unrelated = 2;
		private const int Nesting_Deeper = 3;

		private int DetermineMatchingGroupingCharType(char currentChar, char prevChar)
		{
			switch (currentChar)
			{
				case '(':
				case '{':
				case '[':
					if (prevChar == '(' || prevChar == '{' || prevChar == '[') return Nesting_Deeper;
					break;

				case ')':
					if (prevChar == '(') return Nesting_Opposite;
					break;

				case '}':
					if (prevChar == '{') return Nesting_Opposite;
					break;

				case ']':
					if (prevChar == '[') return Nesting_Opposite;
					break;
			}
            if ((currentChar == '(' || currentChar == '{' || currentChar == '[') && (prevChar == ')' || prevChar == '}' || prevChar == ']'))
            {
                return Nesting_Deeper;
            }
			return Nesting_Unrelated;
		}

		private bool IsGroupingChar(char c)
		{
			if (c == ')' || c == '(' || c == '['  || c == ']' || c == '{' || c == '}') return true;
			return false;
		}

		private bool LastCharIsNonWhitespace(StringBuilder buffer)
		{
            if (buffer.Length > 0 && !Char.IsWhiteSpace(buffer[buffer.Length - 1]))
            {
                return true;
            }
			return false;
		}
		

		public bool IsSymbolTokenThatShouldHaveSpaceBeforeIt(int tokenType)
		{
			switch (tokenType)
			{
				case AS3_exParser.COMMENT_MULTILINE:
				case AS3_exParser.COMMENT_SINGLELINE:
				case AS3_exParser.DOLLAR:
				case AS3_exParser.DOUBLE_QUOTE_LITERAL:
				case AS3_exParser.REGULAR_EXPR_LITERAL:
				case AS3_exParser.SINGLE_QUOTE_LITERAL:
				case AS3_exParser.UNICODE_ESCAPE:
					// Xml elements?
					return true;
			}
			return false;
		}

		public const int INITIAL_INDENT = 0;
		public const int BRACE_INDENT = 34;
		public const int STATEMENT_INDENT = 35;
		public const int EXPRESSION_INDENT = 36;
		public const int EXPRESSION_INDENT_NEXTITEM = 37;

		public class IndentType
		{
			public bool mLabeledIndent;
			public int mType;
			public int mAmount;

			public IndentType(int type, int amount)
			{
				mType = type;
				mAmount = amount;
				mLabeledIndent = false;
			}

			public bool IsLabeledIndent() 
            {
				return mLabeledIndent;
			}

			public void SetLabeledIndent(bool labeledIndent) 
            {
				mLabeledIndent = labeledIndent;
			}
			
			override public String ToString()
			{
				StringBuilder buffer = new StringBuilder();
				buffer.Append("Size=" + mAmount + "->");
				switch (mType)
				{
					case INITIAL_INDENT: 
                        buffer.Append("Initial indent");
					    break;

					case BRACE_INDENT: 
                        buffer.Append("Brace indent");
					    break;

					case STATEMENT_INDENT: 
                        buffer.Append("Statement indent");
					    break;

					case EXPRESSION_INDENT: 
                        buffer.Append("Expression indent");
					    break;

					default:
						buffer.Append("Unknown indent type");
						break;
				}
				return buffer.ToString();
			}
		}
		
		void PushExpressionIndent(int amount)
		{
			mIndentStack.Add(new IndentType(EXPRESSION_INDENT, amount));
		}
		
		public void PushIndent(int type)
		{
			int currentIndent = GetCurrentIndent();
			// If this is a brace indent and the current is a statement indent, then we don't need to increase the indent.
			if (type == BRACE_INDENT)
			{
				if (mIndentStack.Count > 0)
				{
					int indentType = GetIndentType(0);
					if (indentType == STATEMENT_INDENT)
					{
						mIndentStack.Add(new IndentType(type, currentIndent));
						return;
					}
				}
			}
			else if (type == EXPRESSION_INDENT)
			{
				if (mIndentStack.Count > 0)
				{
					int indentType = GetIndentType(0);
					// If previous type was expression indent, don't change the indent
					if (indentType == EXPRESSION_INDENT)
					{
						mIndentStack.Add(new IndentType(type, currentIndent));
						return;
					}
				}
			}
			else if (type == STATEMENT_INDENT)
			{
				// Walk backwards to find previous brace or statement indent and use that as my base indent
				for (int i = mIndentStack.Count - 1; i >= 0; i--)
				{
					IndentType indentType = mIndentStack.ToArray()[i];
					if (indentType.mType == BRACE_INDENT || indentType.mType == STATEMENT_INDENT || indentType.mType == INITIAL_INDENT)
					{
						currentIndent = indentType.mAmount;
						break;
					}
				}
			}
			mIndentStack.Add(new IndentType(type, currentIndent + GetBlockIndent()));
		}
		
		public int GetIndentType(int back)
		{
			if (mIndentStack.Count > back)
			{
				return mIndentStack.ToArray()[mIndentStack.Count - (back + 1)].mType;
			}
			return INITIAL_INDENT; // Have to return something
		}
		
		private int GetCurrentIndent()
		{
			if (mIndentStack.Count > 0)
			{
				return mIndentStack.ToArray()[mIndentStack.Count - 1].mAmount;
			}
			return 0;
		}
		
		public void MakeLabeledIndent()
		{
			if (mIndentStack.Count>0)
			{
				IndentType type = mIndentStack.ToArray()[mIndentStack.Count - 1];
				type.SetLabeledIndent(true);
				if (mIndentStack.Count > 1)
				{
					type.mAmount = mIndentStack.ToArray()[mIndentStack.Count - 2].mAmount;
				}
			}
		}
		
		public bool IsLabeledIndent()
		{
			if (mIndentStack.Count > 0)
			{
				IndentType type = mIndentStack.ToArray()[mIndentStack.Count - 1];
				return type.IsLabeledIndent();
			}
			return false;
		}
		
		public void PopIndent()
		{
			if (mIndentStack.Count > 0)
			{
				mIndentStack.Remove(mIndentStack.ToArray()[mIndentStack.Count - 1]);
				if (GetCurrentIndent() == -1) PopIndent();
			}
		}
		
		public bool IsHardIndent()
		{
            if (mIndentStack.Count >= 1)
            {
                IndentType indent = mIndentStack.ToArray()[mIndentStack.Count - 1];
                if (indent.mType == BRACE_INDENT || indent.mType == INITIAL_INDENT) return true;
            }
			return false;
		}
		
		public int GetBlankLinesBeforeClass() 
        {
			return mBlankLinesBeforeClass;
		}

		public void SetBlankLinesBeforeClass(int blankLinesBeforeClass) 
        {
			mBlankLinesBeforeClass = blankLinesBeforeClass;
		}

		public int GetBlankLinesBeforeControlStatement() 
        {
			return mBlankLinesBeforeControlStatement;
		}

		public void SetBlankLinesBeforeControlStatement(int blankLinesBeforeControlStatement) 
        {
			mBlankLinesBeforeControlStatement = blankLinesBeforeControlStatement;
		}

		public bool IsCRBeforeElse() 
        {
			if (IsUseBraceStyleSetting())
			{
				if (GetBraceStyleSetting() == BraceStyle_Sun) return false;
				else return true;
			}
			return mCRBeforeElse;
		}

		public void SetCRBeforeElse(bool beforeElse) 
        {
			mCRBeforeElse = beforeElse;
		}

		public bool IsCRBeforeCatch() 
        {
			if (IsUseBraceStyleSetting())
			{
				if (GetBraceStyleSetting() == BraceStyle_Sun) return false;
				else return true;
			}
			return mCRBeforeCatch;
		}

		public void SetCRBeforeCatch(bool beforeCatch) 
        {
			mCRBeforeCatch = beforeCatch;
		}

		public bool IsKeepElseIfOnSameLine() 
        {
			if (IsUseBraceStyleSetting()) return true;
			return mKeepElseIfOnSameLine;
		}

		public void SetKeepElseIfOnSameLine(bool keepElseIfOnSameLine) 
        {
			mKeepElseIfOnSameLine = keepElseIfOnSameLine;
		}

		public int GetMaxLineLength() 
        {
			return mMaxLineLength;
		}

		public void SetMaxLineLength(int maxLineLength) 
        {
			mMaxLineLength = maxLineLength;
		}

		public CommonToken GetLastToken() 
        {
			return mLastToken;
		}

		public int GetSpacesAroundAssignment() 
        {
			return mSpacesAroundAssignment;
		}

		public void SetSpacesAroundAssignment(int spacesAroundAssignment) 
        {
			mSpacesAroundAssignment = spacesAroundAssignment;
		}

		public int GetSpacesBeforeComma() 
        {
			return mSpacesBeforeComma;
		}

		public void SetSpacesBeforeComma(int spacesBeforeComma) 
        {
			mSpacesBeforeComma = spacesBeforeComma;
		}

		public int GetSpacesAfterComma() 
        {
			return mSpacesAfterComma;
		}

		public void SetSpacesAfterComma(int spacesAfterComma) 
        {
			mSpacesAfterComma = spacesAfterComma;
		}

		public int GetTabSize() 
        {
			return mTabSize;
		}

		public void SetTabSize(int tabSize) 
        {
			mTabSize = tabSize;
		}

		public bool IsUseTabs() 
        {
			return mUseTabs;
		}

		public void SetUseTabs(bool useTabs) 
        {
			mUseTabs = useTabs;
		}

		public bool IsKeepBlankLines() 
        {
			return mKeepBlankLines;
		}

		public void SetKeepBlankLines(bool keepBlankLines) 
        {
			mKeepBlankLines = keepBlankLines;
		}

		public int GetBlankLinesBeforeFunction() 
        {
			return mBlankLinesBeforeFunction;
		}

		public void SetBlankLinesBeforeFunction(int blankLinesBeforeFunction) 
        {
			mBlankLinesBeforeFunction = blankLinesBeforeFunction;
		}

		public bool IsDoFormat() 
        {
			return mDoFormat;
		}

		public void SetBlockIndent(int blockIndent) 
        {
			mBlockIndent = blockIndent;
		}

        public bool IsIndentMultilineComments()
        {
            return mIndentMultilineComments;
        }

        public void SetIndentMultilineComments(bool indentMultilineComments)
        {
            mIndentMultilineComments = indentMultilineComments;
        }

		public bool IsKeepSingleLineCommentsAtColumn1() 
        {
			return mKeepSingleLineCommentsAtColumn1;
		}

		public void SetKeepSingleLineCommentsAtColumn1(bool keepSingleLineCommentsAtColumn1) 
        {
			mKeepSingleLineCommentsAtColumn1 = keepSingleLineCommentsAtColumn1;
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

		public WrapOptions GetArrayInitWrapOptions() 
        {
			return mArrayInitWrapOptions;
		}

		public void SetArrayInitWrapOptions(WrapOptions arrayInitWrapOptions) 
        {
			mArrayInitWrapOptions = arrayInitWrapOptions;
		}

		public WrapOptions GetMethodCallWrapOptions() 
        {
			return mMethodCallWrapOptions;
		}

		public void SetMethodCallWrapOptions(WrapOptions methodCallWrapOptions) 
        {
			mMethodCallWrapOptions = methodCallWrapOptions;
		}

		public WrapOptions GetMethodDeclWrapOptions() 
        {
			return mMethodDeclWrapOptions;
		}

		public void SetMethodDeclWrapOptions(WrapOptions methodDeclWrapOptions) 
        {
			mMethodDeclWrapOptions = methodDeclWrapOptions;
		}

		public WrapOptions GetExpressionWrapOptions() 
        {
			return mExpressionWrapOptions;
		}

		public void SetExpressionWrapOptions(WrapOptions expressionWrapOptions) 
        {
			mExpressionWrapOptions = expressionWrapOptions;
		}

		public int GetIndentStyle()
        {
			return mIndentStyle;
		}
		
		public void SetIndentStyle(int indentStyle) 
        {
			mIndentStyle = indentStyle;
		}

		public int GetExpressionSpacesAroundSymbolicOperators() 
        {
			return mExpressionSpacesAroundSymbolicOperators;
		}

		public void SetExpressionSpacesAroundSymbolicOperators(int expressionSpacesAroundSymbolicOperators) 
        {
			mExpressionSpacesAroundSymbolicOperators = expressionSpacesAroundSymbolicOperators;
		}
		
		public void PushFormatMode(int formatMode)
		{
			mFormatTypeStack.Add(formatMode);
		}
		
		public void PopFormatMode()
		{
			if (mFormatTypeStack.Count > 1)
			{
				mFormatTypeStack.Remove(mFormatTypeStack.Count - 1);
			}
		}
		
		public int GetFormatMode()
		{
			return mFormatTypeStack.ToArray()[mFormatTypeStack.Count - 1];
		}

		public WrapOptions GetXMLWrapOptions() 
        {
			return mXMLWrapOptions;
		}

		public void SetXMLWrapOptions(WrapOptions wrapOptions) 
        {
			mXMLWrapOptions = wrapOptions;
		}
		
		public void PushLazyIndent(int indentType)
		{
			mLazyIndentType = indentType;
		}
		
		public void PushLazyFormat(int formatType)
		{
			mLazyFormatType = formatType;
		}

		public int GetSpacesAroundColons() 
        {
			return mSpacesAroundColons;
		}

		public void SetSpacesAroundColons(int spacesAroundColons) 
        {
			mSpacesAroundColons = spacesAroundColons;
		}
		
		public bool IsAtBlockStart()
		{
			if (mLastToken != null && mLastToken.Text.Equals("{")) return true;
			return false;
		}
		
		public bool IsCanAddCRsAtBlockStart()
		{
			return mCanAddCRsAtBlockStart;
		}
		
		public void SetCanAddCRsAtBlockStart(bool mode)
		{
			mCanAddCRsAtBlockStart = mode;
		}

		public int GetSpacesBetweenControlKeywordsAndParens() 
        {
			return mSpacesBetweenControlKeywordsAndParens;
		}

		public void SetSpacesBetweenControlKeywordsAndParens(int spacesBetweenControlKeywordsAndParens) 
        {
			mSpacesBetweenControlKeywordsAndParens = spacesBetweenControlKeywordsAndParens;
		}

		public bool IsCollapseSpaceForAdjacentParens() 
        {
			return mCollapseSpaceForAdjacentParens;
		}

		public void SetCollapseSpaceForAdjacentParens(bool collapseSpaceForAdjacentParens) 
        {
			mCollapseSpaceForAdjacentParens = collapseSpaceForAdjacentParens;
		}

		public bool IsDirectiveForNextElement(String directiveName)
		{
			//FIX? return true; //looks like they should always be associated with the next code item
		    if (directiveName == "Bindable" || directiveName == "Inspectable" || directiveName == "ArrayElementType") return true;
		    return false;
		}

		public bool IsNewlineAfterBindable() 
        {
			return mIsNewlineAfterBindable;
		}

		public void SetNewlineAfterBindable(bool newlineAfterBindable) 
        {
			this.mIsNewlineAfterBindable = newlineAfterBindable;
		}
		
		public void MarkBindablePos(bool conditionalCompilerTag)
		{
			if (mBindablePos < 0)
			{
				mMultipleBindableItems = false;
				mBindablePos = mOutputBuffer.Length;
			}
			else
			{
				// We've still got a bindable pos set, so we have more than one item
				mMultipleBindableItems=true;
			}
			mLastBindableWasConditionalTag = conditionalCompilerTag;
			mProcessingBindableTag = true;
		}
		
		public void SetBindableMode()
		{
			mBindableMode = true;
			mProcessingBindableTag = false;
		}

		public int GetSpacesAfterLabel() 
        {
			return mSpacesAfterLabel;
		}

		public void SetSpacesAfterLabel(int spacesAfterLabel) 
        {
			mSpacesAfterLabel = spacesAfterLabel;
		}

		public bool IsTrimTrailingWS() 
        {
			return mTrimTrailingWS;
		}

		public void SetTrimTrailingWS(bool trimTrailingWS) 
        {
			mTrimTrailingWS = trimTrailingWS;
		}

		public bool IsEmptyStatementsOnNewLine() 
        {
			return mEmptyStatementsOnNewLine;
		}

		public void SetEmptyStatementsOnNewLine(bool emptyStatementsOnNewLine) 
        {
			mEmptyStatementsOnNewLine = emptyStatementsOnNewLine;
		}

		public bool IsProcessingBindableTag() 
        {
			return mProcessingBindableTag;
		}

		public void SetProcessingBindableTag(bool processingBindableTag) 
        {
			mProcessingBindableTag = processingBindableTag;
		}

		public int GetSpacesInsideParensEtc() 
        {
			return mSpacesInsideParensEtc;
		}

		public void SetSpacesInsideParensEtc(int spacesInsideParensEtc) 
        {
			mSpacesInsideParensEtc = spacesInsideParensEtc;
		}

		public int GetAdvancedSpacesInsideParens() 
        {
			if (mUseSpacesInsideParensEtc) return GetSpacesInsideParensEtc();
			return mAdvancedSpacesInsideParens;
		}

		public void SetAdvancedSpacesInsideParens(int advancedSpacesInsideParens) 
        {
			mAdvancedSpacesInsideParens = advancedSpacesInsideParens;
		}

		public int GetAdvancedSpacesInsideArrayDeclBrackets() 
        {
			if (mUseSpacesInsideParensEtc) return GetSpacesInsideParensEtc();
			return mAdvancedSpacesInsideArrayDeclBrackets;
		}

		public void SetAdvancedSpacesInsideArrayDeclBrackets(int advancedSpacesInsideArrayDeclBrackets) 
        {
			mAdvancedSpacesInsideArrayDeclBrackets = advancedSpacesInsideArrayDeclBrackets;
		}

		public int GetAdvancedSpacesInsideArrayReferenceBrackets() 
        {
			if (mUseSpacesInsideParensEtc) return GetSpacesInsideParensEtc();
			return mAdvancedSpacesInsideArrayReferenceBrackets;
		}

		public void SetAdvancedSpacesInsideArrayReferenceBrackets(int advancedSpacesInsideArrayReferenceBrackets) 
        {
			mAdvancedSpacesInsideArrayReferenceBrackets = advancedSpacesInsideArrayReferenceBrackets;
		}

		public int GetAdvancedSpacesInsideObjectBraces() 
        {
			if (mUseSpacesInsideParensEtc) return GetSpacesInsideParensEtc();
			return mAdvancedSpacesInsideObjectBraces;
		}

		public void SetAdvancedSpacesInsideObjectBraces(int advancedSpacesInsideObjectBraces) 
        {
			mAdvancedSpacesInsideObjectBraces = advancedSpacesInsideObjectBraces;
		}

		public bool IsUseSpacesInsideParensEtc() 
        {
			return mUseSpacesInsideParensEtc;
		}

		public void SetUseSpacesInsideParensEtc(bool useSpacesInsideParensEtc) 
        {
			mUseSpacesInsideParensEtc = useSpacesInsideParensEtc;
		}

		public int GetAdvancedSpacesAroundAssignmentInOptionalParameters() 
        {
			if (!IsUseAdvancedSpacesAroundAssignmentInOptionalParameters()) return GetSpacesAroundAssignment();
			return mAdvancedSpacesAroundAssignmentInOptionalParameters;
		}

		public void SetAdvancedSpacesAroundAssignmentInOptionalParameters(int advancedSpacesAroundAssignmentInOptionalParameters) 
        {
			mAdvancedSpacesAroundAssignmentInOptionalParameters = advancedSpacesAroundAssignmentInOptionalParameters;
		}

		public bool IsUseAdvancedSpacesAroundAssignmentInOptionalParameters() 
        {
			return mUseAdvancedSpacesAroundAssignmentInOptionalParameters;
		}

		public void SetUseAdvancedSpacesAroundAssignmentInOptionalParameters(bool useAdvancedSpacesAroundAssignmentInOptionalParameters) 
        {
			mUseAdvancedSpacesAroundAssignmentInOptionalParameters = useAdvancedSpacesAroundAssignmentInOptionalParameters;
		}

		public bool IsInParameterDecl() 
        {
			return mInParameterDecl;
		}

		public void SetInParameterDecl(bool inParameterDecl) 
        {
			mInParameterDecl = inParameterDecl;
		}

		public bool IsUseBraceStyleSetting() 
        {
			return mUseBraceStyleSetting;
		}

		public void SetUseBraceStyleSetting(bool useBraceStyleSetting) 
        {
			mUseBraceStyleSetting = useBraceStyleSetting;
		}

		public int GetBraceStyleSetting() 
        {
            if (mBraceStyleSetting == (int)BraceStyle.Inherit)
            {
                PluginCore.CodingStyle cs = PluginCore.PluginBase.Settings.CodingStyle;
                if (cs == PluginCore.CodingStyle.BracesAfterLine) return (int)BraceStyle.AfterLine;
                else return (int)BraceStyle.OnLine;
            }
            else return mBraceStyleSetting;
		}

		public void SetBraceStyleSetting(int braceStyleSetting) 
        {
			mBraceStyleSetting = braceStyleSetting;
		}

		public void SetAlwaysGenerateIndent(bool selection) 
        {
			mAlwaysGenerateIndent=selection;
		}

		public bool IsAlwaysGenerateIndent() 
        {
			return mAlwaysGenerateIndent;
		}

		public int GetBlankLinesToKeep() 
        {
			return mBlankLinesToKeep;
		}

		public void SetBlankLinesToKeep(int blankLinesToKeep) 
        {
			mBlankLinesToKeep = blankLinesToKeep;
		}

		public int GetAdvancedSpacesBeforeColons() 
        {
			if (IsUseGlobalSpacesAroundColons()) return GetSpacesAroundColons();
			return mAdvancedSpacesBeforeColons;
		}

		public void SetAdvancedSpacesBeforeColons(int advancedSpacesBeforeColons) 
        {
			mAdvancedSpacesBeforeColons = advancedSpacesBeforeColons;
		}

		public int GetAdvancedSpacesAfterColons() 
        {
			if (IsUseGlobalSpacesAroundColons()) return GetSpacesAroundColons();
			return mAdvancedSpacesAfterColons;
		}

		public void SetAdvancedSpacesAfterColons(int advancedSpacesAfterColons) 
        {
			mAdvancedSpacesAfterColons = advancedSpacesAfterColons;
		}

		public bool IsUseGlobalSpacesAroundColons() 
        {
			return mUseGlobalSpacesAroundColons;
		}

		public void SetUseGlobalSpacesAroundColons(bool useGlobalSpacesAroundColons) 
        {
			mUseGlobalSpacesAroundColons = useGlobalSpacesAroundColons;
		}

		public void SetBlankLinesBeforeProperties(int lines)
		{
			mBlankLinesBeforeProperties = lines;
		}

		public int GetBlankLinesBeforeProperties() 
        {
			return mBlankLinesBeforeProperties;
		}

		public bool IsIndentAtPackageLevel() 
        {
			return mIndentAtPackageLevel;
		}

		public void SetIndentAtPackageLevel(bool indentAtPackageLevel) 
        {
			mIndentAtPackageLevel = indentAtPackageLevel;
		}

		public bool IsKeepingExcessDeclWhitespace() 
        {
			return mIsKeepingExcessDeclWhitespace;
		}

		public void SetKeepingExcessDeclWhitespace(bool isKeepingExcessDeclWhitespace) 
        {
			mIsKeepingExcessDeclWhitespace = isKeepingExcessDeclWhitespace;
		}

		public int GetColumnForIndex(CommonToken token, int indexWithinToken)
		{
			// Walk tokens back to the start of the line so that I can find other tab characters that would modify the index
			int col = 0;
			int startTokenIndex = token.TokenIndex - 1;
			while (startTokenIndex > 0)
			{
				CommonToken t = (CommonToken)mRawTokens.Get(startTokenIndex);
				if (t.Channel == AS3_exParser.CHANNEL_EOL || t.Channel == AS3_exParser.CHANNEL_SLCOMMENT)
				{
					startTokenIndex++;
					break;
				}
				else if (t.Channel == AS3_exParser.CHANNEL_MLCOMMENT)
				{
					// Handle line break in the middle of comment
					int lastCR = t.Text.LastIndexOf('\n');
					if (lastCR >= 0)
					{
						String text = t.Text.Substring(lastCR + 1);
						col = GetColumnLength(col, text, 0, text.Length);
						startTokenIndex++;
						break;
					}
				}
				startTokenIndex--;
			}
			// Now, walk forward counting columns
			for (int i = startTokenIndex; i < token.TokenIndex; i++)
			{
				CommonToken t = (CommonToken)mRawTokens.Get(i);
				if (t.Channel == AS3_exParser.CHANNEL_WHITESPACE)
				{
					col = GetColumnLength(col, t.Text, 0, t.Text.Length);
				}
				else col += t.Text.Length;
			}
			// Handle the partial token we're currently on
			CommonToken t2 =(CommonToken)mRawTokens.Get(token.TokenIndex);
			String text2 = t2.Text.Substring(0, indexWithinToken);
			col = GetColumnLength(col, text2, 0, indexWithinToken);
			return col;
		}
		
		private int GetColumnLength(int col, String text, int start, int end)
		{
			for (int k = start; k < end && k < text.Length; k++)
			{
				if (text[k] == '\t')
				{
					if (col % GetTabSize() == 0) col += GetTabSize();
					else col += GetTabSize() - col % GetTabSize();
				}
				else col++;
			}
			return col;
		}
		
		public void SetElseIfState()
		{
			mNextTokenInElseIfState = true;
		}
		
		public bool GetElseIfNeedToLoseIndent()
		{
			bool result = mElseIfNeedToLoseIndent;
			mElseIfNeedToLoseIndent = false;
			return result;
		}
		
		public void PushFormatterOff()
		{
			mExcludeStackCount++;
		}
		
		public void PopFormatterOff()
		{
			mExcludeStackCount--;
			if (mExcludeStackCount < 0) mExcludeStackCount = 0;
		}
		
		public bool IsFormatterOff()
		{
			return (mExcludeStackCount > 0);
		}
		
		public void SetE4XTextContent(bool inE4XText)
		{
			mInE4XText = inE4XText;
		}
		
		public bool IsE4XTextContent()
		{
			return mInE4XText;
		}

	}

}