// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;

namespace Aga.Controls.Tree
{
	public class DropNodeValidatingEventArgs: EventArgs
	{
		Point _point;
		TreeNodeAdv _node;

		public DropNodeValidatingEventArgs(Point point, TreeNodeAdv node)
		{
			_point = point;
			_node = node;
		}

		public Point Point
		{
			get { return _point; }
		}

		public TreeNodeAdv Node
		{
			get { return _node; }
			set { _node = value; }
		}
	}
}
