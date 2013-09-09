using System;
using System.Collections;

namespace DifferenceEngine
{
	public class DiffList_CharData : IDiffList
	{
		private char[] _charList;

		public DiffList_CharData(string charData)
		{
			_charList = charData.ToCharArray();
		}
		#region IDiffList Members

		public int Count()
		{
			return _charList.Length;
		}

		public IComparable GetByIndex(int index)
		{
			return _charList[index];
		}

		#endregion
	}
}