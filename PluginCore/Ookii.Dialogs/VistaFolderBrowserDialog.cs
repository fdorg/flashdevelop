// Copyright © Sven Groot (Ookii.org) 2009
// BSD license; see license.txt for details.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using Ookii.Dialogs.Interop;

namespace Ookii.Dialogs
{
    /// <summary>
    /// Prompts the user to select a folder.
    /// </summary>
    /// <remarks>
    /// This class will use the Vista style Select Folder dialog if possible, or the regular FolderBrowserDialog
    /// if it is not. Note that the Vista style dialog is very different, so using this class without testing
    /// in both Vista and older Windows versions is not recommended.
    /// </remarks>
    /// <threadsafety instance="false" static="true" />
    [DefaultEvent("HelpRequest"), Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("SelectedPath"), Description("Prompts the user to select a folder.")]
    public sealed class VistaFolderBrowserDialog : CommonDialog
    {
        private FolderBrowserDialog _downlevelDialog;
        private string _description;
        private bool _useDescriptionForTitle;
        private string[] _selectedPaths;
        private System.Environment.SpecialFolder _rootFolder;
        private bool _multiselect;

        /// <summary>
        /// Occurs when the user clicks the Help button on the dialog box.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler HelpRequest
        {
            add
            {
                base.HelpRequest += value;
            }
            remove
            {
                base.HelpRequest -= value;
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="VistaFolderBrowserDialog" /> class.
        /// </summary>
        public VistaFolderBrowserDialog()
        {
            if( !IsVistaFolderDialogSupported )
                _downlevelDialog = new FolderBrowserDialog();
            else
                Reset();
        }

        #region Public Properties

        /// <summary>
        /// Gets a value that indicates whether the current OS supports Vista-style common file dialogs.
        /// </summary>
        /// <value>
        /// <see langword="true" /> on Windows Vista or newer operating systems; otherwise, <see langword="false" />.
        /// </value>
        [Browsable(false)]
        public static bool IsVistaFolderDialogSupported
        {
            get
            {
                return NativeMethods.IsWindowsVistaOrLater;
            }
        }

        /// <summary>
        /// Gets or sets the descriptive text displayed above the tree view control in the dialog box, or below the list view control
        /// in the Vista style dialog.
        /// </summary>
        /// <value>
        /// The description to display. The default is an empty string ("").
        /// </value>
        [Category("Folder Browsing"), DefaultValue(""), Localizable(true), Browsable(true), Description("The descriptive text displayed above the tree view control in the dialog box, or below the list view control in the Vista style dialog.")]
        public string Description
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.Description;
                return _description;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.Description = value;
                else
                    _description = value ?? String.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the root folder where the browsing starts from. This property has no effect if the Vista style
        /// dialog is used.
        /// </summary>
        /// <value>
        /// One of the <see cref="System.Environment.SpecialFolder" /> values. The default is Desktop.
        /// </value>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="System.Environment.SpecialFolder" /> values.</exception>
        [Localizable(false), Description("The root folder where the browsing starts from. This property has no effect if the Vista style dialog is used."), Category("Folder Browsing"), Browsable(true), DefaultValue(typeof(System.Environment.SpecialFolder), "Desktop")]
        public System.Environment.SpecialFolder RootFolder
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.RootFolder;
                return _rootFolder;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.RootFolder = value;
                else
                    _rootFolder = value;
            }
        }
	
