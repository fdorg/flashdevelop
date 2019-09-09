// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree
{
	public class TreeViewAdvCancelEventArgs : TreeViewAdvEventArgs
	{
		private bool _cancel;

		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}

		public TreeViewAdvCancelEventArgs(TreeNodeAdv node)
			: base(node)
		{
		}

	}
}
