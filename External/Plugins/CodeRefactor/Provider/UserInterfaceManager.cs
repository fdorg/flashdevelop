// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Windows.Forms;
using CodeRefactor.Controls;
using PluginCore;

namespace CodeRefactor.Provider
{
    public static class UserInterfaceManager
    {
        private static ProgressDialog progressDialog;

        /// <summary>
        /// 
        /// </summary>
        private static Form Main => PluginBase.MainForm as Form;

        /// <summary>
        /// 
        /// </summary>
        public static ProgressDialog ProgressDialog
        {
            get
            {
                if (progressDialog is null)
                {
                    progressDialog = new ProgressDialog();
                    Main.AddOwnedForm(progressDialog);
                }
                return progressDialog;
            }
        }

    }

}