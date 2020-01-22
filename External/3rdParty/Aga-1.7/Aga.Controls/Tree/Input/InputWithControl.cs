// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace Aga.Controls.Tree
{
	internal class InputWithControl: NormalInputState
	{
		public InputWithControl(TreeViewAdv tree): base(tree)
		{
		}

		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
			if (Tree.SelectionMode == TreeSelectionMode.Single)
			{
				base.DoMouseOperation(args);
			}
			else if (CanSelect(args.Node))
			{
				args.Node.IsSelected = !args.Node.IsSelected;
				Tree.SelectionStart = args.Node;
			}
		}

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}
	}
}
