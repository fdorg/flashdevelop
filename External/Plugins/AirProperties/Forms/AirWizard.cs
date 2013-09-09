using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Xml.XPath;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;

namespace AirProperties
{
    public partial class AirWizard : Form
    {
        #region Validation Regular Expressions

        // Validation regex pattens based on / adapted from validation requirements in the Descriptor.xsd files from the AIR SDK.
        private const string _IdRegexPattern = @"^[A-Za-z0-9\-\.]{1,212}$";
        private const string _FileNameRegexPattern = @"^[^\*""/:&<>\?\\\|\. ]$|^[^\*""/:&<>\?\\\| ][^\*""/:&<>\?\\\|]*[^\*""/:<>\?\\\|\. ]$";
        // Minor tweak to FilePathRegexPattern to prevent forward slash (/) as last character (causes packaging to fail with error). 
        private const string _FilePathRegexPattern = @"^[^\*""/:&<>\?\\\|\. ]$|^[^\*""/:&<>\?\\\| ][^\*"":&<>\?\\\|]*[^\*""/:<>\?\\\|\. ]$";
        // There is no universal standard for AnyUriRegexPattern but this pattern is a good fit for image and content 
        // URIs used in the descriptor file, as these URIs are relative paths.
        private const string _AnyUriRegexPattern = @"^[^\*"":&<>\?\\\|\. ]$|^[^\*"":&<>\?\\\| ][^\*"":&<>\?\\\|]*[^\*""/:<>\?\\\|\. ]$";
        private const string _CoordinateRegexPattern = @"^(-)?\d+$";
        private const string _NumberRegexPattern = @"^[0-9]*$";
        private const string _PublisherRegexPattern = @"^[A-Fa-f0-9]{40}\.1$";
        private const string _VersionRegexPattern = @"^[0-9]{1,3}(\.[0-9]{1,3}){0,2}$";
        private const string _ExtensionRegexPattern = @"^[A-Za-z0-9\-\.]{1,212}$";

        #endregion
               
        private PluginMain _pluginMain;
        private String _propertiesFilePath;
        private String _propertiesFile;
        private Boolean _isPropertiesLoaded;
        private List<string> _locales = new List<string>();
        private List<PropertyManager.AirFileType> _fileTypes = new List<PropertyManager.AirFileType>();
        private readonly List<PropertyManager.AirApplicationIconField> _iconFields;
        private List<PropertyManager.AirExtension> _extensions = new List<PropertyManager.AirExtension>();
                
        public Boolean IsPropertiesLoaded
        {
            get { return _isPropertiesLoaded; }
        }

