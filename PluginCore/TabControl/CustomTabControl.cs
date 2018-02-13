/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Permissions;

namespace System.Windows.Forms
{
	
	[ToolboxBitmapAttribute(typeof(TabControl))]
	public class CustomTabControl : TabControl
	{
		
		#region	Construction

		public CustomTabControl(){
			
			this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw , true);
			
			this._BackBuffer = new Bitmap(this.Width, this.Height);
			this._BackBufferGraphics = Graphics.FromImage(this._BackBuffer);
			this._TabBuffer = new Bitmap(this.Width, this.Height);
			this._TabBufferGraphics = Graphics.FromImage(this._TabBuffer);
			
			this.DisplayStyle = TabStyle.Default;
			
		}

		protected override void OnCreateControl(){
			base.OnCreateControl();
			this.OnFontChanged(EventArgs.Empty);
		} 

		
		protected override CreateParams CreateParams
        {
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams cp = base.CreateParams;
                if (this.RightToLeftLayout)
                    cp.ExStyle = cp.ExStyle | NativeMethods.WS_EX_LAYOUTRTL | NativeMethods.WS_EX_NOINHERITLAYOUT;
                return cp;
            }
        }
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing) {
				if (this._BackImage != null){
					this._BackImage.Dispose();
				}
				if (this._BackBufferGraphics != null){
					this._BackBufferGraphics.Dispose();
				}
				if (this._BackBuffer != null){
					this._BackBuffer.Dispose();
				}
				if (this._TabBufferGraphics != null){
					this._TabBufferGraphics.Dispose();
				}
				if (this._TabBuffer != null){
					this._TabBuffer.Dispose();
				}
				
				if (this._StyleProvider != null){
					this._StyleProvider.Dispose();
				}
			}
		}

		#endregion
		
		#region Private variables

		private Bitmap _BackImage;
		private Bitmap _BackBuffer;
		private Graphics _BackBufferGraphics;
		private Bitmap _TabBuffer;
		private Graphics _TabBufferGraphics;
		
		private int _oldValue;
		private Point _dragStartPosition = Point.Empty;

		private TabStyle _Style;
		private TabStyleProvider _StyleProvider;

		private List<TabPage> _TabPages;

		#endregion
		
		#region Public properties

		[Category("Appearance"),  DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TabStyleProvider DisplayStyleProvider {
			get {
				if (this._StyleProvider == null){
					this.DisplayStyle = TabStyle.Default;
				}
				
				return this._StyleProvider;
			}
			set {
				this._StyleProvider = value;
			}
		}

		[Category("Appearance"), DefaultValue(typeof(TabStyle), "Default"), RefreshProperties(RefreshProperties.All)]
		public TabStyle DisplayStyle {
			get { return this._Style; }
			set {
				if (this._Style != value){
					this._Style = value;
					this._StyleProvider = TabStyleProvider.CreateProvider(this);
					this.Invalidate();
				}
			}
		}

		[Category("Appearance"), RefreshProperties(RefreshProperties.All)]
		public new bool Multiline {
			get {
				return base.Multiline;
			}
			set {
				base.Multiline = value;
				this.Invalidate();
			}
		}
		
		
		//	Hide the Padding attribute so it can not be changed
		//	We are handling this on the Style Provider
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Padding {
			get { return this.DisplayStyleProvider.Padding; }
			set {
				this.DisplayStyleProvider.Padding = value;
			}
		}
		
		public override bool RightToLeftLayout {
			get { return base.RightToLeftLayout; }
			set { 
				base.RightToLeftLayout = value; 
				this.UpdateStyles();
			}
		}
		
						
		//	Hide the HotTrack attribute so it can not be changed
		//	We are handling this on the Style Provider
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool HotTrack {
			get { return this.DisplayStyleProvider.HotTrack; }
			set {
				this.DisplayStyleProvider.HotTrack = value;
			}
		}

		[Category("Appearance")]
		public new TabAlignment Alignment {
			get { return base.Alignment; }
			set {
				base.Alignment = value;
				switch (value) {
					case TabAlignment.Top:
					case TabAlignment.Bottom:
						this.Multiline = false;
						break;
					case TabAlignment.Left:
					case TabAlignment.Right:
						this.Multiline = true;
						break;
				}
				
			}
		}
		
		//	Hide the Appearance attribute so it can not be changed
		//	We don't want it as we are doing all the painting.
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
		public new TabAppearance Appearance{
			get{
				return base.Appearance;
			}
			set{
				//	Don't permit setting to other appearances as we are doing all the painting
				base.Appearance = TabAppearance.Normal;
			}
		}
		
		public override Rectangle DisplayRectangle {
			get {
				//	Special processing to hide tabs
				if (this._Style == TabStyle.None) {
					return new Rectangle(0, 0, Width, Height);
				} else {
					int tabStripHeight = 0;
					int itemHeight = 0;
					
					if (this.Alignment <= TabAlignment.Bottom) {
						itemHeight = this.ItemSize.Height;
					} else {
						itemHeight = this.ItemSize.Width;
					}
					
					tabStripHeight = 5 + (itemHeight * this.RowCount);
					
					Rectangle rect = new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
					switch (this.Alignment) {
						case TabAlignment.Top:
							rect = new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
							break;
						case TabAlignment.Bottom:
							rect = new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
							break;
						case TabAlignment.Left:
							rect = new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
							break;
						case TabAlignment.Right:
							rect = new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
							break;
					}
					return rect;
				}
			}
		}

		[Browsable(false)]
		public int ActiveIndex {
			get {
				NativeMethods.TCHITTESTINFO hitTestInfo = new NativeMethods.TCHITTESTINFO(this.PointToClient(Control.MousePosition));
				int index = NativeMethods.SendMessage(this.Handle, NativeMethods.TCM_HITTEST, IntPtr.Zero, NativeMethods.ToIntPtr(hitTestInfo)).ToInt32();
				if (index == -1){
					return -1;
				} else {
					if (this.TabPages[index].Enabled){
						return index;
					} else {
						return -1;
					}
				}
			}
		}
		
		[Browsable(false)]
		public TabPage ActiveTab{
			get{
				int activeIndex = this.ActiveIndex;
				if (activeIndex > -1){
					return this.TabPages[activeIndex];
				} else {
					return null;
				}
			}
		}
		
		#endregion

		#region	Extension methods

		public void HideTab(TabPage page){
			if (page != null && this.TabPages.Contains(page)){
				this.BackupTabPages();
				this.TabPages.Remove(page);
			}
		}
		
		public void HideTab(int index){
			if (this.IsValidTabIndex(index)){
				this.HideTab(this._TabPages[index]);
			}
		}

		public void HideTab(string key){
			if (this.TabPages.ContainsKey(key)){
				this.HideTab(this.TabPages[key]);
			}
		}
		
		public void ShowTab(TabPage page){
			if (page != null){
				if (this._TabPages != null){
					if (!this.TabPages.Contains(page)
					    && this._TabPages.Contains(page)){
						
						//	Get insert point from backup of pages
						int pageIndex = this._TabPages.IndexOf(page);
						if (pageIndex > 0){
							int start = pageIndex -1;
							
							//	Check for presence of earlier pages in the visible tabs
							for (int index = start; index >= 0; index--){
								if (this.TabPages.Contains(this._TabPages[index])){
									
									//	Set insert point to the right of the last present tab
									pageIndex = this.TabPages.IndexOf(this._TabPages[index]) + 1;
									break;
								}
							}
						}
						
						//	Insert the page, or add to the end
						if  ((pageIndex >= 0) && (pageIndex < this.TabPages.Count)){
							this.TabPages.Insert(pageIndex, page);
						} else {
							this.TabPages.Add(page);
						}
					}
				} else {
					
					//	If the page is not found at all then just add it
					if (!this.TabPages.Contains(page)){
						this.TabPages.Add(page);
					}
				}
			}
		}
		
		public void ShowTab(int index){
			if (this.IsValidTabIndex(index)){
				this.ShowTab(this._TabPages[index]);
			}
		}

		public void ShowTab(string key){
			if (this._TabPages != null){
				TabPage tab = this._TabPages.Find(delegate(TabPage page){return page.Name.Equals(key, StringComparison.OrdinalIgnoreCase);});
				this.ShowTab(tab);
			}
		}

		private bool IsValidTabIndex(int index){
			this.BackupTabPages();
			return ((index >= 0) && (index < this._TabPages.Count));
		}

		private void BackupTabPages(){
			if (this._TabPages == null){
				this._TabPages = new List<TabPage>();
				foreach (TabPage page in this.TabPages){
					this._TabPages.Add(page);
				}
			}
		}
		
		#endregion

		#region Drag 'n' Drop
		
 		protected override void OnMouseDown(MouseEventArgs e){
  			base.OnMouseDown(e);
  			if (this.AllowDrop){
  			    this._dragStartPosition = new Point(e.X, e.Y);
  			}
  		}
  		
  		protected override void OnMouseUp(MouseEventArgs e){
  			base.OnMouseUp(e);
  			if (this.AllowDrop){
  			    this._dragStartPosition = Point.Empty;
  			}
  		}
  		
  		protected override void OnDragOver(DragEventArgs drgevent){
  			base.OnDragOver(drgevent);
  
  	        if (drgevent.Data.GetDataPresent(typeof(TabPage))){
  				drgevent.Effect = DragDropEffects.Move;
  			} else {
  				drgevent.Effect = DragDropEffects.None;
  		    }
  		}
  		
  		protected override void OnDragDrop(DragEventArgs drgevent){
  			base.OnDragDrop(drgevent);
  	        if (drgevent.Data.GetDataPresent(typeof(TabPage)))
  	        {
  	            drgevent.Effect = DragDropEffects.Move;
  	            
  	            TabPage dragTab = (TabPage)drgevent.Data.GetData(typeof(TabPage));
  	
  				if (this.ActiveTab == dragTab){
  					return;
  				}
  
  	            //	Capture insert point and adjust for removal of tab
 	            //	We cannot assess this after removal as differeing tab sizes will cause
  	            //	inaccuracies in the activeTab at insert point.
  	            int insertPoint = this.ActiveIndex;
  	            if (dragTab.Parent.Equals(this) && this.TabPages.IndexOf(dragTab) < insertPoint){
  	            	insertPoint --;
  	            }
  	            if (insertPoint < 0){
  	            	insertPoint = 0;
  	            }
  	            
  	            //	Remove from current position (could be another tabcontrol)
  	            ((TabControl)dragTab.Parent).TabPages.Remove(dragTab);
  	            
  	            //	Add to current position
  				this.TabPages.Insert(insertPoint, dragTab);
              	this.SelectedTab = dragTab;
              	
              	//	deal with hidden tab handling?
              }
  		}
  		
  		private void StartDragDrop(){
  			if (!this._dragStartPosition.IsEmpty){
  			    TabPage dragTab = this.SelectedTab;
  			    if (dragTab != null){
  			    	//	Test for movement greater than the drag activation trigger area
  				    Rectangle dragTestRect = new Rectangle(this._dragStartPosition, Size.Empty);
  				    dragTestRect.Inflate(SystemInformation.DragSize);
  				    Point pt = this.PointToClient(Control.MousePosition);
  				    if (!dragTestRect.Contains(pt)){
  			            this.DoDragDrop(dragTab, DragDropEffects.All);
  					    this._dragStartPosition = Point.Empty;
  				    }
  			    }
  			}
  		}

		#endregion
		
		#region Events

		[Category("Action")] public event ScrollEventHandler HScroll;
		[Category("Action")] public event EventHandler<TabControlEventArgs> TabImageClick;
		[Category("Action")] public event EventHandler<TabControlCancelEventArgs> TabClosing;
		
		#endregion

		#region	Base class event processing

		protected override void OnFontChanged(EventArgs e){
			IntPtr hFont = this.Font.ToHfont();
			NativeMethods.SendMessage(this.Handle, NativeMethods.WM_SETFONT, hFont, (IntPtr)(-1));
			NativeMethods.SendMessage(this.Handle, NativeMethods.WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero);
			this.UpdateStyles();
			if (this.Visible){
				this.Invalidate();
			}
		}

		protected override void OnResize(EventArgs e)
		{
			//	Recreate the buffer for manual double buffering
			if (this.Width > 0 && this.Height > 0){
				if (this._BackImage != null){
					this._BackImage.Dispose();
					this._BackImage = null;
				}
				if (this._BackBufferGraphics != null){
					this._BackBufferGraphics.Dispose();
				}
				if (this._BackBuffer != null){
					this._BackBuffer.Dispose();
				}

				this._BackBuffer = new Bitmap(this.Width, this.Height);
				this._BackBufferGraphics = Graphics.FromImage(this._BackBuffer);

				if (this._TabBufferGraphics != null){
					this._TabBufferGraphics.Dispose();
				}
				if (this._TabBuffer != null){
					this._TabBuffer.Dispose();
				}

				this._TabBuffer = new Bitmap(this.Width, this.Height);
				this._TabBufferGraphics = Graphics.FromImage(this._TabBuffer);

				if (this._BackImage != null){
					this._BackImage.Dispose();
					this._BackImage = null;
				}

			}
			base.OnResize(e);
		}
		
		protected override void OnParentBackColorChanged(EventArgs e){
			if (this._BackImage != null){
				this._BackImage.Dispose();
				this._BackImage = null;
			}
			base.OnParentBackColorChanged(e);
		}

		protected override void OnParentBackgroundImageChanged(EventArgs e){
			if (this._BackImage != null){
				this._BackImage.Dispose();
				this._BackImage = null;
			}
			base.OnParentBackgroundImageChanged(e);
		}
		
		private void OnParentResize(object sender, EventArgs e){
			if (this.Visible){
				this.Invalidate();
			}
		}
		
	
		protected override void OnParentChanged(EventArgs e){
			base.OnParentChanged(e);
			if (this.Parent != null){
				this.Parent.Resize += this.OnParentResize;
			}
		}
		
		protected override void OnSelecting(TabControlCancelEventArgs e)
		{
			base.OnSelecting(e);
			
			//	Do not allow selecting of disabled tabs
			if (e.Action == TabControlAction.Selecting && e.TabPage != null && !e.TabPage.Enabled){
				e.Cancel = true;
			}
		}
		
		protected override void OnMove(EventArgs e){
			if (this.Width > 0 && this.Height > 0){
				if (this._BackImage != null){
					this._BackImage.Dispose();
					this._BackImage = null;
				}
			}
			base.OnMove(e);
			this.Invalidate();
		}
		
		protected override void OnControlAdded(ControlEventArgs e){
			base.OnControlAdded(e);
			if (this.Visible){
				this.Invalidate();
			}
		}

		protected override void OnControlRemoved(ControlEventArgs e){
			base.OnControlRemoved(e);
			if (this.Visible){
				this.Invalidate();
			}
		}
		
		
	[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
	protected override bool ProcessMnemonic(char charCode){
		    foreach (TabPage page in this.TabPages){
		        if (IsMnemonic(charCode, page.Text)){
		            this.SelectedTab = page;
		            return true;
		        }
		    }
		    return base.ProcessMnemonic(charCode);
		}
				
		protected override void OnSelectedIndexChanged(EventArgs e){
			base.OnSelectedIndexChanged(e);
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		[System.Diagnostics.DebuggerStepThrough()]
		protected override void WndProc(ref Message m){
			
			switch (m.Msg) {
				case NativeMethods.WM_HSCROLL:
					
					//	Raise the scroll event when the scroller is scrolled
					base.WndProc(ref m);
					this.OnHScroll(new ScrollEventArgs(((ScrollEventType)NativeMethods.LoWord(m.WParam)),_oldValue, NativeMethods.HiWord(m.WParam), ScrollOrientation.HorizontalScroll));
					break;
//				case NativeMethods.WM_PAINT:
//					
//					//	Handle painting ourselves rather than call the base OnPaint.
//					CustomPaint(ref m);
//					break;

				default:
					base.WndProc(ref m);
					break;
					
			}
		}

		protected override void OnMouseClick(MouseEventArgs e){
			int index = this.ActiveIndex;
			
			//	If we are clicking on an image then raise the ImageClicked event before raising the standard mouse click event
			//	if there if a handler.
			if (index > -1 && this.TabImageClick != null
			    && (this.TabPages[index].ImageIndex > -1 || !string.IsNullOrEmpty(this.TabPages[index].ImageKey))
			    && this.GetTabImageRect(index).Contains(this.MousePosition)){
				this.OnTabImageClick(new TabControlEventArgs(this.TabPages[index], index, TabControlAction.Selected));
				
				//	Fire the base event
				base.OnMouseClick(e);
				
			} else if (!this.DesignMode && index > -1 && this._StyleProvider.ShowTabCloser && this.GetTabCloserRect(index).Contains(this.MousePosition)){
				
				//	If we are clicking on a closer then remove the tab instead of raising the standard mouse click event
				//	But raise the tab closing event first
				TabPage tab = this.ActiveTab;
				TabControlCancelEventArgs args = new TabControlCancelEventArgs(tab, index, false, TabControlAction.Deselecting);
				this.OnTabClosing(args);
				
				if (!args.Cancel){
					this.TabPages.Remove(tab);
					tab.Dispose();
				}
			} else {
				//	Fire the base event
				base.OnMouseClick(e);
			}
		}

		protected virtual void OnTabImageClick(TabControlEventArgs e){
			if (this.TabImageClick != null){
				this.TabImageClick(this, e);
			}
		}

		protected virtual void OnTabClosing(TabControlCancelEventArgs e){
			if (this.TabClosing != null){
				this.TabClosing(this, e);
			}
		}		
		
		protected virtual void OnHScroll(ScrollEventArgs e){
			//	repaint the moved tabs
			this.Invalidate();
			
			//	Raise the event
			if (this.HScroll != null){
				this.HScroll(this, e);
			}
			
			if (e.Type == ScrollEventType.EndScroll){
				this._oldValue = e.NewValue;
			}
		}
		
		protected override void OnMouseMove(MouseEventArgs e){
			base.OnMouseMove(e);
			if (this._StyleProvider.ShowTabCloser){
				Rectangle tabRect = this._StyleProvider.GetTabRect(this.ActiveIndex);
				if (tabRect.Contains(this.MousePosition)){
					this.Invalidate();
				}
			}
			
			//	Initialise Drag Drop
  			if (this.AllowDrop && e.Button == MouseButtons.Left){
  				this.StartDragDrop();
  			}
		}

        #endregion

        #region	Basic drawing methods

        //		private void CustomPaint(ref Message m){
        //			NativeMethods.PAINTSTRUCT paintStruct = new NativeMethods.PAINTSTRUCT();
        //			NativeMethods.BeginPaint(m.HWnd, ref paintStruct);
        //			using (Graphics screenGraphics = this.CreateGraphics()) {
        //				this.CustomPaint(screenGraphics);
        //			}
        //			NativeMethods.EndPaint(m.HWnd, ref paintStruct);
        //		}

        /*protected override void OnPaint(PaintEventArgs e){
            //	We must always paint the entire area of the tab control
            if (e.ClipRectangle.Equals(this.ClientRectangle)){
				this.CustomPaint(e.Graphics);
			} else {
				//	it is less intensive to just reinvoke the paint with the whole surface available to draw on.
				this.Invalidate();
			}
        }*/

        // HACK: Modded
        private bool _painting = false;
        protected override void OnPaint(PaintEventArgs e)
        {
            //	We must always paint the entire area of the tab control
            if (e.ClipRectangle.Equals(this.ClientRectangle) || _painting)
            {
                _painting = false;
                this.CustomPaint(e.Graphics);
            }
            else
            {
                //	it is less intensive to just reinvoke the paint with the whole surface available to draw on.
                _painting = true;
                this.Invalidate();
            }
        }

        private void CustomPaint(Graphics screenGraphics){
			//	We render into a bitmap that is then drawn in one shot rather than using
			//	double buffering built into the control as the built in buffering
			// 	messes up the background painting.
			//	Equally the .Net 2.0 BufferedGraphics object causes the background painting
			//	to mess up, which is why we use this .Net 1.1 buffering technique.
			
			//	Buffer code from Gil. Schmidt http://www.codeproject.com/KB/graphics/DoubleBuffering.aspx
			
			if (this.Width > 0 && this.Height > 0){
				if (this._BackImage == null){
					//	Cached Background Image
					this._BackImage = new Bitmap(this.Width, this.Height);
					Graphics backGraphics = Graphics.FromImage(this._BackImage);
					backGraphics.Clear(Color.Transparent);
					this.PaintTransparentBackground(backGraphics, this.ClientRectangle);
				}
		
				this._BackBufferGraphics.Clear(Color.Transparent);
				this._BackBufferGraphics.DrawImageUnscaled(this._BackImage, 0, 0);
	
				this._TabBufferGraphics.Clear(Color.Transparent);
				
				if (this.TabCount > 0) {
	
					//	When top or bottom and scrollable we need to clip the sides from painting the tabs.
					//	Left and right are always multiline.
					if (this.Alignment <= TabAlignment.Bottom && !this.Multiline){
						this._TabBufferGraphics.Clip = new Region(new RectangleF(this.ClientRectangle.X + 3, this.ClientRectangle.Y, this.ClientRectangle.Width - 6, this.ClientRectangle.Height));
					}
					
					//	Draw each tabpage from right to left.  We do it this way to handle
					//	the overlap correctly.
					if (this.Multiline) {
						for (int row = 0; row < this.RowCount; row++) {
							for (int index = this.TabCount - 1; index >= 0; index--) {
								if (index != this.SelectedIndex && (this.RowCount == 1 || this.GetTabRow(index) == row)) {
									this.DrawTabPage(index, this._TabBufferGraphics);
								}
							}
						}
					} else {
						for (int index = this.TabCount - 1; index >= 0; index--) {
							if (index != this.SelectedIndex) {
								this.DrawTabPage(index, this._TabBufferGraphics);
							}
						}
					}
	
					//	The selected tab must be drawn last so it appears on top.
					if (this.SelectedIndex > -1) {
						this.DrawTabPage(this.SelectedIndex, this._TabBufferGraphics);
					}
				}
				this._TabBufferGraphics.Flush();
				
				//	Paint the tabs on top of the background
				
				// Create a new color matrix and set the alpha value to 0.5
				ColorMatrix alphaMatrix = new ColorMatrix();
				alphaMatrix.Matrix00 = alphaMatrix.Matrix11 = alphaMatrix.Matrix22 = alphaMatrix.Matrix44 = 1;
				alphaMatrix.Matrix33 = this._StyleProvider.Opacity;
				
				// Create a new image attribute object and set the color matrix to
				// the one just created
				using (ImageAttributes alphaAttributes = new ImageAttributes()){
					
					alphaAttributes.SetColorMatrix(alphaMatrix);
					
					// Draw the original image with the image attributes specified
					this._BackBufferGraphics.DrawImage(this._TabBuffer,
					                                   new Rectangle(0,0,this._TabBuffer.Width, this._TabBuffer.Height),
					                                   0,0,this._TabBuffer.Width, this._TabBuffer.Height, GraphicsUnit.Pixel,
					                                   alphaAttributes);
				}
				
				this._BackBufferGraphics.Flush();
				
				//	Now paint this to the screen
				
				
				//	We want to paint the whole tabstrip and border every time
				//	so that the hot areas update correctly, along with any overlaps
				
				//	paint the tabs etc.
				if (this.RightToLeftLayout){
					screenGraphics.DrawImageUnscaled(this._BackBuffer, -1, 0);
				} else {
					screenGraphics.DrawImageUnscaled(this._BackBuffer, 0, 0);
				}
			}
		}
		
		protected void PaintTransparentBackground(Graphics graphics, Rectangle clipRect)
		{

			if ((this.Parent != null)) {
				
				//	Set the cliprect to be relative to the parent
				clipRect.Offset(this.Location);

				//	Save the current state before we do anything.
				GraphicsState state = graphics.Save();
				
				//	Set the graphicsobject to be relative to the parent
				graphics.TranslateTransform((float)-this.Location.X, (float)-this.Location.Y);
				graphics.SmoothingMode = SmoothingMode.HighSpeed;
				
				//	Paint the parent
				PaintEventArgs e = new PaintEventArgs(graphics, clipRect);
				try {
					this.InvokePaintBackground(this.Parent, e);
					this.InvokePaint(this.Parent, e);
				} finally {
					//	Restore the graphics state and the clipRect to their original locations
					graphics.Restore(state);
					clipRect.Offset(-this.Location.X, -this.Location.Y);
				}
			}
		}
		
		private void DrawTabPage(int index, Graphics graphics)
		{
			graphics.SmoothingMode = SmoothingMode.HighSpeed;
			
			//	Get TabPageBorder
			using (GraphicsPath tabPageBorderPath = this.GetTabPageBorder(index)) {
				
				//	Paint the background
				using (Brush fillBrush = this._StyleProvider.GetPageBackgroundBrush(index)){
					graphics.FillPath(fillBrush, tabPageBorderPath);
				}
				
				if (this._Style != TabStyle.None){
					
					//	Paint the tab
					this._StyleProvider.PaintTab(index, graphics);
					
					//	Draw any image
					this.DrawTabImage(index, graphics);

					//	Draw the text
					this.DrawTabText(index, graphics);

				}
				
				//	Paint the border
				this.DrawTabBorder(tabPageBorderPath, index, graphics);
				
			}
		}

		private void DrawTabBorder(GraphicsPath path, int index, Graphics graphics)
		{
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			Color borderColor;
			if (index == this.SelectedIndex) {
				borderColor = this._StyleProvider.BorderColorSelected;
			} else if (this._StyleProvider.HotTrack && index == this.ActiveIndex) {
				borderColor = this._StyleProvider.BorderColorHot;
			} else {
				borderColor = this._StyleProvider.BorderColor;
			}

			using (Pen borderPen =new Pen(borderColor)){
				graphics.DrawPath(borderPen, path);
			}
		}

		private void DrawTabText(int index, Graphics graphics)
		{
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			Rectangle tabBounds = this.GetTabTextRect(index);
			
			if (this.SelectedIndex == index) {
				using (Brush textBrush = new SolidBrush(this._StyleProvider.TextColorSelected)){
					graphics.DrawString(this.TabPages[index].Text, this.Font, textBrush, tabBounds, this.GetStringFormat());
				}
			} else {
				if (this.TabPages[index].Enabled) {
					using (Brush textBrush = new SolidBrush(this._StyleProvider.TextColor)){
						graphics.DrawString(this.TabPages[index].Text, this.Font, textBrush, tabBounds, this.GetStringFormat());
					}
				} else {
					using (Brush textBrush = new SolidBrush(this._StyleProvider.TextColorDisabled)){
						graphics.DrawString(this.TabPages[index].Text, this.Font, textBrush, tabBounds, this.GetStringFormat());
					}
				}
			}
		}

		private void DrawTabImage(int index, Graphics graphics){
			Image tabImage = null;
			if (this.TabPages[index].ImageIndex > -1 && this.ImageList != null && this.ImageList.Images.Count > this.TabPages[index].ImageIndex){
				tabImage = this.ImageList.Images[this.TabPages[index].ImageIndex];
			} else if ((!string.IsNullOrEmpty(this.TabPages[index].ImageKey) && !this.TabPages[index].ImageKey.Equals("(none)", StringComparison.OrdinalIgnoreCase))
			           && this.ImageList != null && this.ImageList.Images.ContainsKey(this.TabPages[index].ImageKey)) {
				tabImage = this.ImageList.Images[this.TabPages[index].ImageKey];
			}

			if (tabImage != null) {
				if (this.RightToLeftLayout){
					tabImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
				}
				Rectangle imageRect = this.GetTabImageRect(index);
				if (this.TabPages[index].Enabled){
					graphics.DrawImage(tabImage, imageRect);
				} else {
					ControlPaint.DrawImageDisabled(graphics, tabImage, imageRect.X, imageRect.Y, Color.Transparent);
				}
			}
		}

		#endregion

		#region String formatting

		private StringFormat GetStringFormat()
		{
			StringFormat format = null;
			
			//	Rotate Text by 90 degrees for left and right tabs
			switch (this.Alignment) {
				case TabAlignment.Top:
				case TabAlignment.Bottom:
					format = new StringFormat();
					break;
				case TabAlignment.Left:
				case TabAlignment.Right:
					format = new StringFormat(StringFormatFlags.DirectionVertical);
					break;
			}
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			if (this.FindForm() != null && this.FindForm().KeyPreview){
				format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
			} else {
				format.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
			}
			if (this.RightToLeft == RightToLeft.Yes){
				format.FormatFlags = format.FormatFlags | StringFormatFlags.DirectionRightToLeft;
			}
			return format;
		}
		
		#endregion

		#region Tab borders and bounds properties
		
		private GraphicsPath GetTabPageBorder(int index){
			
			GraphicsPath path = new GraphicsPath();
			Rectangle pageBounds = this.GetPageBounds(index);
			Rectangle tabBounds = this._StyleProvider.GetTabRect(index);
			this._StyleProvider.AddTabBorder(path, tabBounds);
			this.AddPageBorder(path, pageBounds, tabBounds);
			
			path.CloseFigure();
			return path;
		}

		public Rectangle GetPageBounds(int index){
			Rectangle pageBounds = this.TabPages[index].Bounds;
			pageBounds.Width += 1;
			pageBounds.Height += 1;
			pageBounds.X -= 1;
			pageBounds.Y -= 1;
			
			if (pageBounds.Bottom > this.Height - 4){
				pageBounds.Height -= (pageBounds.Bottom - this.Height + 4);
			}
			return pageBounds;
		}

		private Rectangle GetTabTextRect(int index){
			Rectangle textRect = new Rectangle();
			using (GraphicsPath path = this._StyleProvider.GetTabBorder(index)){
				RectangleF tabBounds = path.GetBounds();
			
				textRect = new Rectangle((int)tabBounds.X, (int)tabBounds.Y, (int)tabBounds.Width, (int)tabBounds.Height);
				
				//	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
				switch (this.Alignment) {
					case TabAlignment.Top:
						textRect.Y += 4;
						textRect.Height -= 6;
						break;
					case TabAlignment.Bottom:
						textRect.Y += 2;
						textRect.Height -= 6;
						break;
					case TabAlignment.Left:
						textRect.X += 4;
						textRect.Width -= 6;
						break;
					case TabAlignment.Right:
						textRect.X += 2;
						textRect.Width -= 6;
						break;
				}

				//	If there is an image allow for it
				if (this.ImageList != null && (this.TabPages[index].ImageIndex > -1 
				                               || (!string.IsNullOrEmpty(this.TabPages[index].ImageKey)
				                                   && !this.TabPages[index].ImageKey.Equals("(none)", StringComparison.OrdinalIgnoreCase)))){
					Rectangle imageRect = this.GetTabImageRect(index);
					if ((this._StyleProvider.ImageAlign & NativeMethods.AnyLeftAlign) != ((ContentAlignment) 0)) {
						if (this.Alignment <= TabAlignment.Bottom) {
							textRect.X = imageRect.Right + 4;
							textRect.Width -= (textRect.Right - (int)tabBounds.Right);
						} else {
							textRect.Y = imageRect.Y + 4;
							textRect.Height -= (textRect.Bottom - (int)tabBounds.Bottom);
						}
						//	If there is a closer allow for it
						if (this._StyleProvider.ShowTabCloser) {
							Rectangle closerRect = this.GetTabCloserRect(index);
							if (this.Alignment <= TabAlignment.Bottom) {
								if (this.RightToLeftLayout){
									textRect.Width -= (closerRect.Right + 4 - textRect.X);
									textRect.X = closerRect.Right + 4;
								} else {
									textRect.Width -= ((int)tabBounds.Right - closerRect.X + 4);
								}
							} else {
								if (this.RightToLeftLayout){
									textRect.Height -= (closerRect.Bottom + 4 - textRect.Y);
									textRect.Y = closerRect.Bottom + 4;
								} else {
									textRect.Height -= ((int)tabBounds.Bottom - closerRect.Y + 4);
								}
							}
						}
					} else if ((this._StyleProvider.ImageAlign & NativeMethods.AnyCenterAlign) != ((ContentAlignment) 0)) {
						//	If there is a closer allow for it
						if (this._StyleProvider.ShowTabCloser) {
							Rectangle closerRect = this.GetTabCloserRect(index);
							if (this.Alignment <= TabAlignment.Bottom) {
								if (this.RightToLeftLayout){
									textRect.Width -= (closerRect.Right + 4 - textRect.X);
									textRect.X = closerRect.Right + 4;
								} else {
									textRect.Width -= ((int)tabBounds.Right - closerRect.X + 4);
								}
							} else {
								if (this.RightToLeftLayout){
									textRect.Height -= (closerRect.Bottom + 4 - textRect.Y);
									textRect.Y = closerRect.Bottom + 4;
								} else {
									textRect.Height -= ((int)tabBounds.Bottom - closerRect.Y + 4);
								}
							}
						}
					} else {
						if (this.Alignment <= TabAlignment.Bottom) {
							textRect.Width -= ((int)tabBounds.Right - imageRect.X + 4);
						} else {
							textRect.Height -= ((int)tabBounds.Bottom - imageRect.Y + 4);
						}
						//	If there is a closer allow for it
						if (this._StyleProvider.ShowTabCloser) {
							Rectangle closerRect = this.GetTabCloserRect(index);
							if (this.Alignment <= TabAlignment.Bottom) {
								if (this.RightToLeftLayout){
									textRect.Width -= (closerRect.Right + 4 - textRect.X);
									textRect.X = closerRect.Right + 4;
								} else {
									textRect.Width -= ((int)tabBounds.Right - closerRect.X + 4);
								}
							} else {
								if (this.RightToLeftLayout){
									textRect.Height -= (closerRect.Bottom + 4 - textRect.Y);
									textRect.Y = closerRect.Bottom + 4;
								} else {
									textRect.Height -= ((int)tabBounds.Bottom - closerRect.Y + 4);
								}
							}
						}
					}
				} else {
					//	If there is a closer allow for it
					if (this._StyleProvider.ShowTabCloser) {
						Rectangle closerRect = this.GetTabCloserRect(index);
						if (this.Alignment <= TabAlignment.Bottom) {
							if (this.RightToLeftLayout){
								textRect.Width -= (closerRect.Right + 4 - textRect.X);
								textRect.X = closerRect.Right + 4;
							} else {
								textRect.Width -= ((int)tabBounds.Right - closerRect.X + 4);
							}
						} else {
							if (this.RightToLeftLayout){
								textRect.Height -= (closerRect.Bottom + 4 - textRect.Y);
								textRect.Y = closerRect.Bottom + 4;
							} else {
								textRect.Height -= ((int)tabBounds.Bottom - closerRect.Y + 4);
							}
						}
					}
				}
	
		
				//	Ensure it fits inside the path at the centre line
				if (this.Alignment <= TabAlignment.Bottom) {
					while (!path.IsVisible(textRect.Right, textRect.Y) && textRect.Width > 0){
						textRect.Width -= 1;
					}
					while (!path.IsVisible(textRect.X, textRect.Y) && textRect.Width > 0) {
						textRect.X += 1;
						textRect.Width -= 1;
					}
				} else {
					while (!path.IsVisible(textRect.X, textRect.Bottom) && textRect.Height > 0) {
						textRect.Height -= 1;
					}
					while (!path.IsVisible(textRect.X, textRect.Y) && textRect.Height > 0) {
						textRect.Y += 1;
						textRect.Height -= 1;
					}
				}			
			}
			return textRect;
		}

		public int GetTabRow(int index){
			//	All calculations will use this rect as the base point
			//	because the itemsize does not return the correct width.
			Rectangle rect = this.GetTabRect(index);
			
			int row = -1;
			
			switch (this.Alignment) {
				case TabAlignment.Top:
					row = (rect.Y - 2)/rect.Height;
					break;
				case TabAlignment.Bottom:
					row = ((this.Height - rect.Y - 2)/rect.Height) - 1;
					break;
				case TabAlignment.Left:
					row = (rect.X - 2)/rect.Width;
					break;
				case TabAlignment.Right:
					row = ((this.Width - rect.X - 2)/rect.Width) - 1;
					break;
			}
			return row;
		}

		public Point GetTabPosition(int index){

			//	If we are not multiline then the column is the index and the row is 0.
			if (!this.Multiline){
				return new Point(0, index);
			}
			
			//	If there is only one row then the column is the index
			if (this.RowCount == 1){
				return new Point(0, index);
			}
			
			//	We are in a true multi-row scenario
			int row = this.GetTabRow(index);
			Rectangle rect = this.GetTabRect(index);
			int column = -1;
			
			//	Scan from left to right along rows, skipping to next row if it is not the one we want.
			for (int testIndex = 0; testIndex < this.TabCount; testIndex ++){
				Rectangle testRect = this.GetTabRect(testIndex);
				if (this.Alignment <= TabAlignment.Bottom){
					if (testRect.Y == rect.Y){
						column += 1;
					}
				} else {
					if (testRect.X == rect.X){
						column += 1;
					}
				}
				
				if (testRect.Location.Equals(rect.Location)){
					return new Point(row, column);
				}
			}
			
			return new Point(0, 0);
		}
		
		public bool IsFirstTabInRow(int index){
			if (index < 0) {
				return false;
			}
			bool firstTabinRow = (index == 0);
			if (!firstTabinRow){
				if (this.Alignment <= TabAlignment.Bottom) {
					if (this.GetTabRect(index).X == 2){
						firstTabinRow = true;
					}
				} else {
					if (this.GetTabRect(index).Y == 2){
						firstTabinRow = true;
					}
				}
			}
			return firstTabinRow;
		}
		
		private void AddPageBorder(GraphicsPath path, Rectangle pageBounds, Rectangle tabBounds){
			switch (this.Alignment) {
				case TabAlignment.Top:
					path.AddLine(tabBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Y);
					path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
					path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
					path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
					path.AddLine(pageBounds.X, pageBounds.Y, tabBounds.X, pageBounds.Y);
					break;
				case TabAlignment.Bottom:
					path.AddLine(tabBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
					path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
					path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
					path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
					path.AddLine(pageBounds.Right, pageBounds.Bottom, tabBounds.Right, pageBounds.Bottom);
					break;
				case TabAlignment.Left:
					path.AddLine(pageBounds.X, tabBounds.Y, pageBounds.X, pageBounds.Y);
					path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
					path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, pageBounds.Bottom);
					path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
					path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, tabBounds.Bottom);
					break;
				case TabAlignment.Right:
					path.AddLine(pageBounds.Right, tabBounds.Bottom, pageBounds.Right, pageBounds.Bottom);
					path.AddLine(pageBounds.Right, pageBounds.Bottom, pageBounds.X, pageBounds.Bottom);
					path.AddLine(pageBounds.X, pageBounds.Bottom, pageBounds.X, pageBounds.Y);
					path.AddLine(pageBounds.X, pageBounds.Y, pageBounds.Right, pageBounds.Y);
					path.AddLine(pageBounds.Right, pageBounds.Y, pageBounds.Right, tabBounds.Y);
					break;
			}
		}

		private Rectangle GetTabImageRect(int index){
			using (GraphicsPath tabBorderPath = this._StyleProvider.GetTabBorder(index)){
				return this.GetTabImageRect(tabBorderPath);
			}
		}
		private Rectangle GetTabImageRect(GraphicsPath tabBorderPath){
			Rectangle imageRect = new Rectangle();
			RectangleF rect = tabBorderPath.GetBounds();
			
			//	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
			switch (this.Alignment) {
				case TabAlignment.Top:
					rect.Y += 4;
					rect.Height -= 6;
					break;
				case TabAlignment.Bottom:
					rect.Y += 2;
					rect.Height -= 6;
					break;
				case TabAlignment.Left:
					rect.X += 4;
					rect.Width -= 6;
					break;
				case TabAlignment.Right:
					rect.X += 2;
					rect.Width -= 6;
					break;
			}	
			
			//	Ensure image is fully visible
			if (this.Alignment <= TabAlignment.Bottom) {
				if ((this._StyleProvider.ImageAlign & NativeMethods.AnyLeftAlign) != ((ContentAlignment) 0)){
					imageRect = new Rectangle((int)rect.X, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 16)/2), 16, 16);
					while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Y)) {
						imageRect.X += 1;	
					}
					imageRect.X += 4;

				} else if ((this._StyleProvider.ImageAlign & NativeMethods.AnyCenterAlign) != ((ContentAlignment) 0)){
					imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)(((int)rect.Right - (int)rect.X - (int)rect.Height + 2)/2)), (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 16)/2), 16, 16);
				} else {
					imageRect = new Rectangle((int)rect.Right, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 16)/2), 16, 16);
					while (!tabBorderPath.IsVisible(imageRect.Right, imageRect.Y)) {
						imageRect.X -= 1;	
					}
					imageRect.X -= 4;
					
					//	Move it in further to allow for the tab closer
					if (this._StyleProvider.ShowTabCloser && !this.RightToLeftLayout){
						imageRect.X -= 10;
					}
				}
			} else {
				if ((this._StyleProvider.ImageAlign & NativeMethods.AnyLeftAlign) != ((ContentAlignment) 0)){
					imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 16)/2), (int)rect.Y, 16, 16);
					while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Y)) {
						imageRect.Y += 1;	
					}
					imageRect.Y += 4;
				} else if ((this._StyleProvider.ImageAlign & NativeMethods.AnyCenterAlign) != ((ContentAlignment) 0)){
					imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 16)/2), (int)rect.Y + (int)Math.Floor((double)(((int)rect.Bottom - (int)rect.Y - (int)rect.Width + 2)/2)), 16, 16);
				} else {
					imageRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 16)/2), (int)rect.Bottom , 16, 16);
					while (!tabBorderPath.IsVisible(imageRect.X, imageRect.Bottom)) {
						imageRect.Y -= 1;	
					}
					imageRect.Y -= 4;	
					
					//	Move it in further to allow for the tab closer
					if (this._StyleProvider.ShowTabCloser && !this.RightToLeftLayout){
						imageRect.Y -= 10;
					}
				}
			}
			return imageRect;
		}

		public Rectangle GetTabCloserRect(int index){
			Rectangle closerRect = new Rectangle();
			using (GraphicsPath path = this._StyleProvider.GetTabBorder(index)){
				RectangleF rect = path.GetBounds();
				
				//	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
				switch (this.Alignment) {
					case TabAlignment.Top:
						rect.Y += 4;
						rect.Height -= 6;
						break;
					case TabAlignment.Bottom:
						rect.Y += 2;
						rect.Height -= 6;
						break;
					case TabAlignment.Left:
						rect.X += 4;
						rect.Width -= 6;
						break;
					case TabAlignment.Right:
						rect.X += 2;
						rect.Width -= 6;
						break;
				}
				if (this.Alignment <= TabAlignment.Bottom) {
					if (this.RightToLeftLayout){
						closerRect = new Rectangle((int)rect.Left, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 6)/2), 6, 6);
						while (!path.IsVisible(closerRect.Left, closerRect.Y) && closerRect.Right < this.Width) {
							closerRect.X += 1;	
						}
						closerRect.X += 4;
					} else {
						closerRect = new Rectangle((int)rect.Right, (int)rect.Y + (int)Math.Floor((double)((int)rect.Height - 6)/2), 6, 6);
						while (!path.IsVisible(closerRect.Right, closerRect.Y) && closerRect.Right > -6) {
							closerRect.X -= 1;	
						}
						closerRect.X -= 4;
						}
				} else {
					if (this.RightToLeftLayout){
						closerRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 6)/2), (int)rect.Top, 6, 6);
						while (!path.IsVisible(closerRect.X, closerRect.Top) && closerRect.Bottom < this.Height) {
							closerRect.Y += 1;	
						}
						closerRect.Y += 4;
					} else {
						closerRect = new Rectangle((int)rect.X + (int)Math.Floor((double)((int)rect.Width - 6)/2), (int)rect.Bottom, 6, 6);
						while (!path.IsVisible(closerRect.X, closerRect.Bottom) && closerRect.Top > -6) {
							closerRect.Y -= 1;	
						}
						closerRect.Y -= 4;
					}
				}
			}
			return closerRect;
		}

		public new Point MousePosition{
			get {
				Point loc = this.PointToClient(Control.MousePosition);
				if (this.RightToLeftLayout){
					loc.X = (this.Width - loc.X);
				}			
				return loc;
			}
		}

		#endregion
            
	}
}
