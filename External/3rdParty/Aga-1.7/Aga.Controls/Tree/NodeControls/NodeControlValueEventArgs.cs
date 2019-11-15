// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeControlValueEventArgs : NodeEventArgs
	{
		private object _value;
		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		public NodeControlValueEventArgs(TreeNodeAdv node)
			:base(node)
		{
		}
	}
}
