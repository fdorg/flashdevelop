// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree
{
	public class TreeViewAdvEventArgs : EventArgs
	{
		private TreeNodeAdv _node;

		public TreeNodeAdv Node
		{
			get { return _node; }
		}

		public TreeViewAdvEventArgs(TreeNodeAdv node)
		{
			_node = node;
		}
	}
}
