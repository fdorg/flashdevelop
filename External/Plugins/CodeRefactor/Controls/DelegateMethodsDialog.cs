// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;
using ASCompletion.Model;
using PluginCore.Controls;
using PluginCore;
using ASCompletion.Completion;
using PluginCore.Helpers;

namespace CodeRefactor.Controls
{
    public class DelegateMethodsDialog : SmartForm
    {
        Button btnOK;
        Button btnCancel;
        CheckedListBox checkedListBox;
        Dictionary<MemberModel, ClassModel> members;
        public Dictionary<MemberModel, ClassModel> checkedMembers;
        Dictionary<string, MemberModel> members2;

        public DelegateMethodsDialog()
        {
            Owner = (Form)PluginBase.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "5e8c8d89-b70d-4840-9f49-1027b226517a";
            Text = TextHelper.GetString("Title.DelegateMethods");
            InitializeComponent();
            btnOK.Focus();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            btnOK = new ButtonEx();
            checkedListBox = new CheckedListBox();
            btnCancel = new ButtonEx();
            SuspendLayout();
            // 
            // btnOK
            //
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new System.Drawing.Point(303, 288);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(80, 23);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += OkButtonClick;
            // 
            // checkedListBox
            //
            checkedListBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            checkedListBox.CheckOnClick = true;
            checkedListBox.FormattingEnabled = true;
            checkedListBox.Font = PluginBase.Settings.DefaultFont; // Do not remove!!!
            checkedListBox.Location = new System.Drawing.Point(11, 11);
            checkedListBox.Name = "checkedListBox";
            checkedListBox.Size = new System.Drawing.Size(463, 269);
            checkedListBox.IntegralHeight = false;
            checkedListBox.TabIndex = 2;
            // 
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(389, 288);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(85, 23);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += CancelButtonClick;
            // 
            // DelegateMethodsDialog
            //
            ShowIcon = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            MinimumSize = new System.Drawing.Size(300, 200);
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(485, 323);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            Controls.Add(checkedListBox);
            Name = "DelegateMethodsDialog";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        #endregion

        #region Methods And Event Handlers

        public void FillData(Dictionary<MemberModel, ClassModel> members)
        {
            this.members = members;
            string separatorInserted = null;
            members2 = new Dictionary<string, MemberModel>();
            var items = checkedListBox.Items;
            var keys = members.Keys;
            items.Clear(); // Clear items...
            foreach (var member in keys)
            {
                var qname = members[member].QualifiedName;
                if (separatorInserted != qname)
                {
                    separatorInserted = qname;
                    items.Add("--- " + qname);
                }
                var label = TemplateUtils.ToDeclarationString(member, TemplateUtils.GetTemplate("MethodDeclaration"));
                label = label.Replace(SnippetHelper.BOUNDARY, "")
                    .Replace(SnippetHelper.ENTRYPOINT, "")
                    .Replace(SnippetHelper.EXITPOINT, "");
                if ((member.Flags & FlagType.Getter) > 0) label = "get " + label;
                else if ((member.Flags & FlagType.Setter) > 0) label = "set " + label;
                items.Add(label, false);
                members2.Add(label, member);
            }
        }

        void OkButtonClick(object sender, EventArgs e)
        {
            checkedMembers = new Dictionary<MemberModel, ClassModel>();
            foreach (string item in checkedListBox.CheckedItems)
            {
                if (item.StartsWithOrdinal("---")) continue;
                checkedMembers[members2[item]] = members[members2[item]];
            }
            var cancelArgs = new CancelEventArgs(false);
            OnValidating(cancelArgs);
            if (!cancelArgs.Cancel)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        void CancelButtonClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}