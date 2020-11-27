namespace ConsolePanel.Gui
{
    partial class TabbedConsole
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabConsoles = new System.Windows.Forms.TabControl();
            this.btnNew = new System.Windows.Forms.Button();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.pnlContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabConsoles
            // 
            this.tabConsoles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabConsoles.Location = new System.Drawing.Point(0, 0);
            this.tabConsoles.Name = "tabConsoles";
            this.tabConsoles.SelectedIndex = 0;
            this.tabConsoles.Size = new System.Drawing.Size(398, 199);
            this.tabConsoles.TabIndex = 0;
            this.tabConsoles.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tabConsoles_MouseClick);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNew.FlatAppearance.BorderSize = 0;
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Location = new System.Drawing.Point(379, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(16, 16);
            this.btnNew.TabIndex = 0;
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // pnlContainer
            // 
            this.pnlContainer.Controls.Add(this.btnNew);
            this.pnlContainer.Controls.Add(this.tabConsoles);
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(398, 199);
            this.pnlContainer.TabIndex = 1;
            // 
            // TabbedConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlContainer);
            this.Name = "TabbedConsole";
            this.Size = new System.Drawing.Size(398, 199);
            this.pnlContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabConsoles;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Panel pnlContainer;
    }
}
