using System.IO;
using PluginCore.Localization;
using ScintillaNet;

namespace PluginCore.Managers
{
    public class DocumentManager
    {
        static int DocumentCount;
        
        static DocumentManager()
        {
            DocumentCount = 1;
        }

        /// <summary>
        /// Creates a new name for new document 
        /// </summary>
        public static string GetNewDocumentName(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                var setting = PluginBase.MainForm.Settings.DefaultFileExtension;
                extension = setting.Trim().Length > 0 ? setting : "as";
            }
            var count = DocumentCount++;
            if (!extension.StartsWith('.')) extension = "." + extension;
            var untitled = TextHelper.GetString("FlashDevelop.Info.UntitledFileStart");
            return untitled + count + extension;
        }

        /// <summary>
        /// Closes all open files inside the given path
        /// </summary>
        public static void CloseDocuments(string path)
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (!document.IsEditable) continue;
                path = Path.GetFullPath(path);
                var filename = Path.GetFullPath(document.FileName);
                if (filename == path || filename.StartsWithOrdinal(path + Path.DirectorySeparatorChar))
                {
                    document.Close();
                }
            }
        }

        /// <summary>
        /// Renames the found documents based on the specified path
        /// NOTE: Directory paths should be without the last separator
        /// </summary>
        public static void MoveDocuments(string oldPath, string newPath)
        {
            var reactivate = false;
            oldPath = Path.GetFullPath(oldPath);
            newPath = Path.GetFullPath(newPath);
            var current = PluginBase.MainForm.CurrentDocument;
            foreach (var document in PluginBase.MainForm.Documents)
            {
                /* We need to check for virtual models, another more generic option would be 
                 * Path.GetFileName(document.FileName).IndexOfAny(Path.GetInvalidFileNameChars()) == -1
                 * But this one is used in more places */
                if (!document.IsEditable || document.Text.StartsWithOrdinal("[model] ")) continue;
                var filename = Path.GetFullPath(document.FileName);
                if (filename.StartsWithOrdinal(oldPath))
                {
                    var ce = new TextEvent(EventType.FileClose, document.FileName);
                    EventManager.DispatchEvent(PluginBase.MainForm, ce);
                    document.SciControl.FileName = filename.Replace(oldPath, newPath);
                    var oe = new TextEvent(EventType.FileOpen, document.FileName);
                    EventManager.DispatchEvent(PluginBase.MainForm, oe);
                    if (current != document)
                    {
                        document.Activate();
                        reactivate = true;
                    }
                    else
                    {
                        var se = new TextEvent(EventType.FileSwitch, document.FileName);
                        EventManager.DispatchEvent(PluginBase.MainForm, se);
                    }
                }
                PluginBase.MainForm.ClearTemporaryFiles(filename);
                document.RefreshTexts();
            }
            PluginBase.MainForm.RefreshUI();
            if (reactivate) current.Activate();
        }

        /// <summary>
        /// Activates the document specified by document index
        /// </summary>
        public static void ActivateDocument(int index)
        {
            var documents = PluginBase.MainForm.Documents;
            if (index >= 0 && index < documents.Length)
            {
                documents[index].Activate();
            }
            else if (documents.Length > 0)
            {
                documents[0].Activate();
            }
        }

        /// <summary>
        /// Finds the document by the file name
        /// </summary>
        public static ITabbedDocument FindDocument(string filename)
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (document.FileName == filename)
                {
                    return document;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the document by the ScintillaControl
        /// </summary>
        public static ITabbedDocument FindDocument(ScintillaControl sci)
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (document.SciControl == sci)
                {
                    return document;
                }
            }
            return null;
        }
    }
}