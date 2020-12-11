// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree
{
	internal abstract class ColumnState : InputState
	{
		private TreeColumn _column;
		public TreeColumn Column 
		{
			get { return _column; } 
		}

		public ColumnState(TreeViewAdv tree, TreeColumn column)
			: base(tree)
		{
			_column = column;
		}
	}
}
