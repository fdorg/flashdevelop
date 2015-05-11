using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using PluginCore;
using PluginCore.Localization;
using ASCompletion.Model;
using ASClassWizard.Wizards;
using ASClassWizard.Resources;
using PluginCore.Controls;

namespace ASClassWizard.Wizards
{
    public partial class ClassBrowser : SmartForm
    {

        private MemberList all;
        private List<GListBox.GListBoxItem> dataProvider;
        private FlagType invalidFlag;
        private FlagType validFlag;
        private int resultCount;
        private int topIndex;
        private int lastScore;
        private string matchToken;
        private int matchLen;

        public MemberList ClassList
        {
            get { return this.all; }
            set { this.all = value; }
        }

        public List<GListBox.GListBoxItem> DataProvider
        {
            get { return this.dataProvider; }
            set { this.dataProvider = value; }
        }

        public FlagType ExcludeFlag
        {
            get { return this.invalidFlag; }
            set { this.invalidFlag = value; }
        }

        public FlagType IncludeFlag
        {
            get { return this.validFlag; }
            set { this.validFlag = value; }
        }

        public string SelectedClass
        {
            get { return this.itemList.SelectedItem != null ? this.itemList.SelectedItem.ToString() : null; }
        }

        public ClassBrowser()
        {
            this.FormGuid = "a076f763-e85e-49a2-8688-a6d35b39e7c6";
            this.DataProvider = new List<GListBox.GListBoxItem>();
            InitializeComponent();
            InitializeLocalization();
            this.Font = PluginBase.Settings.DefaultFont;
            this.itemList.ImageList = ASCompletion.Context.ASContext.Panel.TreeIcons;
            this.itemList.ItemHeight = this.itemList.ImageList.ImageSize.Height;
            CenterToParent();
        }

        private void InitializeLocalization()
        {
            this.cancelButton.Text = TextHelper.GetString("Wizard.Button.Cancel");
            this.okButton.Text = TextHelper.GetString("Wizard.Button.Ok");
            this.Text = TextHelper.GetString("Wizard.Label.OpenType");
        }

        private void ClassBrowser_Load(object sender, EventArgs e)
        {
            ASClassWizard.Wizards.GListBox.GListBoxItem node;
            this.itemList.BeginUpdate();
            this.itemList.Items.Clear();
            if (this.ClassList != null)
            {
                foreach (MemberModel item in this.ClassList)
                {
                    if (ExcludeFlag > 0) if ((item.Flags & ExcludeFlag) > 0) continue;
                    if (IncludeFlag > 0)
                    {
                        if (!((item.Flags & IncludeFlag) > 0)) continue;
                    }
                    if (this.itemList.Items.Count > 0 && item.Name == this.itemList.Items[this.itemList.Items.Count - 1].ToString()) continue;
                    node = new ASClassWizard.Wizards.GListBox.GListBoxItem(item.Name, (item.Flags & FlagType.Interface) > 0 ? 6 : 8);
                    this.itemList.Items.Add(node);
                    this.DataProvider.Add(node);
                }
            }
            if (this.itemList.Items.Count > 0)
            {
                this.itemList.SelectedIndex = 0;
            }
            this.itemList.EndUpdate();
            this.filterBox.Focus();
            this.filterBox.SelectAll();
        }

        /// <summary>
        /// Filder the list
        /// </summary>
        private void filterBox_TextChanged( Object sender, EventArgs e)
        {
            string text = this.filterBox.Text;
            this.itemList.BeginUpdate();
            this.itemList.Items.Clear();

            topIndex = 0;
            List<GListBox.GListBoxItem> result = this.FilterSmart();
                
            this.itemList.Items.AddRange(result.ToArray());
            this.itemList.EndUpdate();
            if (this.itemList.Items.Count > 0)
            {
                this.itemList.SelectedIndex = Math.Min(topIndex, this.itemList.Items.Count - 1);
            }
        }

        /// <summary>
        /// Filter using CompletionList smart comparison
        /// </summary>
        private List<GListBox.GListBoxItem> FilterSmart()
        {
            lastScore = 99;
            resultCount = 0;
            matchToken = this.filterBox.Text;
            matchLen = matchToken.Length;
            return this.DataProvider.FindAll(FindAllItems);
        }

        /// <summary>
        /// Filder the results
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool FindAllItems( GListBox.GListBoxItem item )
        {
            if (matchLen == 0) return true;
            int score = PluginCore.Controls.CompletionList.SmartMatch(item.Text, matchToken, matchLen);
            if (score > 0 && score < 6)
            {
                if (score < lastScore)
                {
                    lastScore = score;
                    topIndex = resultCount;
                }
                resultCount++;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Select None button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.itemList.SelectedItem = null;
        }


        private void itemList_DoubleClick( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void filterBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                if (this.itemList.Items.Count > 0 && this.itemList.SelectedIndex > 0)
                {
                    --this.itemList.SelectedIndex;
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                if (this.itemList.Items.Count > 0 && this.itemList.SelectedIndex < this.itemList.Items.Count - 1)
                {
                    ++this.itemList.SelectedIndex;
                }
            }
        }
    }
}
