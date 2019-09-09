// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeEventArgs : EventArgs
	{
		private TreeNodeAdv _node;
		public TreeNodeAdv Node
		{
			get { return _node; }
		}

		public NodeEventArgs(TreeNodeAdv node)
		{
			_node = node;
		}
	}
}
