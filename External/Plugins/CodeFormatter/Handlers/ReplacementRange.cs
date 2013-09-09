using System;
using System.Drawing;

namespace CodeFormatter.Handlers
{
	public class ReplacementRange
	{
		Point mRangeInFormattedDoc;
		Point mRangeInOriginalDoc;

		public ReplacementRange(Point rangeInFormattedDoc, Point rangeInOrigDoc)
		{
			mRangeInOriginalDoc = rangeInOrigDoc;
			mRangeInFormattedDoc = rangeInFormattedDoc;
		}

		public Point GetRangeInFormattedDoc() 
        {
			return mRangeInFormattedDoc;
		}

		public Point GetRangeInOriginalDoc() 
        {
			return mRangeInOriginalDoc;
		}

	}

}
