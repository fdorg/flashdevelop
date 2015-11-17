using PluginCore;
using System.Windows.Forms;

namespace CodeRefactor.Controls
{
    class LineEntryDialog : ProjectManager.Helpers.LineEntryDialog
    {
        readonly Keys shortcutToLowercase;
        readonly Keys shortcutToUppercase;

        public LineEntryDialog(string captionText, string labelText, string defaultLine) : base(captionText, labelText, defaultLine)
        {
            shortcutToLowercase = PluginBase.MainForm.GetShortcutItemKeys("EditMenu.ToLowercase");
            shortcutToUppercase = PluginBase.MainForm.GetShortcutItemKeys("EditMenu.ToUppercase");
            lineBox.KeyDown += OnLineBoxOnKeyDown;
        }

        void OnLineBoxOnKeyDown(object sender, KeyEventArgs args)
        {
            string selectedText = lineBox.SelectedText;
            if (string.IsNullOrEmpty(selectedText)) return;
            Keys keys = args.KeyData;
            if (keys == shortcutToLowercase) selectedText = selectedText.ToLower();
            else if (keys == shortcutToUppercase) selectedText = selectedText.ToUpper();
            else return;
            int selectionStart = lineBox.SelectionStart;
            int selectionLength = lineBox.SelectionLength;
            lineBox.Paste(selectedText);
            SelectRange(selectionStart, selectionLength);
        }
    }
}