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
        static private bool nameAsked;
        private IMainForm mainForm;

        public FlashDevelopActions(IMainForm mainForm)
        {
            this.mainForm = mainForm;
        }
        
        public Encoding GetDefaultEncoding()
        {
            return Encoding.GetEncoding((int)mainForm.Settings.DefaultCodePage);
        }

        public string GetDefaultEOLMarker()
        {
            return LineEndDetector.GetNewLineMarker((int)mainForm.Settings.EOLMode);
        }

        public static void CheckAuthorName()
        {
            if (nameAsked) return;
            nameAsked = true;
            foreach (Argument arg in PluginBase.MainForm.CustomArguments)
            {
                if (arg.Key == "DefaultUser" && arg.Value == "...")
                {
                    string caption = TextHelper.GetString("Title.AuthorName");
                    using (LineEntryDialog prompt = new LineEntryDialog(caption, "Author", ""))
                    {
                        if (prompt.ShowDialog() == DialogResult.OK)
                        {
                            arg.Value = prompt.Line;
                        }
                    }
                }
            }
        }

    }

}
