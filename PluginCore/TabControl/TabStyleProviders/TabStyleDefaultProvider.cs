/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace System.Windows.Forms
{

	[System.ComponentModel.ToolboxItem(false)]
	public class TabStyleDefaultProvider : TabStyleProvider
	{
		public TabStyleDefaultProvider(CustomTabControl tabControl) : base(tabControl){
			this._FocusTrack = true;
			this._Radius = 2;
		}
		
		public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds){
			switch (this._TabControl.Alignment) {
				case TabAlignment.Top:
					path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
					path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
					path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
					break;
				case TabAlignment.Bottom:
					path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
					path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
					path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
					break;
				case TabAlignment.Left:
					path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
					path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
					path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
					break;
				case TabAlignment.Right:
					path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
					path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
					path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
					break;
			}
		}
		
		public override Rectangle GetTabRect(int index){
			if (index < 0){
				return new Rectangle();
			}

			Rectangle tabBounds = base.GetTabRect(index);
			bool firstTabinRow = this._TabControl.IsFirstTabInRow(index);

			//	Make non-SelectedTabs smaller and selected tab bigger
			if (index != this._TabControl.SelectedIndex) {
				switch (this._TabControl.Alignment) {
					case TabAlignment.Top:
						tabBounds.Y += 1;
						tabBounds.Height -= 1;
						break;
					case TabAlignment.Bottom:
						tabBounds.Height -= 1;
						break;
					case TabAlignment.Left:
						tabBounds.X += 1;
						tabBounds.Width -= 1;
						break;
					case TabAlignment.Right:
						tabBounds.Width -= 1;
						break;
				}
			} else {
				switch (this._TabControl.Alignment) {
					case TabAlignment.Top:
						if (tabBounds.Y > 0){
							tabBounds.Y -= 1;
							tabBounds.Height += 1;
						}
						
						if (firstTabinRow){
							tabBounds.Width += 1;
						} else {
							tabBounds.X -= 1;
							tabBounds.Width += 2;
						}
						break;
					case TabAlignment.Bottom:
						if (tabBounds.Bottom < this._TabControl.Bottom){
							tabBounds.Height += 1;
						}
						if (firstTabinRow){
							tabBounds.Width += 1;
						} else {
							tabBounds.X -= 1;
							tabBounds.Width += 2;
						}
						break;
					case TabAlignment.Left:
						if (tabBounds.X > 0){
							tabBounds.X -= 1;
							tabBounds.Width += 1;
						}

						if (firstTabinRow){
							tabBounds.Height += 1;
						} else {
							tabBounds.Y -= 1;
							tabBounds.Height += 2;
						}
						break;
					case TabAlignment.Right:
						if (tabBounds.Right < this._TabControl.Right){
							tabBounds.Width += 1;
						}
						if (firstTabinRow){
							tabBounds.Height += 1;
						} else {
							tabBounds.Y -= 1;
							tabBounds.Height += 2;
						}
						break;
				}
			}

			//	Adjust first tab in the row to align with tabpage
			this.EnsureFirstTabIsInView(ref tabBounds, index);

			return tabBounds;
		}
	}
}
