// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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