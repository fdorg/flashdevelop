using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TestApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            this.SuspendLayout();
            this.CreateContent();
            this.ResumeLayout(false);
        }

        public void CreateContent()
        {
            this.Text = "DockPanelSuite TestApp";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            //
            var dockPanel = new DockPanel();
            dockPanel.AllowDrop = true;
            dockPanel.TabIndex = 1;
            dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            dockPanel.AllowEndUserDocking = true;
            dockPanel.AllowEndUserNestedDocking = true;
            dockPanel.Dock = DockStyle.Fill;
            this.Controls.Add(dockPanel);
            //
            var dc = new DockContent();
            dc.TabText = "Hello Doc!";
            dc.DockPanel = dockPanel;
            dc.DockAreas = DockAreas.Document;
            var rtb = new RichTextBox();
            rtb.Dock = DockStyle.Fill;
            dc.Controls.Add(rtb);
            dc.Show();
            //
            DockContent dc2 = new DockContent();
            dc2.TabText = "Hello Doc!";
            dc2.DockPanel = dockPanel;
            dc2.DockAreas = DockAreas.Document;
            var rtb2 = new RichTextBox();
            rtb2.Dock = DockStyle.Fill;
            dc2.Controls.Add(rtb2);
            dc2.Show();
            //
            DockContent dc3 = new DockContent();
            dc3.AllowEndUserDocking = true;
            dc3.AllowDrop = true;
            dc3.TabText = "Hello Tab!";
            dc3.DockPanel = dockPanel;
            dc3.DockAreas = DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop;
            dc3.DockState = DockState.DockRight;
            var rtb3 = new RichTextBox();
            rtb3.Dock = DockStyle.Fill;
            dc3.Controls.Add(rtb3);
            dc3.Show();
            //
            DockContent dc4 = new DockContent();
            dc4.AllowEndUserDocking = true;
            dc4.AllowDrop = true;
            dc4.TabText = "Hello Tab!";
            dc4.DockPanel = dockPanel;
            dc4.DockAreas = DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Float;
            dc4.DockState = DockState.DockBottomAutoHide;
            var rtb4 = new RichTextBox();
            rtb4.Dock = DockStyle.Fill;
            dc4.Controls.Add(rtb4);
            dc4.Show();
        }

    }

}
