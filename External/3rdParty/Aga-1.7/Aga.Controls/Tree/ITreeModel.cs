// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Aga.Controls.Tree
{
	public interface ITreeModel
	{
		IEnumerable GetChildren(TreePath treePath);
		bool IsLeaf(TreePath treePath);

		event EventHandler<TreeModelEventArgs> NodesChanged; 
		event EventHandler<TreeModelEventArgs> NodesInserted;
		event EventHandler<TreeModelEventArgs> NodesRemoved; 
		event EventHandler<TreePathEventArgs> StructureChanged;
	}
}