        public AirWizard(PluginMain pluginMain)
        {
            _pluginMain = pluginMain;
            InitializeComponent();
            InitializeControls();
            InitializeGraphics();
            InitializeLocalization();
            this.Font = PluginCore.PluginBase.MainForm.Settings.DefaultFont;
            this.TitleHeading.Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold);
            _iconFields = new List<PropertyManager.AirApplicationIconField> 
            { 
                new PropertyManager.AirApplicationIconField("16", PropertyManager.AirVersion.V10),
                new PropertyManager.AirApplicationIconField("29", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("32", PropertyManager.AirVersion.V10),
                new PropertyManager.AirApplicationIconField("36", PropertyManager.AirVersion.V25),
                new PropertyManager.AirApplicationIconField("48", PropertyManager.AirVersion.V10),
                new PropertyManager.AirApplicationIconField("50", PropertyManager.AirVersion.V34),
                new PropertyManager.AirApplicationIconField("57", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("58", PropertyManager.AirVersion.V34),
                new PropertyManager.AirApplicationIconField("72", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("100", PropertyManager.AirVersion.V34),
                new PropertyManager.AirApplicationIconField("114", PropertyManager.AirVersion.V26),
                new PropertyManager.AirApplicationIconField("128", PropertyManager.AirVersion.V10),
                new PropertyManager.AirApplicationIconField("144", PropertyManager.AirVersion.V33),
                new PropertyManager.AirApplicationIconField("512", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("1024", PropertyManager.AirVersion.V34)
            };
            if (this._pluginMain.Settings.SelectPropertiesFileOnOpen) SelectAndLoadPropertiesFile();
            else
            {
                _propertiesFilePath = Path.GetDirectoryName(PluginCore.PluginBase.CurrentProject.ProjectPath);
                LoadProperties(true);
            }
            String airVersion = PropertyManager.Version;
            if (PropertyManager.IsUnsupportedVersion) airVersion += "*";
            SetTitle(PluginCore.PluginBase.CurrentProject.Name, airVersion);
        }

        private void InitializeLocalization()
        {
            // Common
            this.OKButton.Text = TextHelper.GetString("Label.Ok");
            this.HelpButton1.Text = TextHelper.GetString("Label.Help");
            this.CancelButton1.Text = TextHelper.GetString("Label.Cancel");
            this.TitleHeading.Text = TextHelper.GetString("Title.AirWizard");
            this.TitleDescription.Text = TextHelper.GetString("Label.TitleDesc").Replace("{0}", "\r\n");
            // Details
            this.DetailsTabPage.Text = TextHelper.GetString("Label.Details");
            this.DescriptionLabel.Text = TextHelper.GetString("Label.Description");
            this.VersionNoLabel.Text = TextHelper.GetString("Label.VersionNo");
            this.CopyrightLabel.Text = TextHelper.GetString("Label.Copyright");
            this.VersionLabel.Text = TextHelper.GetString("Label.Version");
            this.LocaleLabel.Text = TextHelper.GetString("Label.Locale");
            this.NameLabel.Text = TextHelper.GetString("Label.Name");
            this.IDLabel.Text = TextHelper.GetString("Label.Id");
            // Installation
            this.FileNameLabel.Text = TextHelper.GetString("Label.FileName");
            this.PublisherIdLabel.Text = TextHelper.GetString("Label.PublisherId");
            this.InstallationTabPage.Text = TextHelper.GetString("Label.Installation");
            this.MinPatchLevelLabel.Text = TextHelper.GetString("Label.MinPatchLevel");
            this.InstallFolderLabel.Text = TextHelper.GetString("Label.InstallFolder");
            this.ProgramMenuFolderLabel.Text = TextHelper.GetString("Label.ProgramMenuFolder");
            this.SupportedProfilesGroupBox.Text = TextHelper.GetString("Label.SupportedProfiles");
            this.ExtendedDesktopLabel.Text = TextHelper.GetString("Label.ExtendedDesktop");
            this.MobileDeviceLabel.Text = TextHelper.GetString("Label.MobileDevice");
            this.ExtendedTvLabel.Text = TextHelper.GetString("Label.ExtendedTv");
            this.DesktopLabel.Text = TextHelper.GetString("Label.Desktop");
            this.TvLabel.Text = TextHelper.GetString("Label.Tv");
            // Application
            this.ApplicationTabPage.Text = TextHelper.GetString("Label.Application");
            this.CustomUpdateUILabel.Text = TextHelper.GetString("Label.CustomUpdateUI");
            this.BrowersInvocationLabel.Text = TextHelper.GetString("Label.BrowserInvocation");
            this.AppIconsGroupBox.Text = TextHelper.GetString("Label.Icons");
            // Initial Window
            this.TitleLabel.Text = TextHelper.GetString("Label.Title");
            this.ContentLabel.Text = TextHelper.GetString("Label.Content");
            this.VisibleLabel.Text = TextHelper.GetString("Label.Visible");
            this.ResizableLabel.Text = TextHelper.GetString("Label.Resizable");
            this.TransparentLabel.Text = TextHelper.GetString("Label.Transparent");
            this.MinimizableLabel.Text = TextHelper.GetString("Label.Minimizable");
            this.MaximizableLabel.Text = TextHelper.GetString("Label.Maximizable");
            this.WindowBoundsGroupBox.Text = TextHelper.GetString("Label.WindowBounds");
            this.InitialLocationLabel.Text = TextHelper.GetString("Label.InitialLocation");
            this.WindowedPlatformsTabPage.Text = TextHelper.GetString("Label.WindowedPlatforms");
            this.NonWindowedPlatformsTabPage.Text = TextHelper.GetString("Label.NonWindowedPlatforms");
            this.SystemChromeLabel.Text = TextHelper.GetString("Label.SystemChrome");
            this.InitialSizeLabel.Text = TextHelper.GetString("Label.InitialSize");
            this.MinimumSizeLabel.Text = TextHelper.GetString("Label.MinimumSize");
            this.MaximumSizeLabel.Text = TextHelper.GetString("Label.MaximumSize");
            this.ApectRatioLabel.Text = TextHelper.GetString("Label.AspectRatio");
            this.RenderModeLabel.Text = TextHelper.GetString("Label.RenderMode");
            this.AutoOrientsLabel.Text = TextHelper.GetString("Label.AutoOrients");
            this.FullscreenLabel.Text = TextHelper.GetString("Label.Fullscreen");
            // Filetypes
            this.FileTypesTabPage.Text = TextHelper.GetString("Label.FileTypes");
            this.FileTypeNameColumnHeader.Text = TextHelper.GetString("Label.Name");
            this.FileTypeExtensionColumnHeader.Text = TextHelper.GetString("Label.Extension");
            this.FileTypeDetailsTabPage.Text = TextHelper.GetString("Label.Details");
            this.FTExtensionLabel.Text = TextHelper.GetString("Label.Extension");
            this.FTDescriptionLabel.Text = TextHelper.GetString("Label.Description");
            this.FTContentTypeLabel.Text = TextHelper.GetString("Label.ContentType");
            this.RemoveFileTypeButton.Text = TextHelper.GetString("Label.Remove");
            this.NewFileTypeButton.Text = TextHelper.GetString("Label.New");
            this.FTNameLabel.Text = TextHelper.GetString("Label.Name");
            // Extensions
            this.ExtensionsTabPage.Text = TextHelper.GetString("Label.Extensions");
            this.ExtensionIdLabel.Text = TextHelper.GetString("Label.ExtensionId");
            this.ExtensionsColumnHeader.Text = TextHelper.GetString("Label.Extensions");
            this.ExtensionRemoveButton.Text = TextHelper.GetString("Label.Remove");
            this.ExtensionAddButton.Text = TextHelper.GetString("Label.Add");
            // Mobile additions
            this.AndroidManifestAdditionsTabPage.Text = TextHelper.GetString("Label.AndroidAdditions");
            this.IPhoneInfoAdditionsTabPage.Text = TextHelper.GetString("Label.IPhoneAdditions");
        }

        private void InitializeGraphics()
        {
            this.TitlePictureBox.Image = PluginMain.GetImage("blockdevice.png");
            this.NamePictureBox.Image = PluginMain.GetImage("irc_protocol.png");
            this.DescriptionPictureBox.Image = PluginMain.GetImage("irc_protocol.png");
            this.LocalePictureBox.Image = PluginMain.GetImage("irc_protocol.png");
        }

        private void InitializeControls()
        {
            SystemChromeField.Items.Add(new ListItem(TextHelper.GetString("SystemChrome.None"), "none"));
            SystemChromeField.Items.Add(new ListItem(TextHelper.GetString("SystemChrome.Standard"), "standard"));
            AspectRatioField.Items.Add(new ListItem(String.Empty, String.Empty));
            AspectRatioField.Items.Add(new ListItem(TextHelper.GetString("AspectRatio.Portrait"), "portrait"));
            AspectRatioField.Items.Add(new ListItem(TextHelper.GetString("AspectRatio.Landscape"), "landscape"));
            RenderModeField.Items.Add(new ListItem(String.Empty, String.Empty));
            RenderModeField.Items.Add(new ListItem(TextHelper.GetString("RenderMode.Auto"), "auto"));
            RenderModeField.Items.Add(new ListItem(TextHelper.GetString("RenderMode.CPU"), "cpu"));
            RenderModeField.Items.Add(new ListItem(TextHelper.GetString("RenderMode.GPU"), "gpu"));
            RenderModeField.Items.Add(new ListItem(TextHelper.GetString("RenderMode.Direct"), "direct"));
            OpenIconFileDialog.InitialDirectory = Path.GetDirectoryName(PluginCore.PluginBase.CurrentProject.ProjectPath);
            OpenPropertiesFileDialog.InitialDirectory = Path.GetDirectoryName(PluginCore.PluginBase.CurrentProject.ProjectPath);
            AndroidManifestAdditionsField.SelectionTabs = new int[] { 25, 50, 75, 100, 125, 150, 175, 200 };
            IPhoneInfoAdditionsField.SelectionTabs = new int[] { 25, 50, 75, 100, 125, 150, 175, 200 };     
        }

        private void InitializeLocales()
        {
            LocalesField.Items.Clear();
            if (_locales.Count > 0)
            {
                LocalesField.Items.AddRange(_locales.ToArray());
                NamePictureBox.Visible = true;
                DescriptionPictureBox.Visible = true;
            }
            else
            {
                NamePictureBox.Visible = false;
                DescriptionPictureBox.Visible = false;
                LocalesField.Items.Add(TextHelper.GetString("Locale.Default"));
            }
            LocalesField.SelectedIndex = 0;
            // If AIR 1.0 then disable locale support
            if (PropertyManager.MajorVersion == PropertyManager.AirVersion.V10)
            {
                LocalesField.Enabled = false;
                LocaleManagerButton.Enabled = false;
                LocalePictureBox.Visible = false;
                NamePictureBox.Visible = false;
                DescriptionPictureBox.Visible = false;
            }
        }

        private String GetSelectedLocale()
        {
            String item = (String)LocalesField.SelectedItem;
            if (item != null)
            {
                if (item == TextHelper.GetString("Locale.Default")) return String.Empty;
                else return item;
            }
            else return String.Empty;
        }

        private Boolean GetSelectedLocaleIsDefault()
        {
            return (Boolean)(LocalesField.SelectedIndex == 0);
        }

        public void SetTitle(string projectName, string airVersion)
        {
            this.Text = " " + projectName + " - " + TextHelper.GetString("Title.AirWizard") + " (" + airVersion + ")";
        }

        private void SetupUI()
        {
            // Remove unsupported properties
            if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V32)
            {
                SupportedLanguagesLabel.Visible = false;
                SupportedLanguagesField.Visible = false;
                SupportedLanguagesButton.Visible = false;
            }
            if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V25)
            {
                VersionPanel25.Visible = false;
                VersionPanel.Visible = true;
                TvField.Visible = false;
                TvLabel.Visible = false;
                ExtendedTvField.Visible = false;
                ExtendedTvLabel.Visible = false;
                // Remove 36x36
                FileTypeIconsPanel.RemoveRow(3);
                AppPropertiesTabControl.TabPages.Remove(ExtensionsTabPage);
                MobileAdditionsTabControl.TabPages.Remove(AndroidManifestAdditionsTabPage);
            }
            if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V20)
            {
                SupportedProfilesGroupBox.Visible = false;
                // Remove 29x29, 72x72, 512x512
                // Each call reduces existing row numbers, hence multiple calls to same row
                InitialWindowTabControl.TabPages.Remove(NonWindowedPlatformsTabPage);
                FileTypeIconsPanel.RemoveRow(1);
                FileTypeIconsPanel.RemoveRow(3);
                FileTypeIconsPanel.RemoveRow(4);
                AppPropertiesTabControl.TabPages.Remove(MobileAdditionsTabPage);
            }
            if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V153)
            {
                PublisherIdLabel.Visible = false;
                PublisherIdField.Visible = false;
            }

            // Add app icons fields
            int i = 0;
            foreach (PropertyManager.AirApplicationIconField icon in _iconFields)
            {
                if (icon.MinVersion <= PropertyManager.MajorVersion)
                {
                    Label appIconLabel = new Label();
                    TextBox appIconField = new TextBox();
                    Button appIconButton = new Button();

                    icon.Field = appIconField;

                    AppIconsPanel.Controls.Add(appIconLabel, 0, i);
                    AppIconsPanel.Controls.Add(appIconField, 1, i);
                    AppIconsPanel.Controls.Add(appIconButton, 2, i++);

                    appIconLabel.Anchor = AnchorStyles.Left;
                    appIconLabel.AutoSize = true;
                    appIconLabel.ImeMode = ImeMode.NoControl;
                    appIconLabel.Location = new Point(3, 6);
                    appIconLabel.Size = new Size(43, 13);
                    appIconLabel.TabIndex = 0;
                    appIconLabel.Text = String.Format("{0} x {0}", icon.Size);

                    appIconField.Anchor = AnchorStyles.Left;
                    ValidationErrorProvider.SetIconPadding(appIconField, 36);
                    appIconField.Location = new Point(74, 4);
                    appIconField.Margin = new Padding(3, 3, 3, 0);
                    appIconField.Name = "AppIconField" + icon.Size;
                    appIconField.Size = new Size(343, 21);
                    appIconField.TabIndex = 1;
                    appIconField.Validating += ValidateAppIconField;

                    appIconButton.Anchor = AnchorStyles.Left;
                    appIconButton.ImeMode = ImeMode.NoControl;
                    appIconButton.Location = new Point(423, 3);
                    appIconButton.Margin = new Padding(3, 3, 3, 0);
                    appIconButton.Name = "AppIconButton" + icon.Size;
                    appIconButton.Size = new Size(29, 23);
                    appIconButton.TabIndex = 2;
                    appIconButton.Text = "...";
                    appIconButton.UseVisualStyleBackColor = true;
                    appIconButton.Click += AppIconButtonClick;

                    AppIconsPanel.RowStyles.Add(new RowStyle());
        }
            }
        }

