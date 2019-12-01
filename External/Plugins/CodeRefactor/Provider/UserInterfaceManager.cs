using System.Windows.Forms;
using CodeRefactor.Controls;
using PluginCore;

namespace CodeRefactor.Provider
{
    public static class UserInterfaceManager
    {
        static ProgressDialog progressDialog;

        public static ProgressDialog ProgressDialog
        {
            get
            {
                if (progressDialog is null)
                {
                    progressDialog = new ProgressDialog();
                    ((Form) PluginBase.MainForm).AddOwnedForm(progressDialog);
                }
                return progressDialog;
            }
        }
    }
}