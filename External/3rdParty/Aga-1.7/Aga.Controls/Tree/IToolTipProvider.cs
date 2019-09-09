// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using Aga.Controls.Tree.NodeControls;

namespace Aga.Controls.Tree
{
	public interface IToolTipProvider
	{
		string GetToolTip(TreeNodeAdv node, NodeControl nodeControl);
	}
}
