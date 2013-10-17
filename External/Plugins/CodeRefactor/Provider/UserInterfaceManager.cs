using System.Windows.Forms;
using CodeRefactor.Controls;
using PluginCore;

namespace CodeRefactor.Provider
{
    internal static class UserInterfaceManager
    {
        private static ProgressDialog progressDialog;
        private static RenameFileDialog renameFielDialog;

        /// <summary>
        /// 
        /// </summary>
        private static Form Main
        {
            get { return PluginBase.MainForm as Form; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static ProgressDialog ProgressDialog
        {
            get
            {
                if (progressDialog == null)
                {
                    progressDialog = new ProgressDialog();
                    Main.AddOwnedForm(progressDialog);
                }
                return progressDialog;
            }
        }

        internal static RenameFileDialog RenameFileDialog
        {
            get
            {
                if (renameFielDialog == null)
                {
                    renameFielDialog = new RenameFileDialog();
                    renameFielDialog.UpdateReferences.Checked = true;
                    renameFielDialog.NewName.Clear();
                    Main.AddOwnedForm(renameFielDialog);
                }
                return renameFielDialog;
            }
        }

    }

}