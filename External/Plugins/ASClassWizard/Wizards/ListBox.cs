// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginCore;
using System.Drawing;
using System.Windows.Forms;

namespace ASClassWizard.Wizards
{
    public class ListBox : ListBoxEx, IThemeHandler
    {
        public ListBox()
        {
            // Set owner draw mode
            this.ItemHeight = this.Font.Height + 2;
            this.DrawMode = DrawMode.OwnerDrawFixed;
        }

        public ImageList ImageList { get; set; }

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
            Rectangle bounds = e.Bounds;
            Size imageSize = ImageList.ImageSize;
            try
            {
                var item = (ListBoxItem)Items[e.Index];
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
            // properties 
            public string Text { get; set; }

            public int ImageIndex { get; set; }

            public ListBoxItem() : this("") { }

            public ListBoxItem(string text) : this(text, -1) { }

            public ListBoxItem(string text, int index)
            {
                Text = text;
                ImageIndex = index;
            }
            
            public override string ToString() => Text;
        }
    }
}
