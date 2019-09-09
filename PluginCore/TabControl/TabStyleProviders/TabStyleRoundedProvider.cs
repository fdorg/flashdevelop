/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

using System.Drawing;

namespace System.Windows.Forms
{

	[System.ComponentModel.ToolboxItem(false)]
	public class TabStyleRoundedProvider : TabStyleProvider
	{
		public TabStyleRoundedProvider(CustomTabControl tabControl) : base(tabControl){
			this._Radius = 10;
			//	Must set after the _Radius as this is used in the calculations of the actual padding
			this.Padding = new Point(6, 3);
		}
		
		public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds){

			switch (this._TabControl.Alignment) {
				case TabAlignment.Top:
					path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y + this._Radius);
					path.AddArc(tabBounds.X, tabBounds.Y, this._Radius * 2, this._Radius * 2, 180, 90);
					path.AddLine(tabBounds.X + this._Radius, tabBounds.Y, tabBounds.Right - this._Radius, tabBounds.Y);
					path.AddArc(tabBounds.Right - this._Radius * 2, tabBounds.Y, this._Radius * 2, this._Radius * 2, 270, 90);
					path.AddLine(tabBounds.Right, tabBounds.Y + this._Radius, tabBounds.Right, tabBounds.Bottom);
					break;
				case TabAlignment.Bottom:
					path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom - this._Radius);
					path.AddArc(tabBounds.Right - this._Radius * 2, tabBounds.Bottom - this._Radius * 2, this._Radius * 2, this._Radius * 2, 0, 90);
					path.AddLine(tabBounds.Right - this._Radius, tabBounds.Bottom, tabBounds.X + this._Radius, tabBounds.Bottom);
					path.AddArc(tabBounds.X, tabBounds.Bottom - this._Radius * 2, this._Radius * 2, this._Radius * 2, 90, 90);
					path.AddLine(tabBounds.X, tabBounds.Bottom - this._Radius, tabBounds.X, tabBounds.Y);
					break;
				case TabAlignment.Left:
					path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X + this._Radius, tabBounds.Bottom);
					path.AddArc(tabBounds.X, tabBounds.Bottom - this._Radius * 2, this._Radius * 2, this._Radius * 2, 90, 90);
					path.AddLine(tabBounds.X, tabBounds.Bottom - this._Radius, tabBounds.X, tabBounds.Y + this._Radius);
					path.AddArc(tabBounds.X, tabBounds.Y, this._Radius * 2, this._Radius * 2, 180, 90);
					path.AddLine(tabBounds.X + this._Radius, tabBounds.Y, tabBounds.Right, tabBounds.Y);
					break;
				case TabAlignment.Right:
					path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right - this._Radius, tabBounds.Y);
					path.AddArc(tabBounds.Right - this._Radius * 2, tabBounds.Y, this._Radius * 2, this._Radius * 2, 270, 90);
					path.AddLine(tabBounds.Right, tabBounds.Y + this._Radius, tabBounds.Right, tabBounds.Bottom - this._Radius);
					path.AddArc(tabBounds.Right - this._Radius * 2, tabBounds.Bottom - this._Radius * 2, this._Radius * 2, this._Radius * 2, 0, 90);
					path.AddLine(tabBounds.Right - this._Radius, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
					break;
			}
		}
	}
}
