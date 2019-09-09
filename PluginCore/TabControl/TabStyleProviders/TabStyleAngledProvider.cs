/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

using System.Drawing;

namespace System.Windows.Forms
{
	[System.ComponentModel.ToolboxItem(false)]
	public class TabStyleAngledProvider : TabStyleProvider
	{
		public TabStyleAngledProvider(CustomTabControl tabControl) : base(tabControl){
			this._ImageAlign = ContentAlignment.MiddleRight;
			this._Overlap = 7;
			this._Radius = 10;
			
			//	Must set after the _Radius as this is used in the calculations of the actual padding
			this.Padding = new Point(10, 3);

		}
		
		public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds){
			switch (this._TabControl.Alignment) {
				case TabAlignment.Top:
					path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X + this._Radius - 2, tabBounds.Y + 2);
					path.AddLine(tabBounds.X + this._Radius, tabBounds.Y, tabBounds.Right - this._Radius, tabBounds.Y);
					path.AddLine(tabBounds.Right - this._Radius + 2, tabBounds.Y + 2, tabBounds.Right, tabBounds.Bottom);
					break;
				case TabAlignment.Bottom:
					path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right - this._Radius + 2, tabBounds.Bottom - 2);
					path.AddLine(tabBounds.Right - this._Radius, tabBounds.Bottom, tabBounds.X + this._Radius, tabBounds.Bottom);
					path.AddLine(tabBounds.X + this._Radius - 2, tabBounds.Bottom - 2, tabBounds.X, tabBounds.Y);
					break;
				case TabAlignment.Left:
					path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X + 2, tabBounds.Bottom - this._Radius + 2);
					path.AddLine(tabBounds.X, tabBounds.Bottom - this._Radius, tabBounds.X, tabBounds.Y + this._Radius);
					path.AddLine(tabBounds.X + 2, tabBounds.Y + this._Radius - 2, tabBounds.Right, tabBounds.Y);
					break;
				case TabAlignment.Right:
					path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right - 2, tabBounds.Y + this._Radius - 2);
					path.AddLine(tabBounds.Right, tabBounds.Y + this._Radius, tabBounds.Right, tabBounds.Bottom - this._Radius);
					path.AddLine(tabBounds.Right - 2, tabBounds.Bottom - this._Radius + 2, tabBounds.X, tabBounds.Bottom);
					break;
			}
		}

	}
}
