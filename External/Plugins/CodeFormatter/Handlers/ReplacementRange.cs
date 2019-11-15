// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;

namespace CodeFormatter.Handlers
{
    public class ReplacementRange
    {
        public Point mRangeInFormattedDoc;
        private readonly Point mRangeInOriginalDoc;
        private string mAddedText;
        private string mDeletedText;

        public ReplacementRange(Point rangeInFormattedDoc, Point rangeInOrigDoc)
        {
            mRangeInOriginalDoc = rangeInOrigDoc;
            mRangeInFormattedDoc = rangeInFormattedDoc;
            mAddedText = "";
            mDeletedText = "";
        }

        public void setChangedText(string added, string removed)
        {
            if (added != null) mAddedText = added;
            if (removed != null) mDeletedText = removed;
        }
        public Point getRangeInFormattedDoc()
        {
            return mRangeInFormattedDoc;
        }
        public Point getRangeInOriginalDoc()
        {
            return mRangeInOriginalDoc;
        }
        public string getAddedText()
        {
            return mAddedText;
        }
        public string getDeletedText()
        {
            return mDeletedText;
        }

    }

}