        private void LoadProperties(Boolean isDefault)
        {
            string contentProperty;
            string[] minSizeProperty;
            string[] maxSizeProperty;
            _isPropertiesLoaded = false;

            if (isDefault)
            {
                // search app descriptor in project
                _propertiesFile = LookupPropertiesFile();
            }

            if (File.Exists(_propertiesFile))
            {
                if (PropertyManager.InitializeProperties(_propertiesFile))
                {
                    if (PropertyManager.IsUnsupportedVersion)
                    {
                        if (MessageBox.Show(String.Format(TextHelper.GetString("Alert.Message.UnsupportedAirDescriptorFile"),
                            _propertiesFile, PropertyManager.Version, PropertyManager.MaxSupportedVersion),
                            TextHelper.GetString("Alert.Title.UnsupportedAirDescriptorFile"),
                            MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) != System.Windows.Forms.DialogResult.OK)
                            return;
                    }
                    SetupUI();
                    // Build locales list from relevant properties
                    PropertyManager.GetPropertyLocales("name", _locales);
                    PropertyManager.GetPropertyLocales("description", _locales);
                    InitializeLocales();
                    // Details tab
                    PropertyManager.GetProperty("id", IDField);
                    PropertyManager.GetProperty("name", NameField, GetSelectedLocale());
                    PropertyManager.GetProperty("description", DescriptionField, GetSelectedLocale());
                    PropertyManager.GetProperty("copyright", CopyrightField);
                    if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V25)
                    {
                        PropertyManager.GetProperty("version", VersionField);
                    }
                    else
                    {
                        PropertyManager.GetProperty("versionLabel", VersionLabelField);
                        PropertyManager.GetProperty("versionNumber", VersionNoField);
                    }
                    // Installation tab
                    PropertyManager.GetAttribute("minimumPatchLevel", MinimumPatchLevelField);
                    PropertyManager.GetProperty("filename", FileNameField);
                    PropertyManager.GetProperty("installFolder", InstallFolderField);
                    PropertyManager.GetProperty("programMenuFolder", ProgramMenuFolderField);
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V153)
                    {
                        PropertyManager.GetProperty("publisherID", PublisherIdField);
                    }
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                    {
                        foreach (string val in PropertyManager.GetProperty("supportedProfiles").Split(' '))
                        {
                            if (val == "desktop") DesktopField.CheckState = CheckState.Checked;
                            else if (val == "extendedDesktop") ExtendedDesktopField.CheckState = CheckState.Checked;
                            else if (val == "mobileDevice") MobileDeviceField.CheckState = CheckState.Checked;
                            else if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25 && val == "tv")
                            {
                                TvField.CheckState = CheckState.Checked;
                            }
                            else if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25 && val == "extendedTV")
                            {
                                ExtendedTvField.CheckState = CheckState.Checked;
                            }
                        }
                    }
                    // Application tab
                    PropertyManager.GetProperty("allowBrowserInvocation", BrowserInvocationField);
                    PropertyManager.GetProperty("customUpdateUI", CustomUpdateUIField);
                    foreach (PropertyManager.AirApplicationIconField icon in this._iconFields)
                    {
                        if (icon.MinVersion <= PropertyManager.MajorVersion)
                        {
                            PropertyManager.GetProperty("icon/image"+icon.Size+"x"+icon.Size, icon.Field);
                        }
                    }                    
                    // Initial Window tab
                    contentProperty = PropertyManager.GetProperty("initialWindow/content");
                    ContentField.Text = System.Web.HttpUtility.UrlDecode(contentProperty);
                    PropertyManager.GetProperty("initialWindow/title", TitleField);
                    PropertyManager.GetProperty("initialWindow/systemChrome", SystemChromeField, 1);
                    PropertyManager.GetProperty("initialWindow/transparent", TransparentField);
                    PropertyManager.GetProperty("initialWindow/visible", VisibleField);
                    PropertyManager.GetProperty("initialWindow/resizable", ResizableField);
                    PropertyManager.GetProperty("initialWindow/minimizable", MinimizableField);
                    PropertyManager.GetProperty("initialWindow/maximizable", MaximizableField);
                    PropertyManager.GetProperty("initialWindow/x", XField);
                    PropertyManager.GetProperty("initialWindow/y", YField);
                    PropertyManager.GetProperty("initialWindow/width", WidthField);
                    PropertyManager.GetProperty("initialWindow/height", HeightField);
                    minSizeProperty = PropertyManager.GetProperty("initialWindow/minSize").Split(' ');
                    if (minSizeProperty.Length == 2)
                    {
                        MinSizeXField.Text = minSizeProperty[0];
                        MinSizeYField.Text = minSizeProperty[1];
                    }
                    maxSizeProperty = PropertyManager.GetProperty("initialWindow/maxSize").Split(' ');
                    if (maxSizeProperty.Length == 2)
                    {
                        MaxSizeXField.Text = maxSizeProperty[0];
                        MaxSizeYField.Text = maxSizeProperty[1];
                    }
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                    {
                        PropertyManager.GetProperty("initialWindow/aspectRatio", AspectRatioField, 0);
                        PropertyManager.GetProperty("initialWindow/renderMode", RenderModeField, 0);
                        PropertyManager.GetProperty("initialWindow/autoOrients", AutoOrientsField);
                        PropertyManager.GetProperty("initialWindow/fullScreen", FullScreenField);
                    }
                    // File Types tab
                    PropertyManager.GetFileTypes(_fileTypes);
                    ValidateFileTypes();
                    InitializeFileTypesListView();
                    // Extensions tab
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                    {
                        PropertyManager.GetExtensions(_extensions);
                        ValidateExtensions();
                        InitializeExtensionsListView();
                    }
                    // Mobile Additions tab
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                    {
                        String iPhoneInfoAdditions = PropertyManager.GetProperty("iPhone/InfoAdditions");
                        IPhoneInfoAdditionsField.Text = FormatXML(iPhoneInfoAdditions, String.Empty);
                    }
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                    {
                        String androidManifestAdditions = PropertyManager.GetProperty("android/manifestAdditions");
                        AndroidManifestAdditionsField.Text = FormatXML(androidManifestAdditions, "android");
                    }
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V32)
                    {
                        PropertyManager.GetProperty("supportedLanguages", SupportedLanguagesField);
                    }
                    //Validate all controls so the error provider activates on any invalid values
                    ValidateChildren();
                    _isPropertiesLoaded = true;
                }
                else
                {
                    MessageBox.Show(PropertyManager.LastException.Message, TextHelper.GetString("Exception.Title.Initialization"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                if (isDefault && MessageBox.Show(TextHelper.GetString("Alert.Message.AppDescriptorNotFound"), TextHelper.GetString("Alert.Title.AppDescriptorNotFound"), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    SelectAndLoadPropertiesFile();
                }
            }
        }

        private String LookupPropertiesFile()
        {
            String path = Path.Combine(_propertiesFilePath, "application.xml");
            if (File.Exists(path)) return path;
            String[] files = Directory.GetFiles(_propertiesFilePath, "*-app.xml");
            foreach (String file in files)
                if (LooksLikeAIRProperties(file)) return file;
            files = Directory.GetFiles(_propertiesFilePath, "*.xml");
            foreach (String file in files)
                if (LooksLikeAIRProperties(file)) return file;

            // look if we stored a custom path in the project's lcoal storage
            ProjectManager.Projects.Project project = PluginCore.PluginBase.CurrentProject as ProjectManager.Projects.Project;
            if (project.Storage.ContainsKey("air-descriptor"))
                path = project.GetAbsolutePath(project.Storage["air-descriptor"]);

            return path; // not found
        }

        private bool LooksLikeAIRProperties(string file)
        {
            try
            {
                String src = File.ReadAllText(file);
                if (src.IndexOf("xmlns=\"http://ns.adobe.com/air/") > 0) return true;
            }
            catch { }
            return false;
        }
                
        private void SaveProperties()
        {
            string supportedProfilesProperty = String.Empty;
            if (File.Exists(_propertiesFile))
            {
                // Details tab
                PropertyManager.SetProperty("id", IDField);
                PropertyManager.SetProperty("name", NameField, GetSelectedLocale(), GetSelectedLocaleIsDefault());
                PropertyManager.SetProperty("description", DescriptionField, GetSelectedLocale(), GetSelectedLocaleIsDefault());
                PropertyManager.SetProperty("copyright", CopyrightField);
                if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V25)
                {
                    PropertyManager.SetProperty("version", VersionField);
                }
                else
                {
                    PropertyManager.SetProperty("versionLabel", VersionLabelField);
                    PropertyManager.SetProperty("versionNumber", VersionNoField);
                    // will cause any existing version property to be removed
                    PropertyManager.SetProperty("version", String.Empty);
                }
                // Installation tab
                PropertyManager.SetAttribute("minimumPatchLevel", MinimumPatchLevelField);
                PropertyManager.SetProperty("filename", FileNameField);
                PropertyManager.SetProperty("installFolder", InstallFolderField);
                PropertyManager.SetProperty("programMenuFolder", ProgramMenuFolderField);
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V153)
                {
                    PropertyManager.SetProperty("publisherID", PublisherIdField);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                {
                    if (DesktopField.Checked) supportedProfilesProperty += "desktop";
                    if (ExtendedDesktopField.Checked)
                    {
                        if (supportedProfilesProperty.Length > 0) supportedProfilesProperty += " ";
                        supportedProfilesProperty += "extendedDesktop";
                    }
                    if (MobileDeviceField.Checked)
                    {
                        if (supportedProfilesProperty.Length > 0) supportedProfilesProperty += " ";
                        supportedProfilesProperty += "mobileDevice";
                    }
                    if (TvField.Checked)
                    {
                        if (supportedProfilesProperty.Length > 0) supportedProfilesProperty += " ";
                        supportedProfilesProperty += "tv";
                    }
                    if (ExtendedTvField.Checked)
                    {
                        if (supportedProfilesProperty.Length > 0) supportedProfilesProperty += " ";
                        supportedProfilesProperty += "extendedTV";
                    }
                    PropertyManager.SetProperty("supportedProfiles", supportedProfilesProperty);
                }
                // Application tab
                PropertyManager.SetProperty("allowBrowserInvocation", BrowserInvocationField);
                PropertyManager.SetProperty("customUpdateUI", CustomUpdateUIField);
                foreach (PropertyManager.AirApplicationIconField icon in this._iconFields)
                {
                    if (icon.MinVersion <= PropertyManager.MajorVersion)
                    {
                        PropertyManager.SetProperty("icon/image" + icon.Size + "x" + icon.Size, icon.Field);
                    }
                }
                // Initial Window tab
                PropertyManager.SetProperty("initialWindow/content", System.Web.HttpUtility.UrlPathEncode(ContentField.Text));
                PropertyManager.SetProperty("initialWindow/title", TitleField);
                PropertyManager.SetProperty("initialWindow/systemChrome", SystemChromeField);
                PropertyManager.SetProperty("initialWindow/transparent", TransparentField);
                PropertyManager.SetProperty("initialWindow/visible", VisibleField);
                PropertyManager.SetProperty("initialWindow/resizable", ResizableField);
                PropertyManager.SetProperty("initialWindow/minimizable", MinimizableField);
                PropertyManager.SetProperty("initialWindow/maximizable", MaximizableField);
                PropertyManager.SetProperty("initialWindow/x", XField);
                PropertyManager.SetProperty("initialWindow/y", YField);
                PropertyManager.SetProperty("initialWindow/width", WidthField);
                PropertyManager.SetProperty("initialWindow/height", HeightField);
                PropertyManager.SetProperty("initialWindow/minSize", (MinSizeXField.Text + " " + MinSizeYField.Text).Trim());
                PropertyManager.SetProperty("initialWindow/maxSize", (MaxSizeXField.Text + " " + MaxSizeYField.Text).Trim());
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                {
                    PropertyManager.SetProperty("initialWindow/aspectRatio", AspectRatioField);
                    PropertyManager.SetProperty("initialWindow/renderMode", RenderModeField);
                    PropertyManager.SetProperty("initialWindow/autoOrients", AutoOrientsField);
                    PropertyManager.SetProperty("initialWindow/fullScreen", FullScreenField);
                }
                // File Types tab
                PropertyManager.SetFileTypes(_fileTypes);
                // Extensions tab
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                {
                    PropertyManager.SetExtensions(_extensions);
                }
                // Mobile Additions tab
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                {
                    PropertyManager.SetPropertyCData("iPhone/InfoAdditions", IPhoneInfoAdditionsField);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                {
                    PropertyManager.SetPropertyCData("android/manifestAdditions", AndroidManifestAdditionsField);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V32)
                {
                    PropertyManager.SetProperty("supportedLanguages", SupportedLanguagesField);
                }
                PropertyManager.CommitProperties(_propertiesFile);
            }
        }

        private void SelectAndLoadPropertiesFile()
        {
            if (OpenPropertiesFileDialog.ShowDialog() == DialogResult.OK)
            {
                _propertiesFile = OpenPropertiesFileDialog.FileName;
                LoadProperties(false);

                if (_isPropertiesLoaded)
                {
                    // store custom property file location in project's local storage
                    ProjectManager.Projects.Project project = PluginCore.PluginBase.CurrentProject as ProjectManager.Projects.Project;
                    project.Storage.Add("air-descriptor", project.GetRelativePath(_propertiesFile));
                    project.Save();
                }
            }
        }

        private Boolean CheckUniformFileNamePrefix(Boolean isApplicationIcon)
        {
            Boolean isValid = true;
            if (_pluginMain.Settings.UseUniformFilenames)
            {

                if (isApplicationIcon)
                {
                    if (!ValidationErrorProvider.GetError(FileNameField).Equals(String.Empty))
                    {
                        MessageBox.Show(String.Format(TextHelper.GetString("Alert.Message.FileNamePrefixInvalid"), "File Name"), TextHelper.GetString("Alert.Title.FileNamePrefixInvalid"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        isValid = false;
                    }
                }
                else
                {
                    if (!ValidationErrorProvider.GetError(FileTypeExtensionField).Equals(String.Empty))
                    {
                        MessageBox.Show(String.Format(TextHelper.GetString("Alert.Message.FileNamePrefixInvalid"), "Extension"), TextHelper.GetString("Alert.Title.FileNamePrefixInvalid"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        isValid = false;
                    }
                }
            }
            return isValid;
        }

        private void UpdateIcon(string fileName, ref TextBox fileNameTextBox, Point dimensions, Boolean isApplicationIcon)
        {

            string projectPath = Path.GetDirectoryName(PluginCore.PluginBase.CurrentProject.ProjectPath);
            string iconFolder = this._pluginMain.Settings.ProjectIconsFolder;
            string destinationPath = projectPath + @"\" + iconFolder;
            string destinationFileName;
            string packagedFileName = this._pluginMain.Settings.PackageIconsFolder;
            string filePrefix;
            if (isApplicationIcon) filePrefix = FileNameField.Text;
            else filePrefix = FileTypeExtensionField.Text;
            Bitmap img = new Bitmap(fileName);
            // first, check if the image is the correct size
            if (img.Width == dimensions.X && img.Height == dimensions.Y)
            {
                // now check if file is in path relative to project root folder
                // and if not, save to icons folder as specified by plugin settings
                if (!Path.GetDirectoryName(fileName).ToLower().Equals(destinationPath.ToLower()))
                {
                    if (_pluginMain.Settings.UseUniformFilenames)
                    {
                        destinationFileName = filePrefix + dimensions.X.ToString() + Path.GetExtension(fileName);
                    }
                    else if (_pluginMain.Settings.RenameIconsWithSize)
                    {
                        destinationFileName = Path.GetFileNameWithoutExtension(fileName) + dimensions.X.ToString() + Path.GetExtension(fileName);
                    }
                    else destinationFileName = Path.GetFileName(fileName);
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }
                    File.Copy(fileName, destinationPath + @"\" + destinationFileName, true);
                }
                else
                {
                    // if inside destinationPath then just use the current filename
                    destinationFileName = Path.GetFileName(fileName);
                }
                if (packagedFileName.Length > 1) packagedFileName += @"/";
                packagedFileName += destinationFileName;
                fileNameTextBox.Text = packagedFileName;
            }
            else
            {
                MessageBox.Show(String.Format(TextHelper.GetString("Alert.Message.InvalidIconDimensions"), dimensions.X.ToString(), dimensions.Y.ToString()), TextHelper.GetString("Alert.Title.InvalidIconDimensions"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        // adds the loaded file types to the list view
        private void InitializeFileTypesListView()
        {
            ListViewItem fileTypeListItem;
            FileTypesListView.SmallImageList = ListViewStateImageList;
            ListViewStateImageList.Images.Add(ValidationErrorProvider.Icon);
            if (_fileTypes.Count > 0)
            {
                foreach (PropertyManager.AirFileType fileType in _fileTypes)
                {
                    fileTypeListItem = new ListViewItem(fileType.Name);
                    fileTypeListItem.SubItems.Add(fileType.Extension);
                    if (fileType.IsValid)
                    {
                        fileTypeListItem.ToolTipText = String.Empty;
                        fileTypeListItem.ImageIndex = 0;
                    }
                    else
                    {
                        fileTypeListItem.ToolTipText = String.Format(TextHelper.GetString("Validation.InvalidFileType"), fileType.Name);
                        fileTypeListItem.ImageIndex = 1;
                    }
                    FileTypesListView.Items.Add(fileTypeListItem);
                }
                FileTypesListView.Items[0].Selected = true;
            }
            LoadSelectedFileType();
        }

        //adds the loaded extensions to the list view
        private void InitializeExtensionsListView()
        {
            ListViewItem extensionListItem;
            ExtensionsListView.SmallImageList = ListViewStateImageList;
            ListViewStateImageList.Images.Add(ValidationErrorProvider.Icon);
            if (_extensions.Count > 0)
            {
                foreach (PropertyManager.AirExtension extension in _extensions)
                {
                    extensionListItem = new ListViewItem(extension.ExtensionId);
                    if (extension.IsValid)
                    {
                        extensionListItem.ToolTipText = String.Empty;
                        extensionListItem.ImageIndex = 0;
                    }
                    else
                    {
                        extensionListItem.ToolTipText = String.Format(TextHelper.GetString("Validation.InvalidExtension"), extension.ExtensionId);
                        extensionListItem.ImageIndex = 1;
                    }
                    ExtensionsListView.Items.Add(extensionListItem);
                }
                ExtensionsListView.Items[0].Selected = true;
            }
            LoadSelectedExtension();
        }

        // gets the corresponding file type object from the list view selection
        private PropertyManager.AirFileType GetSelectedFileType()
        {
            PropertyManager.AirFileType selectedFileType = null;
            int selectedIndex = -1;
            if (FileTypesListView.SelectedIndices.Count > 0)
            {
                selectedIndex = FileTypesListView.SelectedIndices[0];
            }
            if (selectedIndex >= 0 && _isPropertiesLoaded)
            {
                selectedFileType = _fileTypes[selectedIndex];
            }
            return selectedFileType;
        }

        // gets the corresponding extension object from the list view selection
        private PropertyManager.AirExtension GetSelectedExtension()
        {
            PropertyManager.AirExtension selectedExtension = null;
            int selectedIndex = -1;
            if (ExtensionsListView.SelectedIndices.Count > 0)
            {
                selectedIndex = ExtensionsListView.SelectedIndices[0];
            }
            if (selectedIndex >= 0 && _isPropertiesLoaded)
            {
                selectedExtension = _extensions[selectedIndex];
            }
            return selectedExtension;
        }

        // refreshes the list view to reflect changes to the selected item
        private void RefreshSelectedFileType()
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                FileTypesListView.SelectedItems[0].SubItems[0].Text = selectedFileType.Name;
                FileTypesListView.SelectedItems[0].SubItems[1].Text = selectedFileType.Extension;
                if (selectedFileType.IsValid)
                {
                    FileTypesListView.SelectedItems[0].ToolTipText = String.Empty;
                    FileTypesListView.SelectedItems[0].ImageIndex = 0;
                }
                else
                {
                    FileTypesListView.SelectedItems[0].ToolTipText = String.Format(TextHelper.GetString("Validation.InvalidFileType"), selectedFileType.Name);
                    FileTypesListView.SelectedItems[0].ImageIndex = 1;
                }
            }
        }

        // refreshes the list view to reflect changes to the selected item
        private void RefreshSelectedExtension()
        {
            PropertyManager.AirExtension selectedExtension = GetSelectedExtension();
            if (selectedExtension != null)
            {
                ExtensionsListView.SelectedItems[0].SubItems[0].Text = selectedExtension.ExtensionId;
                if (selectedExtension.IsValid)
                {
                    ExtensionsListView.SelectedItems[0].ToolTipText = String.Empty;
                    ExtensionsListView.SelectedItems[0].ImageIndex = 0;
                }
                else
                {
                    ExtensionsListView.SelectedItems[0].ToolTipText = String.Format(TextHelper.GetString("Validation.InvalidExtension"), selectedExtension.ExtensionId);
                    ExtensionsListView.SelectedItems[0].ImageIndex = 1;
                }
            }
        }

        //loads the properties of the selected file type to the corresponding controls
        private void LoadSelectedFileType()
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                FileTypeDetailsTabControl.Enabled = true;
                RemoveFileTypeButton.Enabled = true;
                FileTypeNameField.Text = selectedFileType.Name;
                FileTypeExtensionField.Text = selectedFileType.Extension;
                FileTypeDescriptionField.Text = selectedFileType.Description;
                FileTypeContentTypeField.Text = selectedFileType.ContentType;
                FileTypeIconField16.Text = selectedFileType.GetIconPath(16);
                FileTypeIconField32.Text = selectedFileType.GetIconPath(32);
                FileTypeIconField48.Text = selectedFileType.GetIconPath(48);
                FileTypeIconField128.Text = selectedFileType.GetIconPath(128);
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                {
                    FileTypeIconField29.Text = selectedFileType.GetIconPath(29);
                    FileTypeIconField72.Text = selectedFileType.GetIconPath(72);
                    FileTypeIconField512.Text = selectedFileType.GetIconPath(512);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                {
                    FileTypeIconField36.Text = selectedFileType.GetIconPath(36);
                }
            }
            else
            {
                FileTypeDetailsTabControl.Enabled = false;
                RemoveFileTypeButton.Enabled = false;
                FileTypeDetailsTabControl.SelectedTab = FileTypeDetailsTabPage;
                FileTypeNameField.Text = "";
                FileTypeExtensionField.Text = "";
                FileTypeDescriptionField.Text = "";
                FileTypeContentTypeField.Text = "";
                FileTypeIconField16.Text = "";
                FileTypeIconField32.Text = "";
                FileTypeIconField48.Text = "";
                FileTypeIconField128.Text = "";
                ValidationErrorProvider.SetError(FileTypeNameField, String.Empty);
                ValidationErrorProvider.SetError(FileTypeExtensionField, String.Empty);
                ValidationErrorProvider.SetError(FileTypeDescriptionField, String.Empty);
                ValidationErrorProvider.SetError(FileTypeContentTypeField, String.Empty);
                ValidationErrorProvider.SetError(FileTypeIconField16, String.Empty);
                ValidationErrorProvider.SetError(FileTypeIconField32, String.Empty);
                ValidationErrorProvider.SetError(FileTypeIconField48, String.Empty);
                ValidationErrorProvider.SetError(FileTypeIconField128, String.Empty);
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                {
                    FileTypeIconField29.Text = String.Empty;
                    FileTypeIconField72.Text = String.Empty;
                    FileTypeIconField512.Text = String.Empty;
                    ValidationErrorProvider.SetError(FileTypeIconField29, String.Empty);
                    ValidationErrorProvider.SetError(FileTypeIconField72, String.Empty);
                    ValidationErrorProvider.SetError(FileTypeIconField512, String.Empty);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                {
                    FileTypeIconField36.Text = String.Empty;
                    ValidationErrorProvider.SetError(FileTypeIconField36, String.Empty);
                }
            }
        }

        // loads the properties of the selected extension to the corresponding controls
        private void LoadSelectedExtension()
        {
            PropertyManager.AirExtension selectedExtension = GetSelectedExtension();
            if (selectedExtension != null)
            {
               
                ExtensionRemoveButton.Enabled = true;
                ExtensionIdField.Enabled = true;
                ExtensionIdField.Text = selectedExtension.ExtensionId;
            }
            else
            {
                ExtensionRemoveButton.Enabled = false;
                ExtensionIdField.Enabled = false;
                ExtensionIdField.Text = String.Empty;
                ValidationErrorProvider.SetError(ExtensionIdField, String.Empty);
            }
        }

        // used to format CDATA values for Android and iPhone additions (which is XML)
        private String FormatXML(string xml, string nameSpace)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.ConformanceLevel = ConformanceLevel.Fragment;
            NameTable nameTable = new NameTable();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace(nameSpace, nameSpace);
            MemoryStream ms = new MemoryStream();
            // Create a XMLTextWriter that will send its output to a memory stream (file)
            XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);
            XmlParserContext parserContext = new XmlParserContext(nameTable, namespaceManager, xtw.XmlLang, xtw.XmlSpace);
            try
            {
                // Load the unformatted XML text string into an XPath Document
                XPathDocument doc = new XPathDocument(XmlReader.Create(new StringReader(xml), readerSettings, parserContext));
                // Set the formatting property of the XML Text Writer to indented
                // the text writer is where the indenting will be performed
                xtw.Formatting = Formatting.Indented;
                xtw.IndentChar = '\x09'; //tab
                xtw.Indentation = 1;                
                // write doc xml to the xmltextwriter
                doc.CreateNavigator().WriteSubtree(xtw);
                // Flush the contents of the text writer
                // to the memory stream, which is simply a memory file
                xtw.Flush();
                // set to start of the memory stream (file)
                ms.Seek(0, SeekOrigin.Begin);
                // create a reader to read the contents of
                // the memory stream (file)
                StreamReader sr = new StreamReader(ms);
                // return the formatted string to caller (without the namespace declaration)
                return sr.ReadToEnd().Replace(" xmlns:" + nameSpace + "=\"" + nameSpace + "\"", ""); 
            }
            catch (Exception)
            {
                //debug purposes only
                //MessageBox.Show(ex.ToString());
                //return original xml
                return xml;
            }
        }

        #region Event Handlers

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (ValidateChildren())
            {
                SaveProperties();
                Close();
            }
            else
            {
                MessageBox.Show(TextHelper.GetString("Alert.Message.InvalidProperties"), TextHelper.GetString("Alert.Title.InvalidProperties"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            //I don't think Adobe provide localized documentation so there's no point in localizing the help url
            System.Diagnostics.Process.Start("http://help.adobe.com/en_US/air/build/WS5b3ccc516d4fbf351e63e3d118666ade46-7ff1.html");
        }

        private void SystemChromeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateSystemChrome();
        }

        private void AppIconButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String size = btn.Name.Replace("AppIconButton", "");
            if (CheckUniformFileNamePrefix(true) && OpenIconFileDialog.ShowDialog() == DialogResult.OK)
            {
                Control[] controls = AppIconsPanel.Controls.Find("AppIconField" + size, true);
                if (controls.Length > 0)
                {
                    TextBox filePath = (TextBox)controls[0];
                    UpdateIcon(OpenIconFileDialog.FileName, ref filePath, new Point(Convert.ToInt32(size), Convert.ToInt32(size)), true);
                }
            }
        }

        private void FileTypeIconButtonClick(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            String size = btn.Name.Replace("FileTypeIconButton", "");
            if (CheckUniformFileNamePrefix(false) && OpenIconFileDialog.ShowDialog() == DialogResult.OK)
            {
                Control[] controls = FileTypeIconsPanel.Controls.Find("FileTypeIconField" + size, true);
                if (controls.Length > 0)
                {
                    TextBox filePath = (TextBox)controls[0];
                    UpdateIcon(OpenIconFileDialog.FileName, ref filePath, new Point(Convert.ToInt32(size), Convert.ToInt32(size)), false);
                    PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
                    if (selectedFileType != null)
                    {
                        selectedFileType.SetIconPath(Convert.ToInt16(size), filePath.Text);
                    } 
                }
            }
        }

        private void LocalesField_SelectedIndexChanged(object sender, EventArgs e)
        {
            //refresh the locale-specific properties
            if (_isPropertiesLoaded)
            {
                PropertyManager.GetProperty("name", NameField, GetSelectedLocale());
                PropertyManager.GetProperty("description", DescriptionField, GetSelectedLocale());
            }
        }

        private void LocalesField_Enter(object sender, EventArgs e)
        {
            //save the locale-specific properties in case locale is about to change
            if (_isPropertiesLoaded)
            {
                PropertyManager.SetProperty("name", NameField, GetSelectedLocale(), GetSelectedLocaleIsDefault());
                PropertyManager.SetProperty("description", DescriptionField, GetSelectedLocale(), GetSelectedLocaleIsDefault());
            }
        }

        private void LocaleManagerButton_Click(object sender, EventArgs e)
        {
            List<string> originalLocales = new List<string>();
            originalLocales.AddRange(_locales);
            //Save active 
            PropertyManager.SetProperty("name", NameField, GetSelectedLocale(), GetSelectedLocaleIsDefault());
            PropertyManager.SetProperty("description", DescriptionField, GetSelectedLocale(), GetSelectedLocaleIsDefault());
            LocaleManager frmLocaleMan = new LocaleManager(ref _locales);
            if (frmLocaleMan.ShowDialog(this) == DialogResult.OK)
            {
                //Check to see if any locales have been removed
                foreach (string locale in originalLocales)
                {
                    if (!_locales.Contains(locale))
                    {
                        //remove affected properties from properties file
                        PropertyManager.RemoveLocalizedProperty("name", locale);
                        PropertyManager.RemoveLocalizedProperty("description", locale);
                    }
                }
                //Check to see if any locales have been added
                foreach (string locale in _locales)
                {
                    if (!originalLocales.Contains(locale))
                    {
                        //create the affected properties now, even though value is empty, so the locale 
                        //will be preserved if the user closes the form without specifying a value
                        PropertyManager.CreateLocalizedProperty("name", locale, (Boolean)_locales[0].Equals(locale));
                        PropertyManager.CreateLocalizedProperty("description", locale, (Boolean)_locales[0].Equals(locale));
                    }
                }
                //Re-initialize locales and refresh affected property fields
                InitializeLocales();
                PropertyManager.GetProperty("name", NameField, GetSelectedLocale());
                PropertyManager.GetProperty("description", DescriptionField, GetSelectedLocale());
            }
            else
            {
                //reset the locales in case any changes were made
                _locales.Clear();
                _locales.AddRange(originalLocales);
            }
        }

        private void FileTypesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedFileType();
            //Validate all controls so the error provider activates on any invalid values
            //(bit heavy handed as it does all controls - not just those for file types, but it's way easier and the performance seems fine).
            ValidateChildren();
        }
        
        private void NewFileTypeButton_Click(object sender, EventArgs e)
        {
            //create new file type and call validate file types to set validation flags
            PropertyManager.AirFileType fileType = new PropertyManager.AirFileType();
            _fileTypes.Add(fileType);
            ValidateFileTypes();
            //add the item to the list view and select it
            ListViewItem fileTypeListItem = new ListViewItem(fileType.Name);
            fileTypeListItem.SubItems.Add(fileType.Extension);
            FileTypesListView.Items.Add(fileTypeListItem);
            FileTypesListView.SelectedIndices.Clear();
            FileTypesListView.Items[FileTypesListView.Items.Count - 1].Selected = true;
            FileTypeDetailsTabControl.SelectedTab = FileTypeDetailsTabPage;
            RefreshSelectedFileType();
        }

        private void RemoveFileTypeButton_Click(object sender, EventArgs e)
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                _fileTypes.Remove(selectedFileType);
                FileTypesListView.Items.RemoveAt(FileTypesListView.SelectedIndices[0]);
            }
        }

        private void ExtensionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedExtension();
            //Validate all controls so the error provider activates on any invalid values
            //(bit heavy handed as it does all controls - not just those for extensions, but it's way easier and the performance seems fine).
            ValidateChildren();
        }

        private void ExtensionAddButton_Click(object sender, EventArgs e)
        {
            //create new file type and call validate extensions to set validation flags
            PropertyManager.AirExtension extension = new PropertyManager.AirExtension();
            _extensions.Add(extension);
            ValidateExtensions();
            //add the item to the list view and select it
            ListViewItem extensionListItem = new ListViewItem(extension.ExtensionId);
            ExtensionsListView.Items.Add(extensionListItem);
            ExtensionsListView.SelectedIndices.Clear();
            ExtensionsListView.Items[ExtensionsListView.Items.Count - 1].Selected = true;
            RefreshSelectedExtension();
        }

        private void ExtensionRemoveButton_Click(object sender, EventArgs e)
        {
            PropertyManager.AirExtension selectedExtension = GetSelectedExtension();
            if (selectedExtension != null)
            {
                _extensions.Remove(selectedExtension);
                ExtensionsListView.Items.RemoveAt(ExtensionsListView.SelectedIndices[0]);
            }
        }

        private void SupportedLanguagesButton_Click(object sender, EventArgs e)
        {
            List<string> locales = new List<string>();
            locales.AddRange(SupportedLanguagesField.Text.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));
            LocaleManager frmLocaleMan = new LocaleManager(ref locales);
            if (frmLocaleMan.ShowDialog(this) == DialogResult.OK)
            {
                SupportedLanguagesField.Text = String.Join(" ", locales.ToArray());
            }

        }

        #endregion

        #region Validation Event Handlers / Methods

        // Ensures that Transparent value is false and cannot 
        // be changed when System Chrome is Standard
        private void ValidateSystemChrome()
        {
            ListItem item;
            item = (ListItem)SystemChromeField.SelectedItem;
            if (item != null && item.Value == "standard")
            {
                TransparentField.Checked = false;
                TransparentField.Enabled = false;
            }
            else TransparentField.Enabled = true;
        }

        // validates that the supplied image URI is a PNG file
        private Boolean ValidateImageExtension(String imageURI)
        {
            if (imageURI.ToLower().EndsWith(".png")) return true;
            else return false;
        }

        // loops through each loaded file type and sets initial validity flags
        // does represent a little bit of code duplication, but shit happens.
        // we need to do this is in case there are file types with invalid property values when the file is opened, or when a new file type is added
        private void ValidateFileTypes()
        {
            foreach (PropertyManager.AirFileType fileType in _fileTypes)
            {
                // always true;
                fileType.DescriptionIsValid = true;
                // set validity based on validation requirements
                if (fileType.Name.Length <= 0) fileType.NameIsValid = false;
                else fileType.NameIsValid = true;
                if (!Regex.IsMatch(fileType.Extension, _FileNameRegexPattern)) fileType.ExtensionIsValid = false;
                else fileType.ExtensionIsValid = true;
                if (fileType.ContentType.Length <= 0) fileType.ContentTypeIsValid = false;
                else fileType.ContentTypeIsValid = true;
                foreach (PropertyManager.AirFileType.AirFileTypeIcon icon in fileType.Icons)
                {
                    if (icon.MinVersion <= PropertyManager.MajorVersion)
                    {
                        if (icon.FilePath.Length > 0 && (!Regex.IsMatch(icon.FilePath, _AnyUriRegexPattern) || !ValidateImageExtension(icon.FilePath)))
                        {
                            icon.IsValid = false;
                        }
                        else icon.IsValid = true;
                    }
                }
            }
        }

        // loops through each loaded extension and sets initial validity flags
        // does represent a little bit of code duplication, but shit happens.
        // we need to do this is in case there are extensions with invalid property values when the file is opened, or when a new extension is added
        private void ValidateExtensions()
        {
            foreach (PropertyManager.AirExtension extension in _extensions)
            {
                if (extension.ExtensionId.Length > 0 && !Regex.IsMatch(extension.ExtensionId, _ExtensionRegexPattern)) extension.IsValid = false;
                else extension.IsValid = true;
            }
        }

        private void ValidateAppIconField(object sender, CancelEventArgs e)
        {
            TextBox field = (TextBox)sender;
            String size = field.Name.Replace("tbAppIcon", "");
            field.Text = field.Text.Trim();
            if (field.Visible && field.Text.Length > 0 && (!Regex.IsMatch(field.Text, _AnyUriRegexPattern) || !ValidateImageExtension(field.Text)))
            {
                this.ValidationErrorProvider.SetError(field, String.Format(TextHelper.GetString("Validation.InvalidProperty"), TextHelper.GetString("Label.Icon") + " " + size + " x " + size));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(field, String.Empty);
        }

        private void ValidateFileTypeIconField(object sender, CancelEventArgs e)
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            TextBox field = (TextBox)sender;
            String size = field.Name.Replace("FileTypeIconField", "");
            if (selectedFileType != null)
            {
                field.Text = field.Text.Trim();
                selectedFileType.SetIconPath(Convert.ToInt16(size), field.Text);
                if (field.Text.Length > 0 && (!Regex.IsMatch(field.Text, _AnyUriRegexPattern) || !ValidateImageExtension(field.Text)))
                {
                    selectedFileType.SetIconIsValid(Convert.ToInt16(size), false);
                    this.ValidationErrorProvider.SetError(field, String.Format(TextHelper.GetString("Validation.InvalidProperty"), TextHelper.GetString("Label.Icon") + " " + size + " x " + size));
                    e.Cancel = true;
                }
                else
                {
                    selectedFileType.SetIconIsValid(Convert.ToInt16(size), true);
                    this.ValidationErrorProvider.SetError(field, String.Empty);
                }
                RefreshSelectedFileType();
            }
        }

        private void IDField_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(IDField.Text, _IdRegexPattern))
            {
                this.ValidationErrorProvider.SetError(IDField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), IDLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(IDField, "");
        }

        private void VersionField_Validating(object sender, CancelEventArgs e)
        {
            if (VersionField.Text.Length <= 0 && PropertyManager.MajorVersion < PropertyManager.AirVersion.V25)
            {
                this.ValidationErrorProvider.SetError(VersionField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), VersionLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(VersionField, "");
        }

        private void FileNameField_Validating(object sender, CancelEventArgs e)
        {
            FileNameField.Text = FileNameField.Text.Trim();
            if (!Regex.IsMatch(FileNameField.Text, _FileNameRegexPattern))
            {
                this.ValidationErrorProvider.SetError(FileNameField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), FileNameLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(FileNameField, "");
        }

        private void InstallFolderField_Validating(object sender, CancelEventArgs e)
        {
            InstallFolderField.Text = InstallFolderField.Text.Trim();
            if (InstallFolderField.Text.Length > 0 && !Regex.IsMatch(InstallFolderField.Text, _FilePathRegexPattern))
            {
                this.ValidationErrorProvider.SetError(InstallFolderField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), InstallFolderLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(InstallFolderField, "");
        }

        private void ProgramMenuFolderField_Validating(object sender, CancelEventArgs e)
        {
            ProgramMenuFolderField.Text = ProgramMenuFolderField.Text.Trim();
            if (ProgramMenuFolderField.Text.Length > 0 && !Regex.IsMatch(ProgramMenuFolderField.Text, _FilePathRegexPattern))
            {
                this.ValidationErrorProvider.SetError(ProgramMenuFolderField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), ProgramMenuFolderLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(ProgramMenuFolderField, "");
        }


        private void ContentField_Validating(object sender, CancelEventArgs e)
        {
            ContentField.Text = ContentField.Text.Trim();
            if (!Regex.IsMatch(ContentField.Text, _AnyUriRegexPattern))
            {
                this.ValidationErrorProvider.SetError(ContentField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), ContentLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(ContentField, "");
        }

        private void XField_Validating(object sender, CancelEventArgs e)
        {
            XField.Text = XField.Text.Trim();
            if (XField.Text.Length > 0 && !Regex.IsMatch(XField.Text, _CoordinateRegexPattern))
            {
                this.ValidationErrorProvider.SetError(XField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), InitialLocationLabel.Text + " (X)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(XField, "");
        }

        private void YField_Validating(object sender, CancelEventArgs e)
        {
            YField.Text = YField.Text.Trim();
            if (YField.Text.Length > 0 && !Regex.IsMatch(YField.Text, _CoordinateRegexPattern))
            {
                this.ValidationErrorProvider.SetError(YField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), InitialLocationLabel.Text + " (Y)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(YField, "");
        }

        private void WidthField_Validating(object sender, CancelEventArgs e)
        {
            WidthField.Text = WidthField.Text.Trim();
            if (WidthField.Text.Length > 0 && !Regex.IsMatch(WidthField.Text, _NumberRegexPattern))
            {
                this.ValidationErrorProvider.SetError(WidthField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), InitialSizeLabel.Text + " (X)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(WidthField, "");
        }

        private void HeightField_Validating(object sender, CancelEventArgs e)
        {
            HeightField.Text = HeightField.Text.Trim();
            if (HeightField.Text.Length > 0 && !Regex.IsMatch(HeightField.Text, _NumberRegexPattern))
            {
                this.ValidationErrorProvider.SetError(HeightField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), InitialSizeLabel.Text + " (Y)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(HeightField, "");
        }

        private void MinSizeXField_Validating(object sender, CancelEventArgs e)
        {
            bool isValid = true;
            MinSizeXField.Text = MinSizeXField.Text.Trim();
            if (MinSizeXField.Text.Length > 0)
            {
                if (!Regex.IsMatch(MinSizeXField.Text, _NumberRegexPattern)) isValid = false;
                else if (MaxSizeXField.Text.Length > 0 && ValidationErrorProvider.GetError(MaxSizeXField).Equals(String.Empty))
                {
                    if (Convert.ToInt32(MinSizeXField.Text) > Convert.ToInt32(MaxSizeXField.Text)) isValid = false;
                }
            }
            else if (MinSizeYField.Text.Length > 0)
            {
                isValid = false;
            }
            if (!isValid)
            {
                this.ValidationErrorProvider.SetError(MinSizeXField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), MinimumSizeLabel.Text + " (X)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(MinSizeXField, "");
        }

        private void MinSizeYField_Validating(object sender, CancelEventArgs e)
        {
            bool isValid = true;
            MinSizeYField.Text = MinSizeYField.Text.Trim();
            if (MinSizeYField.Text.Length > 0)
            {
                if (!Regex.IsMatch(MinSizeYField.Text, _NumberRegexPattern)) isValid = false;
                else if (MaxSizeYField.Text.Length > 0 && ValidationErrorProvider.GetError(MaxSizeYField).Equals(String.Empty))
                {
                    if (Convert.ToInt32(MinSizeYField.Text) > Convert.ToInt32(MaxSizeYField.Text)) isValid = false;
                }
            }
            else if (MinSizeXField.Text.Length > 0)
            {
                isValid = false;
            }
            if (!isValid)
            {
                this.ValidationErrorProvider.SetError(MinSizeYField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), MinimumSizeLabel.Text + " (Y)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(MinSizeYField, "");
        }

        private void MaxSizeXField_Validating(object sender, CancelEventArgs e)
        {
            bool isValid = true;
            MaxSizeXField.Text = MaxSizeXField.Text.Trim();
            if (MaxSizeXField.Text.Length > 0)
            {
                if (!Regex.IsMatch(MaxSizeXField.Text, _NumberRegexPattern)) isValid = false;
                else if (MinSizeXField.Text.Length > 0 && ValidationErrorProvider.GetError(MinSizeXField).Equals(String.Empty))
                {
                    if (Convert.ToInt32(MaxSizeXField.Text) < Convert.ToInt32(MinSizeXField.Text)) isValid = false;
                }
            }
            else if (MaxSizeYField.Text.Length > 0)
            {
                isValid = false;
            }
            if (!isValid)
            {
                this.ValidationErrorProvider.SetError(MaxSizeXField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), MaximumSizeLabel.Text + " (X)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(MaxSizeXField, "");
        }

        private void MaxSizeYField_Validating(object sender, CancelEventArgs e)
        {
            bool isValid = true;
            MaxSizeYField.Text = MaxSizeYField.Text.Trim();
            if (MaxSizeYField.Text.Length > 0)
            {
                if (!Regex.IsMatch(MaxSizeYField.Text, _NumberRegexPattern)) isValid = false;
                else if (MinSizeYField.Text.Length > 0 && ValidationErrorProvider.GetError(MinSizeYField).Equals(String.Empty))
                {
                    if (Convert.ToInt32(MaxSizeYField.Text) < Convert.ToInt32(MinSizeYField.Text)) isValid = false;
                }
            }
            else if (MaxSizeXField.Text.Length > 0)
            {
                isValid = false;
            }
            if (!isValid)
            {
                this.ValidationErrorProvider.SetError(MaxSizeYField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), MaximumSizeLabel.Text + " (Y)"));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(MaxSizeYField, "");
        }

        private void FileTypeNameField_Validating(object sender, CancelEventArgs e)
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                FileTypeNameField.Text = FileTypeNameField.Text.Trim();
                selectedFileType.Name = FileTypeNameField.Text;
                if (FileTypeNameField.Text.Length <= 0)
                {
                    selectedFileType.NameIsValid = false;
                    this.ValidationErrorProvider.SetError(FileTypeNameField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), FTNameLabel.Text));
                    e.Cancel = true;
                }
                else
                {
                    selectedFileType.NameIsValid = true;
                    this.ValidationErrorProvider.SetError(FileTypeNameField, "");
                }
                RefreshSelectedFileType();
            }
        }

        private void FileTypeExtensionField_Validating(object sender, CancelEventArgs e)
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                FileTypeExtensionField.Text = FileTypeExtensionField.Text.Trim();
                FileTypeExtensionField.Text = FileTypeExtensionField.Text.Trim('.');
                selectedFileType.Extension = FileTypeExtensionField.Text;
                if (!Regex.IsMatch(FileTypeExtensionField.Text, _FileNameRegexPattern))
                {
                    selectedFileType.ExtensionIsValid = false;
                    this.ValidationErrorProvider.SetError(FileTypeExtensionField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), FTExtensionLabel.Text));
                    e.Cancel = true;
                }
                else
                {
                    selectedFileType.ExtensionIsValid = true;
                    this.ValidationErrorProvider.SetError(FileTypeExtensionField, "");
                }
                RefreshSelectedFileType();
            }
        }

        private void FileTypeDescriptionField_Validating(object sender, CancelEventArgs e)
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                FileTypeDescriptionField.Text = FileTypeDescriptionField.Text.Trim();
                selectedFileType.Description = FileTypeDescriptionField.Text;
            }
        }

        private void FileTypeContentTypeField_Validating(object sender, CancelEventArgs e)
        {
            PropertyManager.AirFileType selectedFileType = GetSelectedFileType();
            if (selectedFileType != null)
            {
                FileTypeContentTypeField.Text = FileTypeContentTypeField.Text.Trim();
                selectedFileType.ContentType = FileTypeContentTypeField.Text;
                // validate as required field for AIR 1.5+
                if (FileTypeContentTypeField.Text.Length <= 0 && PropertyManager.MajorVersion >= PropertyManager.AirVersion.V15)
                {
                    selectedFileType.ContentTypeIsValid = false;
                    this.ValidationErrorProvider.SetError(FileTypeContentTypeField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), FTContentTypeLabel.Text));
                    e.Cancel = true;
                }
                else
                {
                    selectedFileType.ContentTypeIsValid = true;
                    this.ValidationErrorProvider.SetError(FileTypeContentTypeField, "");
                }
                RefreshSelectedFileType();
            }
        }

        private void FileTypesListView_Validating(object sender, CancelEventArgs e)
        {
            foreach (PropertyManager.AirFileType fileType in _fileTypes)
            {
                if (!fileType.IsValid)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void PublisherIDField_Validating(object sender, CancelEventArgs e)
        {
            PublisherIdField.Text = PublisherIdField.Text.Trim();
            if (PublisherIdField.Text.Length > 0 && !Regex.IsMatch(PublisherIdField.Text, _PublisherRegexPattern))
            {
                this.ValidationErrorProvider.SetError(PublisherIdField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), PublisherIdLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(PublisherIdField, "");
        }

        private void VersionNoField_Validating(object sender, CancelEventArgs e)
        {
            VersionNoField.Text = VersionNoField.Text.Trim();
            if (VersionNoField.Text.Length > 0 && !Regex.IsMatch(VersionNoField.Text, _VersionRegexPattern))
            {
                this.ValidationErrorProvider.SetError(VersionNoField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), VersionNoLabel.Text));
                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(VersionNoField, "");
        }

        private void ExtensionIdField_Validating(object sender, CancelEventArgs e)
        {
            PropertyManager.AirExtension selectedExtension = GetSelectedExtension();
            if (selectedExtension != null)
            {
                ExtensionIdField.Text = ExtensionIdField.Text.Trim();
                selectedExtension.ExtensionId = ExtensionIdField.Text;
                if (ExtensionIdField.Text.Length > 0 && !Regex.IsMatch(ExtensionIdField.Text, _ExtensionRegexPattern))
                {
                    selectedExtension.IsValid = false;
                    this.ValidationErrorProvider.SetError(ExtensionIdField, String.Format(TextHelper.GetString("Validation.InvalidProperty"), ExtensionIdLabel.Text));
                    e.Cancel = true;
                }
                else
                {
                    selectedExtension.IsValid = true;
                    this.ValidationErrorProvider.SetError(ExtensionIdField, "");
                }

                RefreshSelectedExtension();
            }
        }

        private void ExtensionsListView_Validating(object sender, CancelEventArgs e)
        {
            foreach (PropertyManager.AirExtension extension in _extensions)
            {
                if (!extension.IsValid)
                {
                    e.Cancel = true;
                    break;
                }
            }
        }

        #endregion

    }

}

