using CodeFormatter.Preferences;
using CodeFormatter.Utilities;
using PluginCore;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CodeFormatter.Dialogs
{
    public partial class HaxeAStyleDialog : Form
    {
        private ScintillaControl txtExample;

        /// <summary>
        /// Contains a mapping of bracket styles with the corresponding option for AStyle
        /// </summary>
        private static Dictionary<string, string> bracketStyles = new Dictionary<string, string>
        {
            { "Allman", "allman" },
            { "Java", "java" },
            { "Kernighan & Ritchie", "kr" },
            { "Stroustrup", "stroustrup" },
            { "Whitesmith", "whitesmith" },
            { "VTK", "vtk" },
            { "Banner", "banner" },
            { "GNU", "gnu" },
            { "Linux", "linux" },
            { "Horstmann", "horstmann" },
            { "One True Brace", "otbs" },
            { "Google", "google" },
            //{ "Mozilla", "mozilla" }, //not supported by old version of AStyle
            { "Pico", "pico" },
            { "Lisp", "lisp" },
        };
        /// <summary>
        /// Contains an inverted version of <see cref="bracketStyles"/>
        /// </summary>
        private static Dictionary<string, string> reverseBracketStyle = new Dictionary<string, string>();

        private Dictionary<CheckBox, string> mapping = new Dictionary<CheckBox, string>();
        private Dictionary<string, CheckBox> reverseMapping = new Dictionary<string, CheckBox>();

        static HaxeAStyleDialog()
        {
            foreach (string name in bracketStyles.Keys)
            {
                reverseBracketStyle[bracketStyles[name]] = name;
            }
        }

        public HaxeAStyleDialog()
        {
            InitializeComponent();
            this.Font = PluginBase.Settings.DefaultFont;

            //Create Scintilla
            txtExample = new ScintillaControl();
            txtExample.Dock = DockStyle.Fill;
            txtExample.ConfigurationLanguage = "haxe";
            txtExample.ViewWhitespace = ScintillaNet.Enums.WhiteSpace.VisibleAlways;

            this.pnlSci.Controls.Add(txtExample);

            checkForceTabs.DataBindings.Add("Enabled", checkTabs, "Checked");
            checkPadAll.DataBindings.Add("Enabled", checkPadBlocks, "Checked");

            checkOneLineBrackets.DataBindings.Add("Enabled", checkAddBrackets, "Checked");

            foreach (TabPage page in this.tabControl.TabPages)
            {
                MapCheckBoxes(page);
            }

            cbBracketStyle.DataSource = bracketStyles.Keys.ToArray();

            LoadSettings();

            ValidateControls();
            ReformatExample();
        }

        private void SaveSettings()
        {
            HaxeAStyleHelper.SaveOptions(GetOptions());
        }

        private void LoadSettings()
        {
            SetOptions(HaxeAStyleHelper.LoadOptions());
        }

        /// <summary>
        /// Fills <see cref="mapping"/> and <see cref="reverseMapping"/>.
        /// It checks the given <paramref name="page"/> for checkboxes that have a Tag.
        /// The tab is assumed to be the flag that should be set.
        /// Checkboxes without Tag are ignored by this method and have to be handled manually.
        /// </summary>
        private void MapCheckBoxes(TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;
                if (check != null && check.Tag != null)
                {
                    //Tag is used to assign simple flags to the corresponding control
                    ///More complex options are handled in SetOptions and GetOptions<see cref=""/>
                    mapping.Add(check, (string)check.Tag);
                    reverseMapping.Add((string)check.Tag, check);
                }
            }
        }

        /// <summary>
        /// Helper method used by <see cref="GetOptions"/> to automatically read values of simple flags from the
        /// respective checkboxes into the given <paramref name="options"/>.
        /// </summary>
        /// <param name="page">The TabPage to set options for</param>
        private void GetBasicTabOptions(List<HaxeAStyleOption> options, TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;
                if (check != null && IsChecked(check) && mapping.ContainsKey(check))
                {
                    options.Add(new HaxeAStyleOption(mapping[check]));
                }
            }
        }

        /// <summary>
        /// Helper method used by <see cref="SetOptions"/> to automatically set simple flag checkboxes.
        /// </summary>
        /// <param name="page">The TabPage to search through</param>
        private void SetBasicTabOptions(HaxeAStyleOptions options, TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;

                bool hasOption = check != null && mapping.ContainsKey(check) && options.Exists(mapping[check]);

                if (hasOption)
                {
                    check.Checked = true;
                }
            }
        }

        /// <summary>
        /// Helper method to fill this dialog from the given <paramref name="options"/>.
        /// </summary>
        private void SetOptions(HaxeAStyleOptions options)
        {
            if (options.Count == 0)
            {
                return;
            }

            //set default switches
            foreach (TabPage page in this.tabControl.TabPages)
            {
                SetBasicTabOptions(options, page);
            }

            //Brackets
            cbBracketStyle.SelectedItem = reverseBracketStyle[(string)options.Find("--style").Value];

            checkOneLineBrackets.Checked = options.Exists("--add-one-line-brackets");
            checkAddBrackets.Checked = checkOneLineBrackets.Checked || options.Exists("--add-brackets");

            //Padding
            HaxeAStyleOption breakBlocks = options.Find("--break-blocks");

            if (breakBlocks != null)
            {
                checkPadBlocks.Checked = true;
                checkPadAll.Checked = "all".Equals(breakBlocks.Value);
            }

            //Tabs / Indentation
            HaxeAStyleOption forceTabs = options.Find("--indent=force-tab");
            HaxeAStyleOption useTabs = options.Find("--indent=tab");
            HaxeAStyleOption useSpaces = options.Find("--indent=spaces");

            if (forceTabs != null)
            {
                checkTabs.Checked = true;
                checkForceTabs.Checked = true;
                numIndentWidth.Value = Convert.ToDecimal(forceTabs.Value);
            }
            else if (useTabs != null)
            {
                checkTabs.Checked = true;
                numIndentWidth.Value = Convert.ToDecimal(useTabs.Value);
            }
            else
            {
                checkTabs.Checked = false;
                numIndentWidth.Value = Convert.ToDecimal(useSpaces.Value);
            }
        }

        /// <summary>
        /// Helper method to create <see cref="HaxeAStyleOptions" /> from the currently selected
        /// options.
        /// </summary>
        /// <returns>An object of type <see cref="HaxeAStyleOptions" />, which is a list of <see cref="HaxeAStyleOption"/></returns>
        private HaxeAStyleOptions GetOptions()
        {
            HaxeAStyleOptions options = new HaxeAStyleOptions();

            HaxeAStyleHelper.AddDefaultOptions(options);

            //handling default switches
            foreach (TabPage page in this.tabControl.TabPages)
            {
                GetBasicTabOptions(options, page);
            }

            //special options

            //Tabs / Indentation
            if (IsChecked(checkForceTabs))
            {
                options.Add(new HaxeAStyleOption("--indent=force-tab", numIndentWidth.Value));
            }
            else if (IsChecked(checkTabs))
            {
                options.Add(new HaxeAStyleOption("--indent=tab", numIndentWidth.Value));
            }
            else
            {
                options.Add(new HaxeAStyleOption("--indent=spaces", numIndentWidth.Value));
            }

            //Brackets
            options.Add(new HaxeAStyleOption("--style", bracketStyles[(string)cbBracketStyle.SelectedItem]));
            
            
            if (IsChecked(checkAddBrackets))
            {
                if (IsChecked(checkOneLineBrackets))
                {
                    options.Add(new HaxeAStyleOption("--add-one-line-brackets"));
                }
                else
                {
                    options.Add(new HaxeAStyleOption("--add-brackets"));
                }
            }

            //Padding
            if (IsChecked(checkPadBlocks))
            {
                if (IsChecked(checkPadAll))
                {
                    options.Add(new HaxeAStyleOption("--break-blocks", "all"));
                }
                else
                {
                    options.Add(new HaxeAStyleOption("--break-blocks"));
                }
            }
            //options.Add("--indent-continuation=" + numIndentContinuation.Value); //not supported by old version of AStyle

            return options;
        }

        /// <summary>
        /// Helper method to apply the selected AStyle settings to the text
        /// </summary>
        private void ReformatExample()
        {
            txtExample.IsReadOnly = false;
            txtExample.Text = PluginCore.PluginBase.MainForm.CurrentDocument.SciControl.Text;
            txtExample.TabWidth = (int)numIndentWidth.Value;

            AStyleInterface astyle = new AStyleInterface();
            string[] options = GetOptions().ToStringArray();

            txtExample.IsFocus = true;
            int pos = txtExample.CurrentPos;
            txtExample.Text = astyle.FormatSource(txtExample.Text, String.Join(" ", options));
            txtExample.CurrentPos = pos;
            txtExample.ScrollCaret();
            txtExample.IsReadOnly = true;
        }

        /// <summary>
        /// Helper method to determine if <paramref name="chk"/> is enabled and checked.
        /// </summary>
        private bool IsChecked(CheckBox chk)
        {
            return chk.Checked && chk.Enabled;
        }

        /// <summary>
        /// Checks for incompatible selections and fixes them.
        /// For example, it makes no sense to delete and fill empty lines at the same time.
        /// </summary>
        /// <param name="sender">An optional argument to determin what control triggers the validation. Needed for some checks</param>
        private void ValidateControls(object sender = null)
        {
            //Bracket style
            string style = bracketStyles[(string)cbBracketStyle.SelectedValue];
            switch (style)
            {
                case "java":
                case "kr":
                case "stroustrup":
                case "linux":
                case "otbs":
                    checkBreakClosing.Enabled = true;
                    break;
                default: //style enables this by default
                    checkBreakClosing.Enabled = false;
                    break;
            }
            switch (style)
            {
                case "java":
                case "stroustrup":
                case "banner":
                case "google":
                case "lisp": //style enables this by default
                    checkAttachClasses.Enabled = false;
                    break;
                default:
                    checkAttachClasses.Enabled = true;
                    break;
            }
            checkRemoveBrackets.Enabled = style != "otbs";

            //Checkboxes
            checkKeepOneLineBlocks.Enabled = !checkOneLineBrackets.Checked;
            //These exclude each other:
            if (sender == checkFillEmptyLines && checkFillEmptyLines.Checked)
            {
                checkDeleteEmptyLines.Checked = false;
            }
            else if (checkDeleteEmptyLines.Checked)
            {
                checkFillEmptyLines.Checked = false;
            }

            if (sender == checkAddBrackets && checkAddBrackets.Checked)
            {
                checkRemoveBrackets.Checked = false;
            }
            else if (checkRemoveBrackets.Checked)
            {
                checkAddBrackets.Checked = false;
            }
            //
        }

        private void check_Click(object sender, EventArgs e)
        {
            ValidateControls(sender);

            ReformatExample();
        }

        private void numIndentWidth_ValueChanged(object sender, EventArgs e)
        {
            ReformatExample();
        }

        private void cbBracketStyle_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ValidateControls();

            ReformatExample();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
