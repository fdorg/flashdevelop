// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree.NodeControls
{
	public class LabelEventArgs : EventArgs
	{
		private object _subject;
		public object Subject
		{
			get { return _subject; }
		}

		private string _oldLabel;
		public string OldLabel
		{
			get { return _oldLabel; }
		}

		private string _newLabel;
		public string NewLabel
		{
			get { return _newLabel; }
		}

		public LabelEventArgs(object subject, string oldLabel, string newLabel)
		{
			_subject = subject;
			_oldLabel = oldLabel;
			_newLabel = newLabel;
		}
	}
}
