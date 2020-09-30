/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
 */

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms
{
	[ToolboxItem(false)]
	public abstract class TabStyleProvider : Component
	{
		#region Constructor
		
		protected TabStyleProvider(CustomTabControl tabControl)
        {
			_TabControl = tabControl;
            _BorderColor = Color.Empty;
			_BorderColorSelected = Color.Empty;
			_FocusColor = Color.Orange;
            _ImageAlign = _TabControl.RightToLeftLayout ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft;
            HotTrack = true;
            //	Must set after the _Overlap as this is used in the calculations of the actual padding
			Padding = new Point(6,3);
		}
		
		#endregion

		#region Factory Methods
		
		public static TabStyleProvider CreateProvider(CustomTabControl tabControl)
        {
            //	Depending on the display style of the tabControl generate an appropriate provider.
            TabStyleProvider provider = tabControl.DisplayStyle switch
            {
                TabStyle.None => new TabStyleNoneProvider(tabControl),
                TabStyle.Default => new TabStyleDefaultProvider(tabControl),
                TabStyle.Angled => new TabStyleAngledProvider(tabControl),
                TabStyle.Rounded => new TabStyleRoundedProvider(tabControl),
                TabStyle.VisualStudio => new TabStyleVisualStudioProvider(tabControl),
                TabStyle.Chrome => new TabStyleChromeProvider(tabControl),
                TabStyle.IE8 => new TabStyleIE8Provider(tabControl),
                TabStyle.VS2010 => new TabStyleVS2010Provider(tabControl),
                // HACK: Added
                TabStyle.Flat => new TabStyleFlatProvider(tabControl),
                _ => new TabStyleDefaultProvider(tabControl)
            };
            provider._Style = tabControl.DisplayStyle;
			return provider;
		}
		
		#endregion
		
		#region	Protected variables
		
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected CustomTabControl _TabControl;

		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Point _Padding;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected bool _HotTrack;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected TabStyle _Style = TabStyle.Default;
		
		
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected ContentAlignment _ImageAlign;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected int _Radius = 1;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected int _Overlap;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected bool _FocusTrack;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected float _Opacity = 1;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected bool _ShowTabCloser;
		
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Color _BorderColorSelected = Color.Empty;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Color _BorderColor = Color.Empty;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Color _BorderColorHot = Color.Empty;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		
		protected Color _CloserColorActive = Color.Black;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Color _CloserColor = Color.DarkGray;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		
		protected Color _FocusColor = Color.Empty;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		
		protected Color _TextColor = Color.Empty;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Color _TextColorSelected = Color.Empty;
		[Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
		protected Color _TextColorDisabled = Color.Empty;
		
		#endregion
		
		#region overridable Methods
		
		public abstract void AddTabBorder(GraphicsPath path, Rectangle tabBounds);

		public virtual Rectangle GetTabRect(int index)
        {
			if (index < 0) return new Rectangle();
            var tabBounds = _TabControl.GetTabRect(index);
			if (_TabControl.RightToLeftLayout)
            {
				tabBounds.X = _TabControl.Width - tabBounds.Right;
			}
			var firstTabinRow = _TabControl.IsFirstTabInRow(index);
            //	Expand to overlap the tabpage
			switch (_TabControl.Alignment)
            {
				case TabAlignment.Top:
					tabBounds.Height += 2;
					break;
				case TabAlignment.Bottom:
					tabBounds.Height += 2;
					tabBounds.Y -= 2;
					break;
				case TabAlignment.Left:
					tabBounds.Width += 2;
					break;
				case TabAlignment.Right:
					tabBounds.X -= 2;
					tabBounds.Width += 2;
					break;
			}
            
			//	Greate Overlap unless first tab in the row to align with tabpage
			if ((!firstTabinRow || _TabControl.RightToLeftLayout) && _Overlap > 0)
            {
				if (_TabControl.Alignment <= TabAlignment.Bottom)
                {
					tabBounds.X -= _Overlap;
					tabBounds.Width += _Overlap;
				}
                else
                {
					tabBounds.Y -= _Overlap;
					tabBounds.Height += _Overlap;
				}
			}
            //	Adjust first tab in the row to align with tabpage
			EnsureFirstTabIsInView(ref tabBounds, index);
            return tabBounds;
		}

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#")]
		protected virtual void EnsureFirstTabIsInView(ref Rectangle tabBounds, int index)
        {
			//	Adjust first tab in the row to align with tabpage
			//	Make sure we only reposition visible tabs, as we may have scrolled out of view.
            var firstTabinRow = _TabControl.IsFirstTabInRow(index);
            if (firstTabinRow)
            {
				if (_TabControl.Alignment <= TabAlignment.Bottom)
                {
					if (_TabControl.RightToLeftLayout)
                    {
						if (tabBounds.Left < _TabControl.Right)
                        {
							int tabPageRight = _TabControl.GetPageBounds(index).Right;
							if (tabBounds.Right > tabPageRight)
                            {
								tabBounds.Width -= (tabBounds.Right - tabPageRight);
							}
						}
					}
                    else if (tabBounds.Right > 0)
                    {
                        int tabPageX = _TabControl.GetPageBounds(index).X;
                        if (tabBounds.X < tabPageX)
                        {
                            tabBounds.Width -= (tabPageX - tabBounds.X);
                            tabBounds.X = tabPageX;
                        }
                    }
                }
                else if (_TabControl.RightToLeftLayout)
                {
                    if (tabBounds.Top < _TabControl.Bottom)
                    {
                        int tabPageBottom = _TabControl.GetPageBounds(index).Bottom;
                        if (tabBounds.Bottom > tabPageBottom)
                        {
                            tabBounds.Height -= (tabBounds.Bottom - tabPageBottom);
                        }
                    }
                }
                else if (tabBounds.Bottom > 0)
                {
                    int tabPageY = _TabControl.GetPageBounds(index).Location.Y;
                    if (tabBounds.Y < tabPageY)
                    {
                        tabBounds.Height -= (tabPageY - tabBounds.Y);
                        tabBounds.Y = tabPageY;
                    }
                }
            }
		}

		protected virtual Brush GetTabBackgroundBrush(int index)
        {
			LinearGradientBrush fillBrush = null;

			//	Capture the colours dependant on selection state of the tab
			var dark = Color.FromArgb(207, 207, 207);
			var light = Color.FromArgb(242, 242, 242);
			
			if (_TabControl.SelectedIndex == index)
            {
				dark = SystemColors.ControlLight;
				light = SystemColors.Window;
			}
            else if (!_TabControl.TabPages[index].Enabled)
            {
				light = dark;
			}
            else if (_HotTrack && index == _TabControl.ActiveIndex)
            {
				//	Enable hot tracking
				light = Color.FromArgb(234, 246, 253);
				dark = Color.FromArgb(167, 217, 245);
			}
			
			//	Get the correctly aligned gradient
			Rectangle tabBounds = GetTabRect(index);
			tabBounds.Inflate(3,3);
			tabBounds.X -= 1;
			tabBounds.Y -= 1;
			switch (_TabControl.Alignment)
            {
				case TabAlignment.Top:
					if (_TabControl.SelectedIndex == index)
                    {
						dark = light;
					}
					fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
					break;
				case TabAlignment.Bottom:
					fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Vertical);
					break;
				case TabAlignment.Left:
					fillBrush = new LinearGradientBrush(tabBounds, dark, light, LinearGradientMode.Horizontal);
					break;
				case TabAlignment.Right:
					fillBrush = new LinearGradientBrush(tabBounds, light, dark, LinearGradientMode.Horizontal);
					break;
			}
            //	Add the blend
			fillBrush.Blend = GetBackgroundBlend();
            return fillBrush;
		}

		#endregion
		
		#region	Base Properties

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TabStyle DisplayStyle
        {
			get => _Style;
            set => _Style = value;
        }

		[Category("Appearance")]
		public ContentAlignment ImageAlign
        {
			get => _ImageAlign;
            set 
            {
				_ImageAlign = value;
				_TabControl.Invalidate();
			}
		}
		
		[Category("Appearance")]
		public Point Padding
        {
			get => _Padding;
            set
            {
				_Padding = value;
				//	This line will trigger the handle to recreate, therefore invalidating the control
				if (_ShowTabCloser)
                {
                    ((TabControl)_TabControl).Padding = value.X + _Radius/2 < -6
                        ? new Point(0, value.Y)
                        : new Point(value.X + _Radius/2 + 6, value.Y);
                }
                else
                {
                    ((TabControl)_TabControl).Padding = value.X + _Radius/2 < 1
                        ? new Point(0, value.Y)
                        : new Point(value.X + _Radius/2 -1, value.Y);
                }
			}
		}


		[Category("Appearance"), DefaultValue(1), Browsable(true)]
		public int Radius
        {
			get => _Radius;
            set
            {
				if (value < 1) throw new ArgumentException("The radius must be greater than 1", nameof(value));
                _Radius = value;
				//	Adjust padding
				Padding = _Padding;
			}
		}

		[Category("Appearance")]
		public int Overlap
        {
			get => _Overlap;
            set
            {
				if (value < 0) throw new ArgumentException("The tabs cannot have a negative overlap", nameof(value));
                _Overlap = value;
            }
		}
        
		[Category("Appearance")]
		public bool FocusTrack
        {
			get => _FocusTrack;
            set
            {
				_FocusTrack = value;
				_TabControl.Invalidate();
			}
		}
		
		[Category("Appearance")]
		public bool HotTrack
        {
			get => _HotTrack;
            set
            {
				_HotTrack = value;
				((TabControl)_TabControl).HotTrack = value;
			}
		}

		[Category("Appearance")]
		public bool ShowTabCloser
        {
			get => _ShowTabCloser;
            set
            {
				_ShowTabCloser = value;
				//	Adjust padding
				Padding = _Padding;
			}
		}

		[Category("Appearance")]
		public float Opacity
        {
			get => _Opacity;
            set
            {
				if (value < 0) throw new ArgumentException("The opacity must be between 0 and 1", nameof(value));
                if (value > 1) throw new ArgumentException("The opacity must be between 0 and 1", nameof(value));
                _Opacity = value;
				_TabControl.Invalidate();
			}
		}
		
		[Category("Appearance"), DefaultValue(typeof(Color), "")]
		public Color BorderColorSelected
		{
			get => _BorderColorSelected.IsEmpty ? ThemedColors.ToolBorder : _BorderColorSelected;
            set
            {
                _BorderColorSelected = value.Equals(ThemedColors.ToolBorder) ? Color.Empty : value;
                _TabControl.Invalidate();
            }
		}

		[Category("Appearance"), DefaultValue(typeof(Color), "")]
		public Color BorderColorHot
		{
			get => _BorderColorHot.IsEmpty ? SystemColors.ControlDark : _BorderColorHot;
            set
            {
                _BorderColorHot = value.Equals(SystemColors.ControlDark) ? Color.Empty : value;
                _TabControl.Invalidate();
            }
		}
		
		[Category("Appearance"), DefaultValue(typeof(Color), "")]
		public Color BorderColor
		{
			get => _BorderColor.IsEmpty ? SystemColors.ControlDark : _BorderColor;
            set
            {
                _BorderColor = value.Equals(SystemColors.ControlDark) ? Color.Empty : value;
                _TabControl.Invalidate();
            }
		}
		
		[Category("Appearance"), DefaultValue(typeof(Color), "")]
		public Color TextColor
		{
			get => _TextColor.IsEmpty ? SystemColors.ControlText : _TextColor;
            set
            {
                _TextColor = value.Equals(SystemColors.ControlText) ? Color.Empty : value;
                _TabControl.Invalidate();
            }
		}
		
		[Category("Appearance"), DefaultValue(typeof(Color), "")]
		public Color TextColorSelected
		{
			get => _TextColorSelected.IsEmpty ? SystemColors.ControlText : _TextColorSelected;
            set
            {
                _TextColorSelected = value.Equals(SystemColors.ControlText) ? Color.Empty : value;
                _TabControl.Invalidate();
            }
		}
		
		[Category("Appearance"), DefaultValue(typeof(Color), "")]
		public Color TextColorDisabled
		{
			get => _TextColor.IsEmpty ? SystemColors.ControlDark : _TextColorDisabled;
            set
            {
                _TextColorDisabled = value.Equals(SystemColors.ControlDark) ? Color.Empty : value;
                _TabControl.Invalidate();
            }
		}
		
		
		[Category("Appearance"), DefaultValue(typeof(Color), "Orange")]
		public Color FocusColor
		{
			get => _FocusColor;
            set
            {
                _FocusColor = value;
				_TabControl.Invalidate();
			}
		}

		[Category("Appearance"), DefaultValue(typeof(Color), "Black")]
		public Color CloserColorActive
		{
			get => _CloserColorActive;
            set
            {
                _CloserColorActive = value;
				_TabControl.Invalidate();
			}
		}
		
		[Category("Appearance"), DefaultValue(typeof(Color), "DarkGrey")]
		public Color CloserColor
		{
			get => _CloserColor;
            set
            {
                _CloserColor = value;
				_TabControl.Invalidate();
			}
		}
		
		#endregion

		#region Painting
		
        // HACK: Allow override
		public virtual void PaintTab(int index, Graphics graphics)
        {
            using var tabpath = GetTabBorder(index);
            using var fillBrush = GetTabBackgroundBrush(index);
            //	Paint the background
            graphics.FillPath(fillBrush, tabpath);
					
            //	Paint a focus indication
            if (_TabControl.Focused)
            {
                DrawTabFocusIndicator(tabpath, index, graphics);
            }

            //	Paint the closer
            DrawTabCloser(index, graphics);
        }
		
		protected virtual void DrawTabCloser(int index, Graphics graphics)
        {
            if (!_ShowTabCloser) return;
            var closerRect = _TabControl.GetTabCloserRect(index);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var closerPath = GetCloserPath(closerRect);
            if (closerRect.Contains(_TabControl.MousePosition))
            {
                using var closerPen = new Pen(_CloserColorActive);
                graphics.DrawPath(closerPen, closerPath);
            }
            else
            {
                using var closerPen = new Pen(_CloserColor);
                graphics.DrawPath(closerPen, closerPath);
            }
        }
		
		protected static GraphicsPath GetCloserPath(Rectangle closerRect)
        {
			var closerPath = new GraphicsPath();
			closerPath.AddLine(closerRect.X, closerRect.Y, closerRect.Right, closerRect.Bottom);
			closerPath.CloseFigure();
			closerPath.AddLine(closerRect.Right, closerRect.Y, closerRect.X, closerRect.Bottom);
			closerPath.CloseFigure();
            return closerPath;
		}

        // HACK: Allow override
        protected virtual void DrawTabFocusIndicator(GraphicsPath tabpath, int index, Graphics graphics)
        {
            if (!_FocusTrack || !_TabControl.Focused || index != _TabControl.SelectedIndex) return;
            Brush focusBrush = null;
            var pathRect = tabpath.GetBounds();
            var focusRect = Rectangle.Empty;
            switch (_TabControl.Alignment)
            {
                case TabAlignment.Top:
                    focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, (int)pathRect.Width, 4);
                    focusBrush = new LinearGradientBrush(focusRect,_FocusColor, SystemColors.Window, LinearGradientMode.Vertical);
                    break;
                case TabAlignment.Bottom:
                    focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Bottom - 4, (int)pathRect.Width, 4);
                    focusBrush = new LinearGradientBrush(focusRect, SystemColors.ControlLight, _FocusColor, LinearGradientMode.Vertical);
                    break;
                case TabAlignment.Left:
                    focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, 4, (int)pathRect.Height);
                    focusBrush = new LinearGradientBrush(focusRect, _FocusColor, SystemColors.ControlLight, LinearGradientMode.Horizontal);
                    break;
                case TabAlignment.Right:
                    focusRect = new Rectangle((int)pathRect.Right - 4, (int)pathRect.Y, 4, (int)pathRect.Height);
                    focusBrush = new LinearGradientBrush(focusRect, SystemColors.ControlLight, _FocusColor, LinearGradientMode.Horizontal);
                    break;
            }
				
            //	Ensure the focus stip does not go outside the tab
            var focusRegion = new Region(focusRect);
            focusRegion.Intersect(tabpath);
            graphics.FillRegion(focusBrush, focusRegion);
            focusRegion.Dispose();
            focusBrush.Dispose();
        }

		#endregion
		
		#region Background brushes

        Blend GetBackgroundBlend()
        {
			float[] relativeIntensities = {0f, 0.7f, 1f};
			float[] relativePositions = {0f, 0.6f, 1f};

			//	Glass look to top aligned tabs
			if (_TabControl.Alignment == TabAlignment.Top)
            {
                relativeIntensities = new[] {0f, 0.5f, 1f, 1f};
                relativePositions = new[] {0f, 0.5f, 0.51f, 1f};
            }

            return new Blend {Factors = relativeIntensities, Positions = relativePositions};
		}
		
		public virtual Brush GetPageBackgroundBrush(int index)
        {
            //	Capture the colours dependant on selection state of the tab
			var light = Color.FromArgb(242, 242, 242);
			if (_TabControl.Alignment == TabAlignment.Top)
            {
				light = Color.FromArgb(207, 207, 207);
			}
            if (_TabControl.SelectedIndex == index)
            {
				light = SystemColors.Window;
			}
            else if (!_TabControl.TabPages[index].Enabled)
            {
				light = Color.FromArgb(207, 207, 207);
			}
            else if (_HotTrack && index == _TabControl.ActiveIndex)
            {
				//	Enable hot tracking
				light = Color.FromArgb(234, 246, 253);
			}
            return new SolidBrush(light);
		}
		
		#endregion
		
		#region Tab border and rect
		
		public GraphicsPath GetTabBorder(int index)
        {
			
			var path = new GraphicsPath();
			var tabBounds = GetTabRect(index);
            AddTabBorder(path, tabBounds);
            path.CloseFigure();
			return path;
		}

		#endregion
    }
}