using System.Text;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Utilities;
using ProjectManager.Helpers;

namespace ProjectManager.Actions
{
    public class FlashDevelopActions
    {
        static bool nameAsked;
        readonly IMainForm mainForm;

        public FlashDevelopActions(IMainForm mainForm) => this.mainForm = mainForm;

        public Encoding GetDefaultEncoding() => Encoding.GetEncoding((int) mainForm.Settings.DefaultCodePage);

        public string GetDefaultEOLMarker() => LineEndDetector.GetNewLineMarker((int) mainForm.Settings.EOLMode);

        public static void CheckAuthorName()
        {
            if (nameAsked) return;
            nameAsked = true;
            foreach (var arg in PluginBase.MainForm.CustomArguments)
            {
                if (arg.Key == "DefaultUser" && arg.Value == "...")
                {
                    var caption = TextHelper.GetString("Title.AuthorName");
                    using var dialog = new LineEntryDialog(caption, "Author", "");
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        arg.Value = dialog.Line;
                    }
                }
            }
        }
    }
}