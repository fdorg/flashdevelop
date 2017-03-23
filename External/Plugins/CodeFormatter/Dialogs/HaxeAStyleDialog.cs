using CodeFormatter.Properties;
using FlashDevelop.Managers;
using PluginCore.Helpers;
using PluginCore.Utilities;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace CodeFormatter.Dialogs
{
    public partial class HaxeAStyleDialog : Form
    {
        private string configFile;

        private static Dictionary<string, int> bracketStyles = new Dictionary<string, int>
        {
            { "Allman", 1 },
            { "Java", 2 },
            { "Kernighan & Ritchie", 3 },
            { "Stroustrup", 4 },
            { "Whitesmith", 5 },
            { "VTK", 15 },
            { "Banner", 6 },
            { "GNU", 7 },
            { "Linux", 8 },
            { "Horstmann", 9 },
            { "One True Brace", 10 },
            { "Google", 14 },
            //{ "Mozilla", 16 }, //not supported by old version of AStyle
            { "Pico", 11 },
            { "Lisp", 12 },
        };

        private Dictionary<CheckBox, string> mapping = new Dictionary<CheckBox, string>();
        private Dictionary<string, CheckBox> reverseMapping = new Dictionary<string, CheckBox>();

        public HaxeAStyleDialog()
        {
            InitializeComponent();

            String dataDir = Path.Combine(PathHelper.DataDir, "CodeFormatter");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            configFile = Path.Combine(dataDir, "HaxeAStyleConfig.fdb");
            

            checkForceTabs.DataBindings.Add("Enabled", checkTabs, "Checked");
            checkPadAll.DataBindings.Add("Enabled", checkPadBlocks, "Checked");

            checkOneLineBrackets.DataBindings.Add("Enabled", checkAddBrackets, "Checked");
            //TODO: cross enable / disable these:
            //checkFillEmptyLines.DataBindings.Add("Enabled", checkDeleteEmptyLines, "Checked");
            //checkDeleteEmptyLines.DataBindings.Add("Enabled", checkFillEmptyLines, "Checked");
            //and these:
            //checkAddBrackets;
            //checkRemoveBrackets;

            foreach (TabPage page in this.tabControl.TabPages)
            {
                MapCheckBoxes(page);
            }

            //txtExample.Text = Encoding.UTF8.GetString(Resources.CodeExample);
            cbBracketStyle.DataSource = bracketStyles.Keys.ToArray();

            ReformatExample();
        }

        /// <summary>
        /// Saves the style settings in the config file
        /// </summary>
        private void SaveSettings()
        {
            string[] options = GetOptions();

            ObjectSerializer.Serialize(this.configFile, options);
        }

        private void LoadSettings()
        {
            string[] options = new string[0];
            if (!File.Exists(this.configFile))
            {
                this.SaveSettings();
            }
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.configFile, options);
                options = (string[])obj;
            }

            //TODO: load
            //if (options.Contains(""))
        }

        private void MapCheckBoxes(TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;
                if (check != null && check.Tag != null)
                {
                    mapping.Add(check, (string)check.Tag);
                    reverseMapping.Add((string)check.Tag, check);
                }
            }
        }

        private void AddBasicTabOptions(List<string> options, TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;
                if (check != null && IsChecked(check) && mapping.ContainsKey(check))
                {
                    options.Add(mapping[check]);
                }
            }
        }

        private void check_Click(object sender, EventArgs e)
        {
            ReformatExample();
        }

        private string[] GetOptions()
        {
            List<string> options = new List<string>();

            options.Add("--mode=cs");


            //handling default switches
            foreach (TabPage page in this.tabControl.TabPages)
            {
                AddBasicTabOptions(options, page);
            }

            //special switches

            //Tabs / Indentation
            if (IsChecked(checkForceTabs))
            {
                options.Add("--indent=force-tab=" + numIndentWidth.Value);
            }
            else if (IsChecked(checkTabs))
            {
                options.Add("--indent=tab=" + numIndentWidth.Value);
            }
            else
            {
                options.Add("--indent=spaces=" + numIndentWidth.Value);
            }

            //Brackets
            options.Add("-A" + bracketStyles[(string)cbBracketStyle.SelectedItem]);
            
            
            if (IsChecked(checkAddBrackets))
            {
                if (IsChecked(checkOneLineBrackets))
                {
                    options.Add("--add-one-line-brackets");
                }
                else
                {
                    options.Add("--add-brackets");
                }
            }

            //Padding
            if (IsChecked(checkPadBlocks))
            {
                if (IsChecked(checkPadAll))
                {
                    options.Add("--break-blocks=all");
                }
                else
                {
                    options.Add("--break-blocks");
                }
            }
            //options.Add("--indent-continuation=" + numIndentContinuation.Value); //not supported by old version of AStyle


            return options.ToArray();
        }

        private void ReformatExample()
        {
            txtExample.SelectionTabs = new int[] { 15, 30, 45, 60, 75 };
            txtExample.Text = PluginCore.PluginBase.MainForm.CurrentDocument.SciControl.Text;

            AStyleInterface astyle = new AStyleInterface();
            txtExample.Text = astyle.FormatSource(txtExample.Text, String.Join(" ", GetOptions()));
        }

        private bool IsChecked(CheckBox chk)
        {
            return chk.Checked && chk.Enabled;
        }

        private void numIndentWidth_ValueChanged(object sender, EventArgs e)
        {
            ReformatExample();
        }

        private void checkOneLineBrackets_CheckedChanged(object sender, EventArgs e)
        {
            if (checkOneLineBrackets.Checked)
            {
                checkKeepOneLineBlocks.Enabled = false;
            }
        }

        private void cbBracketStyle_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int style = bracketStyles[(string)cbBracketStyle.SelectedValue];

            switch (style)
            {
                case 2:
                case 3:
                case 4:
                case 8:
                case 10:
                    checkBreakClosing.Enabled = true;
                    break;
                default: //style enables this by default
                    checkBreakClosing.Enabled = false;
                    break;
            }

            ReformatExample();
        }
    }
}
