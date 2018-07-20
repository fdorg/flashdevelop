// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;

namespace CodeFormatter.Handlers
{
    public class ReplacementRange
    {
        public Point mRangeInFormattedDoc;
        private Point mRangeInOriginalDoc;
        private String mAddedText;
        private String mDeletedText;

        public ReplacementRange(Point rangeInFormattedDoc, Point rangeInOrigDoc)
        {
            mRangeInOriginalDoc = rangeInOrigDoc;
            mRangeInFormattedDoc = rangeInFormattedDoc;
            mAddedText = "";
            mDeletedText = "";
        }

        public void setChangedText(String added, String removed)
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
        public String getAddedText()
        {
            return mAddedText;
        }
        public String getDeletedText()
        {
            return mDeletedText;
        }

    }

}