        /// <summary>
        /// Gets or sets the path selected by the user.
        /// </summary>
        /// <value>
        /// The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an empty string ("").
        /// </value>
        [Browsable(true), Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Description("The path selected by the user."), DefaultValue(""), Localizable(true), Category("Folder Browsing")]
        public string SelectedPath
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.SelectedPath;
                return _selectedPaths[0];
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.SelectedPath = value;
                _selectedPaths = new string[1];
                _selectedPaths[0] = value ?? string.Empty;
            }
        }

        public string[] SelectedPaths
        {
            get
            {
                if (_downlevelDialog != null)
                    return new string[] {_downlevelDialog.SelectedPath };
                return SelectedPathsInternal;
            }
        }

        public bool Multiselect
        {
            get
            {
                if (_downlevelDialog != null)
                    return false;
                return _multiselect;
            }
            set
            {
                if (_downlevelDialog == null)
                    _multiselect = value;
            }
        }

        private bool _showNewFolderButton;

        /// <summary>
        /// Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box. This
        /// property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the New Folder button is shown in the dialog box; otherwise, <see langword="false" />. The default is <see langword="true" />.
        /// </value>
        [Browsable(true), Localizable(false), Description("A value indicating whether the New Folder button appears in the folder browser dialog box. This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown."), DefaultValue(true), Category("Folder Browsing")]
        public bool ShowNewFolderButton
        {
            get
            {
                if( _downlevelDialog != null )
                    return _downlevelDialog.ShowNewFolderButton;
                return _showNewFolderButton;
            }
            set
            {
                if( _downlevelDialog != null )
                    _downlevelDialog.ShowNewFolderButton = value;
                else
                    _showNewFolderButton = value;
            }
        }
	

        /// <summary>
        /// Gets or sets a value that indicates whether to use the value of the <see cref="Description" /> property
        /// as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
        /// </summary>
        /// <value><see langword="true" /> to indicate that the value of the <see cref="Description" /> property is used as dialog title; <see langword="false" />
        /// to indicate the value is added as additional text to the dialog. The default is <see langword="false" />.</value>
        [Category("Folder Browsing"), DefaultValue(false), Description("A value that indicates whether to use the value of the Description property as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.")]
        public bool UseDescriptionForTitle
        {
            get { return _useDescriptionForTitle; }
            set { _useDescriptionForTitle = value; }
        }	

        #endregion

        #region Internal Properties

        internal string[] SelectedPathsInternal
        {
            private get
            {
                if (_selectedPaths == null)
                {
                    return new string[0];
                }
                return (string[])_selectedPaths.Clone();
            }
            set
            {
                _selectedPaths = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            _description = string.Empty;
            _useDescriptionForTitle = false;
            _selectedPaths = new string[] { string.Empty };
            _rootFolder = Environment.SpecialFolder.Desktop;
            _showNewFolderButton = true;
            _multiselect = false;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Specifies a common dialog box.
        /// </summary>
        /// <param name="hwndOwner">A value that represents the window handle of the owner window for the common dialog box.</param>
        /// <returns><see langword="true" /> if the file could be opened; otherwise, <see langword="false" />.</returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            if( _downlevelDialog != null )
                return _downlevelDialog.ShowDialog(hwndOwner == IntPtr.Zero ? null : new WindowHandleWrapper(hwndOwner)) == DialogResult.OK;

            Ookii.Dialogs.Interop.IFileDialog dialog = null;
            try
            {
                dialog = new Ookii.Dialogs.Interop.NativeFileOpenDialog();
                SetDialogProperties(dialog);
                int result = dialog.Show(hwndOwner);
                if( result < 0 )
                {
                    if( (uint)result == (uint)HRESULT.ERROR_CANCELLED )
                        return false;
                    else
                        throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(result);
                } 
                GetResult(dialog);
                return true;
            }
            finally
            {
                if( dialog != null )
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(dialog);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="VistaFolderBrowserDialog" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if( disposing && _downlevelDialog != null )
                    _downlevelDialog.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion

        #region Private Methods

        private void SetDialogProperties(Ookii.Dialogs.Interop.IFileDialog dialog)
        {
            // Description
            if( !string.IsNullOrEmpty(_description) )
            {
                if( _useDescriptionForTitle )
                {
                    dialog.SetTitle(_description);
                }
                else
                {
                    Ookii.Dialogs.Interop.IFileDialogCustomize customize = (Ookii.Dialogs.Interop.IFileDialogCustomize)dialog;
                    customize.AddText(0, _description);
                }
            }

            NativeMethods.FOS options = NativeMethods.FOS.FOS_PICKFOLDERS | NativeMethods.FOS.FOS_FORCEFILESYSTEM | NativeMethods.FOS.FOS_FILEMUSTEXIST;
            if (Multiselect)
                options |= NativeMethods.FOS.FOS_ALLOWMULTISELECT;
            dialog.SetOptions(options);

            if( !string.IsNullOrEmpty(_selectedPaths[0]) )
            {
                string parent = Path.GetDirectoryName(_selectedPaths[0]);
                if( parent == null || !Directory.Exists(parent) )
                {
                    dialog.SetFileName(_selectedPaths[0]);
                }
                else
                {
                    string folder = Path.GetFileName(_selectedPaths[0]);
                    dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
                    dialog.SetFileName(folder);
                }
            }
        }

        private void GetResult(Ookii.Dialogs.Interop.IFileDialog dialog)
        {
            if (Multiselect)
            {
                Ookii.Dialogs.Interop.IShellItemArray results;
                ((Ookii.Dialogs.Interop.IFileOpenDialog)dialog).GetResults(out results);
                uint count;
                results.GetCount(out count);
                string[] selectedPaths = new string[count];
                for (uint x = 0; x < count; ++x)
                {
                    Ookii.Dialogs.Interop.IShellItem item;
                    results.GetItemAt(x, out item);
                    string name;
                    item.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out name);
                    selectedPaths[x] = name;
                }
                _selectedPaths = selectedPaths;

            }
            else
            {
                Ookii.Dialogs.Interop.IShellItem item;
                dialog.GetResult(out item);
                item.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out _selectedPaths[0]);
            }
        }

        #endregion
    }
}
