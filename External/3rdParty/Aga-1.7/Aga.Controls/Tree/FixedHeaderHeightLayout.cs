// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace Aga.Controls.Tree
{
	class FixedHeaderHeightLayout : IHeaderLayout
	{
		TreeViewAdv _treeView;
		int _headerHeight;

		public FixedHeaderHeightLayout(TreeViewAdv treeView, int headerHeight)
		{
			_treeView = treeView;
			PreferredHeaderHeight = headerHeight;
		}

		#region Implementation of IHeaderLayout

		public int PreferredHeaderHeight
		{
			get { return _headerHeight; }
			set { _headerHeight = value; }
		}

		public void ClearCache()
		{
		}

		#endregion
	}
}
