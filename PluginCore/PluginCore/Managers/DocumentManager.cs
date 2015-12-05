using System;
using System.IO;
using PluginCore.Localization;
using ScintillaNet;

namespace PluginCore.Managers
{
    public class DocumentManager
    {
        private static Int32 DocumentCount;
        
        static DocumentManager()
        {
            DocumentCount = 1;
        }

        /// <summary>
        /// Creates a new name for new document 
        /// </summary>
        public static String GetNewDocumentName(String extension)
        {
            if (String.IsNullOrEmpty(extension))
            {
                String setting = PluginBase.MainForm.Settings.DefaultFileExtension;
                if (setting.Trim() != String.Empty) extension = setting;
                else extension = "as";
            }
            Int32 count = DocumentCount++;
            if (!extension.StartsWith('.')) extension = "." + extension;
            String untitled = TextHelper.GetString("FlashDevelop.Info.UntitledFileStart");
            return untitled + count + extension;
        }

        /// <summary>
        /// Closes all open files inside the given path
        /// </summary>
        public static void CloseDocuments(String path)
        {
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.IsEditable)
                {
                    path = Path.GetFullPath(path);
                    Char separator = Path.DirectorySeparatorChar;
                    String filename = Path.GetFullPath(document.FileName);
                    if (filename == path || filename.StartsWithOrdinal(path + separator))
                    {
                        document.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Renames the found documents based on the specified path
        /// NOTE: Directory paths should be without the last separator
        /// </summary>
        public static void MoveDocuments(String oldPath, String newPath)
        {
            Boolean reactivate = false;
            oldPath = Path.GetFullPath(oldPath);
            newPath = Path.GetFullPath(newPath);
            ITabbedDocument current = PluginBase.MainForm.CurrentDocument;
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                /* We need to check for virtual models, another more generic option would be 
                 * Path.GetFileName(document.FileName).IndexOfAny(Path.GetInvalidFileNameChars()) == -1
                 * But this one is used in more places */
                if (document.IsEditable && !document.Text.StartsWithOrdinal("[model] "))
                {
                    String filename = Path.GetFullPath(document.FileName);
                    if (filename.StartsWithOrdinal(oldPath))
                    {
                        TextEvent ce = new TextEvent(EventType.FileClose, document.FileName);
                        EventManager.DispatchEvent(PluginBase.MainForm, ce);
                        document.SciControl.FileName = filename.Replace(oldPath, newPath);
                        TextEvent oe = new TextEvent(EventType.FileOpen, document.FileName);
                        EventManager.DispatchEvent(PluginBase.MainForm, oe);
                        if (current != document)
                        {
                            document.Activate();
                            reactivate = true;
                        }
                        else
                        {
                            TextEvent se = new TextEvent(EventType.FileSwitch, document.FileName);
                            EventManager.DispatchEvent(PluginBase.MainForm, se);
                        }
                    }
                    PluginBase.MainForm.ClearTemporaryFiles(filename);
                    document.RefreshTexts();
                }
            }
            PluginBase.MainForm.RefreshUI();
            if (reactivate) current.Activate();
        }

        /// <summary>
        /// Activates the document specified by document index
        /// </summary>
        public static void ActivateDocument(Int32 index)
        {
            if (index < PluginBase.MainForm.Documents.Length && index >= 0)
            {
                PluginBase.MainForm.Documents[index].Activate();
            }
            else if (PluginBase.MainForm.Documents.Length > 0)
            {
                PluginBase.MainForm.Documents[0].Activate();
            }
        }

        /// <summary>
        /// Finds the document by the file name
        /// </summary>
        public static ITabbedDocument FindDocument(String filename)
        {
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.IsEditable && document.FileName == filename)
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
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.IsEditable && document.SciControl == sci)
                {
                    return document;
                }
            }
            return null;
        }

    }

}
