using PluginCore;
using System.Drawing;
using System.Windows.Forms;

namespace ASClassWizard.Wizards
{
    public class ListBox : ListBoxEx, IThemeHandler
    {
        private ImageList _myImageList;

        public ListBox()
        {
            // Set owner draw mode
            this.ItemHeight = this.Font.Height + 2;
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public ImageList ImageList
        {
            get { return _myImageList; }
            set { _myImageList = value; }
        }

        public void AfterTheming()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BorderColor = PluginBase.MainForm.GetThemeColor("ListBox.BorderColor", SystemColors.ControlText);
            this.ForeColor = PluginBase.MainForm.GetThemeColor("ListBox.ForeColor", SystemColors.ControlText);
            this.BackColor = PluginBase.MainForm.GetThemeColor("ListBox.BackColor", SystemColors.Window);
            this.Refresh();
        }

        protected override void OnDrawItem( System.Windows.Forms.DrawItemEventArgs e )
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            ListBoxItem item;
            Rectangle bounds = e.Bounds;
            Size imageSize = _myImageList.ImageSize;
            try
            {
                item = (ListBoxItem)Items[e.Index];
                if (item.ImageIndex != -1)
                {
                    ImageList.Draw(e.Graphics, bounds.Left, bounds.Top, item.ImageIndex);
                    e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left + imageSize.Width, bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left, bounds.Top);
                }
            }
            catch
            {
                if (e.Index != -1)
                {
                    e.Graphics.DrawString(Items[e.Index].ToString(), e.Font,
                        new SolidBrush(e.ForeColor), bounds.Left, bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(Text, e.Font, new SolidBrush(e.ForeColor),
                        bounds.Left, bounds.Top);
                }
            }
            base.OnDrawItem(e);
        }

        public class ListBoxItem
        {
            public int matchScore;
        
            private string _myText;
            private int _myImageIndex;

            // properties 
            public string Text
            {
                get { return _myText; }
                set { _myText = value; }
            }
            public int ImageIndex
            {
                get { return _myImageIndex; }
                set { _myImageIndex = value; }
            }
        
            public ListBoxItem(string text, int index)
            {
                _myText = text;
                _myImageIndex = index;
            }
            public ListBoxItem(string text) : this(text, -1) { }
            public ListBoxItem() : this("") { }
            public override string ToString()
            {
             return _myText;
            }
        }
    }
}
