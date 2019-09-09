// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Aga.Controls.Properties;

namespace Aga.Controls.Tree.NodeControls
{
	public class NodeStateIcon: NodeIcon
	{
		private Image _leaf;
		private Image _opened;
		private Image _closed;

		public NodeStateIcon()
		{
			_leaf = MakeTransparent(Resources.Leaf);
			_opened = MakeTransparent(Resources.Folder);
			_closed = MakeTransparent(Resources.FolderClosed);
		}

		private static Image MakeTransparent(Bitmap bitmap)
		{
			bitmap.MakeTransparent(bitmap.GetPixel(0,0));
			return bitmap;
		}

		protected override Image GetIcon(TreeNodeAdv node)
		{
			Image icon = base.GetIcon(node);
			if (icon != null)
				return icon;
			else if (node.IsLeaf)
				return _leaf;
			else if (node.CanExpand && node.IsExpanded)
				return _opened;
			else
				return _closed;
		}
	}
}
