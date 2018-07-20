// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
