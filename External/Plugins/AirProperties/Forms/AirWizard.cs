// TODO: This form is getting huge, it should be refactored. It isn't hard, just abstract properties and link fields, possible values and valid versions together, taking into account that some versions add new values to existing properties. This would clean things like SetupUI, LoadProperties, SaveProperties...

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
using AirProperties.Forms;
using ICSharpCode.SharpZipLib.Zip;
using PluginCore.Localization;
using ProjectManager.Projects;
using System.Collections;
using ProjectManager.Projects.AS3;
using PluginCore.Managers;

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
        private List<string> _removedExtensions = new List<string>();

        // Android extra
        private List<PropertyManager.AndroidPermission> _androidPermissions;
        private AndroidManifestManager androidManifest;

        // iOS extra
        private IphonePlistManager iPhoneAdditions;
        private IphonePlistManager iPhoneEntitlements;

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
                new PropertyManager.AirApplicationIconField("40", PropertyManager.AirVersion.V39),
                new PropertyManager.AirApplicationIconField("48", PropertyManager.AirVersion.V10),
                new PropertyManager.AirApplicationIconField("50", PropertyManager.AirVersion.V34),
                new PropertyManager.AirApplicationIconField("57", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("58", PropertyManager.AirVersion.V34),
                new PropertyManager.AirApplicationIconField("64", PropertyManager.AirVersion.V170),
                new PropertyManager.AirApplicationIconField("72", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("76", PropertyManager.AirVersion.V39),
                new PropertyManager.AirApplicationIconField("80", PropertyManager.AirVersion.V39),
                new PropertyManager.AirApplicationIconField("87", PropertyManager.AirVersion.V170),
                new PropertyManager.AirApplicationIconField("96", PropertyManager.AirVersion.V20),
                new PropertyManager.AirApplicationIconField("100", PropertyManager.AirVersion.V34),
                new PropertyManager.AirApplicationIconField("114", PropertyManager.AirVersion.V26),
                new PropertyManager.AirApplicationIconField("120", PropertyManager.AirVersion.V39),
                new PropertyManager.AirApplicationIconField("128", PropertyManager.AirVersion.V10),
                new PropertyManager.AirApplicationIconField("144", PropertyManager.AirVersion.V33),
                new PropertyManager.AirApplicationIconField("152", PropertyManager.AirVersion.V39),
                new PropertyManager.AirApplicationIconField("180", PropertyManager.AirVersion.V170),
                new PropertyManager.AirApplicationIconField("192", PropertyManager.AirVersion.V150),
                new PropertyManager.AirApplicationIconField("320", PropertyManager.AirVersion.V170),
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
            // New
            this.DesktopResolutionLabel.Text = TextHelper.GetString("Label.Resolution");
            this.AndroidManifestAdditionsButton.Text = TextHelper.GetString("Label.Advanced");
            this.AndroidColorDepthLabel.Text = TextHelper.GetString("Label.ColorDepth");
            this.AndroidPermissionsLabel.Text = TextHelper.GetString("Label.SelectPermForInfo");
            this.AndroidManifestAdditionsButton.Text = TextHelper.GetString("Label.Advanced");
            this.IPhoneInfoAdditionsButton.Text = TextHelper.GetString("Label.Advanced");
            this.IPhoneOtherBehaviorGroup.Text = TextHelper.GetString("Label.OtherBehavior");
            this.label8.Text = TextHelper.GetString("Label.PushNotifications");
            this.IPhoneExternalSWFsLabel.Text = TextHelper.GetString("Label.ExternalSWFs");
            this.IPhoneLookGroup.Text = TextHelper.GetString("Label.LookAndFeel");
            this.IPhonePrerrenderedIconLabel.Text = TextHelper.GetString("Label.PrerenderedIcon");
            this.IPhoneStatusBarStyleLabel.Text = TextHelper.GetString("Label.StatusBarStyle");
            this.IPhoneBackgroundBehaviorGroup.Text = TextHelper.GetString("Label.BackgroundBehavior");
            this.IPhoneExitsOnSuspendLabel.Text = TextHelper.GetString("Label.ExitOnSuspend");
            this.IPhoneDeviceBehaviorGroup.Text = TextHelper.GetString("Label.Display");
            this.IPhoneResolutionExcludeButton.Text = TextHelper.GetString("Label.Exclude");
            this.IPhoneResolutionLabel.Text = TextHelper.GetString("Label.Resolution");
            this.IPhoneForceCPULabel.Text = TextHelper.GetString("Label.CPURendering");
            this.IPhoneDeviceLabel.Text = TextHelper.GetString("Label.SupportedDevices");
            this.IPhoneInfoAdditionsLabel.Text = TextHelper.GetString("Label.InfoAdditions");
            this.IPhoneEntitlementsLabel.Text = TextHelper.GetString("Label.Entitlements");
            this.ExtensionBrowseButton.Text = TextHelper.GetString("Label.Browse");
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
            SoftKeyboardField.Items.Add(new ListItem(String.Empty, String.Empty));
            SoftKeyboardField.Items.Add(new ListItem(TextHelper.GetString("SoftKeyboard.None"), "none"));
            SoftKeyboardField.Items.Add(new ListItem(TextHelper.GetString("SoftKeyboard.Pan"), "pan"));
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

        private void InitializeAndroidPermissions()
        {
            _androidPermissions = new List<PropertyManager.AndroidPermission>
                {
                    new PropertyManager.AndroidPermission("android.permission.INTERNET", "Allows applications to open network sockets. Removing the permission will prevent you from debugging your application on your device"),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_EXTERNAL_STORAGE", "Allows an application to write to external storage."),
                    new PropertyManager.AndroidPermission("android.permission.READ_PHONE_STATE", "Allows read only access to phone state."),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_FINE_LOCATION", "Allows an app to access precise location from location sources such as GPS, cell towers, and Wi-Fi."),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_COARSE_LOCATION", "Allows an app to access approximate location derived from network location sources such as cell towers and Wi-Fi."),
                    new PropertyManager.AndroidPermission("android.permission.DISABLE_KEYGUARD","Allows applications to disable the keyguard. WAKE_LOCK should be toggled together to access AIR's SystemIdleMode APIs"),
                    new PropertyManager.AndroidPermission("android.permission.WAKE_LOCK", "Allows using PowerManager WakeLocks to keep processor from sleeping or screen from dimming. Needed to access AIR's SystemIdleMode APIs"),
                    new PropertyManager.AndroidPermission("android.permission.CAMERA","Required to be able to access the camera device."),
                    new PropertyManager.AndroidPermission("android.permission.RECORD_AUDIO", "Allows an application to record audio"),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_NETWORK_STATE", "Allows applications to access information about networks. ACCESS_WIFI_STATE should be toggled together in order to use AIR's NetworkInfo APIs"),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_WIFI_STATE", "Allows applications to access information about Wi-Fi networks. Toggle ACCESS_NETWORK_STATE together in order to use AIR's NetworkInfo APIs"),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_CHECKIN_PROPERTIES", "Allows read/write access to the \"properties\" table in the checkin database, to change values that get uploaded."),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_LOCATION_EXTRA_COMMANDS", "Allows an application to access extra location provider commands"),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_MOCK_LOCATION", "Allows an application to create mock location providers for testing"),
                    new PropertyManager.AndroidPermission("android.permission.ACCESS_SURFACE_FLINGER", "Allows an application to use SurfaceFlinger's low level features."),
                    new PropertyManager.AndroidPermission("android.permission.ACCOUNT_MANAGER", "Allows applications to call into AccountAuthenticators."),
                    new PropertyManager.AndroidPermission("android.permission.ADD_VOICEMAIL", "Allows an application to add voicemails into the system."),
                    new PropertyManager.AndroidPermission("android.permission.AUTHENTICATE_ACCOUNTS", "Allows an application to act as an AccountAuthenticator for the AccountManager"),
                    new PropertyManager.AndroidPermission("android.permission.BATTERY_STATS", "Allows an application to collect battery statistics"),
                    new PropertyManager.AndroidPermission("android.permission.BIND_ACCESSIBILITY_SERVICE", "Must be required by an AccessibilityService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_APPWIDGET",  "Allows an application to tell the AppWidget service which application can access AppWidget's data."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_DEVICE_ADMIN", "Must be required by device administration receiver, to ensure that only the system can interact with it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_INPUT_METHOD", "Must be required by an InputMethodService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_NFC_SERVICE", "Must be required by a HostApduService or OffHostApduService to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_NOTIFICATION_LISTENER_SERVICE",  "Must be required by an NotificationListenerService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_PRINT_SERVICE",  "Must be required by a PrintService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_REMOTEVIEWS", "Must be required by a RemoteViewsService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_TEXT_SERVICE", "Must be required by a TextService (e.g. SpellCheckerService) to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_VPN_SERVICE", "Must be required by a VpnService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BIND_WALLPAPER", "Must be required by a WallpaperService, to ensure that only the system can bind to it."),
                    new PropertyManager.AndroidPermission("android.permission.BLUETOOTH", "Allows applications to connect to paired bluetooth devices"),
                    new PropertyManager.AndroidPermission("android.permission.BLUETOOTH_ADMIN"  ,"Allows applications to discover and pair bluetooth devices"),
                    new PropertyManager.AndroidPermission("android.permission.BLUETOOTH_PRIVILEGED", "Allows applications to pair bluetooth devices without user interaction."),
                    new PropertyManager.AndroidPermission("android.permission.BODY_SENSORS",    "Allows an application to access data from sensors that the user uses to measure what is happening inside his/her body, such as heart rate."),
                    new PropertyManager.AndroidPermission("android.permission.BRICK",   "Required to be able to disable the device (very dangerous!)."),
                    new PropertyManager.AndroidPermission("android.permission.BROADCAST_PACKAGE_REMOVED"    ,"Allows an application to broadcast a notification that an application package has been removed."),
                    new PropertyManager.AndroidPermission("android.permission.BROADCAST_SMS","Allows an application to broadcast an SMS receipt notification."),
                    new PropertyManager.AndroidPermission("android.permission.BROADCAST_STICKY", "Allows an application to broadcast sticky intents."),
                    new PropertyManager.AndroidPermission("android.permission.BROADCAST_WAP_PUSH","Allows an application to broadcast a WAP PUSH receipt notification."),
                    new PropertyManager.AndroidPermission("android.permission.CALL_PHONE","Allows an application to initiate a phone call without going through the Dialer user interface for the user to confirm the call being placed."),
                    new PropertyManager.AndroidPermission("android.permission.CALL_PRIVILEGED","Allows an application to call any phone number, including emergency numbers, without going through the Dialer user interface for the user to confirm the call being placed."),
                    new PropertyManager.AndroidPermission("android.permission.CAPTURE_AUDIO_OUTPUT","Allows an application to capture audio output."),
                    new PropertyManager.AndroidPermission("android.permission.CAPTURE_SECURE_VIDEO_OUTPUT","Allows an application to capture secure video output."),
                    new PropertyManager.AndroidPermission("android.permission.CAPTURE_VIDEO_OUTPUT","Allows an application to capture video output."),
                    new PropertyManager.AndroidPermission("android.permission.CHANGE_COMPONENT_ENABLED_STATE","Allows an application to change whether an application component (other than its own) is enabled or not."),
                    new PropertyManager.AndroidPermission("android.permission.CHANGE_CONFIGURATION","Allows an application to modify the current configuration, such as locale."),
                    new PropertyManager.AndroidPermission("android.permission.CHANGE_NETWORK_STATE","Allows applications to change network connectivity state"),
                    new PropertyManager.AndroidPermission("android.permission.CHANGE_WIFI_MULTICAST_STATE","Allows applications to enter Wi-Fi Multicast mode"),
                    new PropertyManager.AndroidPermission("android.permission.CHANGE_WIFI_STATE","Allows applications to change Wi-Fi connectivity state"),
                    new PropertyManager.AndroidPermission("android.permission.CLEAR_APP_CACHE","Allows an application to clear the caches of all installed applications on the device."),
                    new PropertyManager.AndroidPermission("android.permission.CLEAR_APP_USER_DATA","Allows an application to clear user data."),
                    new PropertyManager.AndroidPermission("android.permission.CONTROL_LOCATION_UPDATES","Allows enabling/disabling location update notifications from the radio."),
                    new PropertyManager.AndroidPermission("android.permission.DELETE_CACHE_FILES","Allows an application to delete cache files."),
                    new PropertyManager.AndroidPermission("android.permission.DELETE_PACKAGES","Allows an application to delete packages."),
                    new PropertyManager.AndroidPermission("android.permission.DEVICE_POWER","Allows low-level access to power management."),
                    new PropertyManager.AndroidPermission("android.permission.DIAGNOSTIC","Allows applications to RW to diagnostic resources."),
                    new PropertyManager.AndroidPermission("android.permission.DUMP","Allows an application to retrieve state dump information from system services."),
                    new PropertyManager.AndroidPermission("android.permission.EXPAND_STATUS_BAR", "Allows an application to expand or collapse the status bar."),
                    new PropertyManager.AndroidPermission("android.permission.FACTORY_TEST", "Run as a manufacturer test application, running as the root user."),
                    new PropertyManager.AndroidPermission("android.permission.FLASHLIGHT", "Allows access to the flashlight"),
                    new PropertyManager.AndroidPermission("android.permission.FORCE_BACK", "Allows an application to force a BACK operation on whatever is the top activity."),
                    new PropertyManager.AndroidPermission("android.permission.GET_ACCOUNTS", "Allows access to the list of accounts in the Accounts Service"),
                    new PropertyManager.AndroidPermission("android.permission.GET_PACKAGE_SIZE", "Allows an application to find out the space used by any package."),
                    new PropertyManager.AndroidPermission("android.permission.GET_TASKS", "Allows an application to get information about the currently or recently running tasks."),
                    new PropertyManager.AndroidPermission("android.permission.GET_TOP_ACTIVITY_INFO", "Allows an application to retrieve private information about the current top activity, such as any assist context it can provide."),
                    new PropertyManager.AndroidPermission("android.permission.GLOBAL_SEARCH", "This permission can be used on content providers to allow the global search system to access their data."),
                    new PropertyManager.AndroidPermission("android.permission.HARDWARE_TEST", "Allows access to hardware peripherals."),
                    new PropertyManager.AndroidPermission("android.permission.INJECT_EVENTS", "Allows an application to inject user events (keys, touch, trackball) into the event stream and deliver them to ANY window."),
                    new PropertyManager.AndroidPermission("android.permission.INSTALL_LOCATION_PROVIDER", "Allows an application to install a location provider into the Location Manager."),
                    new PropertyManager.AndroidPermission("android.permission.INSTALL_PACKAGES", "Allows an application to install packages."),
                    new PropertyManager.AndroidPermission("android.permission.INSTALL_SHORTCUT", "Allows an application to install a shortcut in Launcher"),
                    new PropertyManager.AndroidPermission("android.permission.INTERNAL_SYSTEM_WINDOW", "Allows an application to open windows that are for use by parts of the system user interface."),
                    new PropertyManager.AndroidPermission("android.permission.KILL_BACKGROUND_PROCESSES", "Allows an application to call killBackgroundProcesses(String)."),
                    new PropertyManager.AndroidPermission("android.permission.LOCATION_HARDWARE", "Allows an application to use location features in hardware, such as the geofencing api."),
                    new PropertyManager.AndroidPermission("android.permission.MANAGE_ACCOUNTS", "Allows an application to manage the list of accounts in the AccountManager"),
                    new PropertyManager.AndroidPermission("android.permission.MANAGE_APP_TOKENS", "Allows an application to manage (create, destroy, Z-order) application tokens in the window manager."),
                    new PropertyManager.AndroidPermission("android.permission.MANAGE_DOCUMENTS", "Allows an application to manage access to documents, usually as part of a document picker."),
                    new PropertyManager.AndroidPermission("android.permission.MASTER_CLEAR", "Not for use by third-party applications."),
                    new PropertyManager.AndroidPermission("android.permission.MEDIA_CONTENT_CONTROL", "Allows an application to know what content is playing and control its playback."),
                    new PropertyManager.AndroidPermission("android.permission.MODIFY_AUDIO_SETTINGS", "Allows an application to modify global audio settings"),
                    new PropertyManager.AndroidPermission("android.permission.MODIFY_PHONE_STATE", "Allows modification of the telephony state - power on, mmi, etc."),
                    new PropertyManager.AndroidPermission("android.permission.MOUNT_FORMAT_FILESYSTEMS", "Allows formatting file systems for removable storage."),
                    new PropertyManager.AndroidPermission("android.permission.MOUNT_UNMOUNT_FILESYSTEMS", "Allows mounting and unmounting file systems for removable storage."),
                    new PropertyManager.AndroidPermission("android.permission.NFC", "Allows applications to perform I/O operations over NFC"),
                    new PropertyManager.AndroidPermission("android.permission.PROCESS_OUTGOING_CALLS", "Allows an application to modify or abort outgoing calls."),
                    new PropertyManager.AndroidPermission("android.permission.READ_CALENDAR", "Allows an application to read the user's calendar data."),
                    new PropertyManager.AndroidPermission("android.permission.READ_CALL_LOG", "Allows an application to read the user's call log."),
                    new PropertyManager.AndroidPermission("android.permission.READ_CONTACTS", "Allows an application to read the user's contacts data."),
                    new PropertyManager.AndroidPermission("android.permission.READ_EXTERNAL_STORAGE", "Allows an application to read from external storage."),
                    new PropertyManager.AndroidPermission("android.permission.READ_FRAME_BUFFER", "Allows an application to take screen shots and more generally get access to the frame buffer data."),
                    new PropertyManager.AndroidPermission("android.permission.READ_HISTORY_BOOKMARKS", "Allows an application to read (but not write) the user's browsing history and bookmarks."),
                    new PropertyManager.AndroidPermission("android.permission.READ_LOGS", "Allows an application to read the low-level system log files."),
                    new PropertyManager.AndroidPermission("android.permission.READ_PROFILE", "Allows an application to read the user's personal profile data."),
                    new PropertyManager.AndroidPermission("android.permission.READ_SMS", "Allows an application to read SMS messages."),
                    new PropertyManager.AndroidPermission("android.permission.READ_SOCIAL_STREAM", "Allows an application to read from the user's social stream."),
                    new PropertyManager.AndroidPermission("android.permission.READ_SYNC_SETTINGS", "Allows applications to read the sync settings"),
                    new PropertyManager.AndroidPermission("android.permission.READ_SYNC_STATS", "Allows applications to read the sync stats"),
                    new PropertyManager.AndroidPermission("android.permission.READ_USER_DICTIONARY", "Allows an application to read the user dictionary."),
                    new PropertyManager.AndroidPermission("android.permission.REBOOT", "Required to be able to reboot the device."),
                    new PropertyManager.AndroidPermission("android.permission.RECEIVE_BOOT_COMPLETED", "Allows an application to receive the ACTION_BOOT_COMPLETED that is broadcast after the system finishes booting."),
                    new PropertyManager.AndroidPermission("android.permission.RECEIVE_MMS", "Allows an application to monitor incoming MMS messages, to record or perform processing on them."),
                    new PropertyManager.AndroidPermission("android.permission.RECEIVE_SMS", "Allows an application to monitor incoming SMS messages, to record or perform processing on them."),
                    new PropertyManager.AndroidPermission("android.permission.RECEIVE_WAP_PUSH", "Allows an application to monitor incoming WAP push messages."),
                    new PropertyManager.AndroidPermission("android.permission.REORDER_TASKS", "Allows an application to change the Z-order of tasks"),
                    new PropertyManager.AndroidPermission("android.permission.SEND_RESPOND_VIA_MESSAGE", "Allows an application (Phone) to send a request to other applications to handle the respond-via-message action during incoming calls."),
                    new PropertyManager.AndroidPermission("android.permission.SEND_SMS", "Allows an application to send SMS messages."),
                    new PropertyManager.AndroidPermission("android.permission.SET_ACTIVITY_WATCHER", "Allows an application to watch and control how activities are started globally in the system."),
                    new PropertyManager.AndroidPermission("android.permission.SET_ALARM", "Allows an application to broadcast an Intent to set an alarm for the user."),
                    new PropertyManager.AndroidPermission("android.permission.SET_ALWAYS_FINISH", "Allows an application to control whether activities are immediately finished when put in the background."),
                    new PropertyManager.AndroidPermission("android.permission.SET_ANIMATION_SCALE", "Modify the global animation scaling factor."),
                    new PropertyManager.AndroidPermission("android.permission.SET_DEBUG_APP", "Configure an application for debugging."),
                    new PropertyManager.AndroidPermission("android.permission.SET_ORIENTATION", "Allows low-level access to setting the orientation (actually rotation) of the screen."),
                    new PropertyManager.AndroidPermission("android.permission.SET_POINTER_SPEED", "Allows low-level access to setting the pointer speed."),
                    new PropertyManager.AndroidPermission("android.permission.SET_PROCESS_LIMIT", "Allows an application to set the maximum number of (not needed) application processes that can be running."),
                    new PropertyManager.AndroidPermission("android.permission.SET_TIME", "Allows applications to set the system time."),
                    new PropertyManager.AndroidPermission("android.permission.SET_TIME_ZONE", "Allows applications to set the system time zone"),
                    new PropertyManager.AndroidPermission("android.permission.SET_WALLPAPER", "Allows applications to set the wallpaper"),
                    new PropertyManager.AndroidPermission("android.permission.SET_WALLPAPER_HINTS", "Allows applications to set the wallpaper hints"),
                    new PropertyManager.AndroidPermission("android.permission.SIGNAL_PERSISTENT_PROCESSES", "Allow an application to request that a signal be sent to all persistent processes."),
                    new PropertyManager.AndroidPermission("android.permission.STATUS_BAR", "Allows an application to open, close, or disable the status bar and its icons."),
                    new PropertyManager.AndroidPermission("android.permission.SUBSCRIBED_FEEDS_READ", "Allows an application to allow access the subscribed feeds ContentProvider."),
                    new PropertyManager.AndroidPermission("android.permission.SUBSCRIBED_FEEDS_WRITE", ""),
                    new PropertyManager.AndroidPermission("android.permission.SYSTEM_ALERT_WINDOW", "Allows an application to open windows using the type TYPE_SYSTEM_ALERT, shown on top of all other applications."),
                    new PropertyManager.AndroidPermission("android.permission.TRANSMIT_IR", "Allows using the device's IR transmitter, if available"),
                    new PropertyManager.AndroidPermission("android.permission.UNINSTALL_SHORTCUT", "Allows an application to uninstall a shortcut in Launcher"),
                    new PropertyManager.AndroidPermission("android.permission.UPDATE_DEVICE_STATS", "Allows an application to update device statistics."),
                    new PropertyManager.AndroidPermission("android.permission.USE_CREDENTIALS", "Allows an application to request authtokens from the AccountManager"),
                    new PropertyManager.AndroidPermission("android.permission.USE_SIP", "Allows an application to use SIP service"),
                    new PropertyManager.AndroidPermission("android.permission.VIBRATE", "Allows access to the vibrator"),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_APN_SETTINGS", "Allows applications to write the apn settings."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_CALENDAR", "Allows an application to write (but not read) the user's calendar data."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_CALL_LOG", "Allows an application to write (but not read) the user's contacts data."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_CONTACTS", "Allows an application to write (but not read) the user's contacts data."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_GSERVICES", "Allows an application to modify the Google service map."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_HISTORY_BOOKMARKS", "Allows an application to write (but not read) the user's browsing history and bookmarks."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_PROFILE", "Allows an application to write (but not read) the user's personal profile data."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_SECURE_SETTINGS", "Allows an application to read or write the secure system settings."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_SETTINGS", "Allows an application to read or write the system settings."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_SMS", "Allows an application to write SMS messages."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_SOCIAL_STREAM", "Allows an application to write (but not read) the user's social stream data."),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_SYNC_SETTINGS", "Allows applications to write the sync settings"),
                    new PropertyManager.AndroidPermission("android.permission.WRITE_USER_DICTIONARY", "Allows an application to write to the user dictionary."),
                };
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
            if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V36)
            {
                WindowedPlatformsTabPage.Controls.Remove(DesktopResolutionLabel);
                WindowedPlatformsTabPage.Controls.Remove(DesktopResolutionCombo);
                IPhoneBasicSettingsPanel.Controls.Remove(IPhoneResolutionExcludeButton);
                IPhoneResolutionCombo.Width = IPhoneDeviceBehaviorGroup.Width - IPhoneResolutionCombo.Left - IPhoneResolutionCombo.Margin.Right;
            }
            else
            {
                DesktopResolutionCombo.Items.AddRange(new object[]
                    {
                        new ListItem(string.Empty, string.Empty),
                        new ListItem(TextHelper.GetString("DesktopResolution.Standard"), "standard"),
                        new ListItem(TextHelper.GetString("DesktopResolution.High"), "high")
                    });
            }
            if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V33)
            {
                IPhonePushNotifcationsCombo.Items.AddRange(new object[]
                    {
                        new ListItem(string.Empty, string.Empty),
                        new ListItem(TextHelper.GetString("IPhonePushNotifications.Development"), "development"),
                        new ListItem(TextHelper.GetString("IPhonePushNotifications.Distribution"), "production")
                    });
            }
            else
            {
                IPhoneBasicSettingsPanel.Controls.Remove(IPhoneOtherBehaviorGroup);
            }
            if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V32)
            {
                AspectRatioField.Items.Add(new ListItem(TextHelper.GetString("AspectRatio.Any"), "any"));
            }
            else
            {
                IPhoneBasicSettingsPanel.Controls.Remove(IPhoneBackgroundBehaviorGroup);
                IPhoneBasicSettingsPanel.Controls.Remove(MinimumiOsVersionLabel);
                IPhoneBasicSettingsPanel.Controls.Remove(MinimumiOsVersionField);
            }
            if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V32)
            {
                SupportedLanguagesLabel.Visible = false;
                SupportedLanguagesField.Visible = false;
                SupportedLanguagesButton.Visible = false;
                NonWindowedPlatformsTabPage.Controls.Remove(DepthStencilLabel);
                NonWindowedPlatformsTabPage.Controls.Remove(DepthStencilField);
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
            else
            {
                InitializeAndroidPermissions();
                foreach (var androidPermission in _androidPermissions)
                {
                    AndroidUserPermissionsList.Items.Add(androidPermission.Constant);
                }
                AndroidColorDepthCombo.Items.AddRange(new object[]
                    {
                        new ListItem(string.Empty, string.Empty),
                        new ListItem("16bit", "16bit"),
                        new ListItem("32bit", "32bit")
                    });
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
            else
            {
                IPhoneDeviceCombo.DropDownClosed += IPhoneDeviceCombo_DropDownClosed;
                IPhoneResolutionCombo.Items.AddRange(new object[]
                    {
                        new ListItem(string.Empty, string.Empty),
                        new ListItem(TextHelper.GetString("IPhoneResolution.Standard"), "standard"),
                        new ListItem(TextHelper.GetString("IPhoneResolution.High"), "high")
                    });
                IPhoneDeviceCombo.Items.AddRange(new object[]
                    {
                        new ListItem("iPhone/iPod", "1"), 
                        new ListItem("iPad", "2")
                    });
                IPhoneBGModesCombo.Items.AddRange(new object[]
                    {
                        new ListItem(TextHelper.GetString("IPhoneBGMode.Location"), "location"),
                        new ListItem(TextHelper.GetString("IPhoneBGMode.Audio"), "audio")
                    });
                IPhoneStatusBarStyleCombo.Items.AddRange(new object[]
                    {
                        new ListItem(string.Empty, string.Empty),
                        new ListItem("UIStatusBarStyleDefault", "UIStatusBarStyleDefault"),
                        new ListItem("UIStatusBarStyleLightContent", "UIStatusBarStyleLightContent"),
                        new ListItem("UIStatusBarStyleBlackTranslucent", "UIStatusBarStyleBlackTranslucent"),
                        new ListItem("UIStatusBarStyleBlackOpaque", "UIStatusBarStyleBlackOpaque")
                    });
                if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V26)
                {
                    IPhoneBasicSettingsPanel.Controls.Remove(IPhoneDeviceBehaviorGroup);
                } else if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V34)
                {
                    IPhoneBasicSettingsPanel.Controls.Remove(IPhoneOtherBehaviorGroup);
                }
                else if (PropertyManager.MajorVersion < PropertyManager.AirVersion.V37)
                {
                    IPhoneDeviceBehaviorGroup.Controls.Remove(IPhoneForceCPULabel);
                    IPhoneDeviceBehaviorGroup.Controls.Remove(IPhoneForceCPUField);
                    IPhoneDeviceBehaviorGroup.Controls.Remove(IPhoneForceCPUButton);
                    IPhoneOtherBehaviorGroup.Controls.Remove(IPhoneExternalSWFsLabel);
                    IPhoneOtherBehaviorGroup.Controls.Remove(IPhoneExternalSWFsField);
                    IPhoneOtherBehaviorGroup.Controls.Remove(IPhoneExternalSWFsButton);
                }
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
                        IPhoneInfoAdditionsField.Text = PropertyManager.GetProperty("iPhone/InfoAdditions");

                        // On older AIR versions adding your own entitlements was not allowed and you had to do it separatedly, which one? the one that added push support?
                        IPhoneEntitlementsField.Text = PropertyManager.GetProperty("iPhone/Entitlements");
                    }
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                    {
                        AndroidManifestAdditionsField.Text = PropertyManager.GetProperty("android/manifestAdditions");

                        PropertyManager.GetProperty("android/colorDepth", AndroidColorDepthCombo, 0);
                    }
                    if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V25)
                    {
                        PropertyManager.GetProperty("iPhone/requestedDisplayResolution", IPhoneResolutionCombo, 0);
                        PropertyManager.GetProperty("initialWindow/softKeyboardBehavior", SoftKeyboardField, 0);
                    }
                    if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V32)
                    {
                        PropertyManager.GetProperty("supportedLanguages", SupportedLanguagesField);
                        PropertyManager.GetProperty("initialWindow/depthAndStencil", DepthStencilField);
                    }
                    if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V35)
                    {
                        PropertyManager.GetProperty("initialWindow/requestedDisplayResolution", DesktopResolutionCombo, 0);
                        IPhoneResolutionExcludeButton.Tag = PropertyManager.GetAttribute("iPhone/requestedDisplayResolution/@excludeDevices");
                    }
                    if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V36)
                    {
                        PropertyManager.GetProperty("iPhone/forceCPURenderModeForDevices",IPhoneForceCPUField);
                        PropertyManager.GetProperty("iPhone/externalSwfs", IPhoneExternalSWFsField);
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
            foreach (String srcPath in PluginCore.PluginBase.CurrentProject.SourcePaths)
            {
                files = Directory.GetFiles(ProjectPaths.GetAbsolutePath(_propertiesFilePath, srcPath), "*-app.xml");
                foreach (String file in files)
                    if (LooksLikeAIRProperties(file)) return file;
            }
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

                    bool refreshProject = false;
                    var project = PluginCore.PluginBase.CurrentProject as ProjectManager.Projects.Project;
                    foreach (var extension in _extensions)
                    {
                        if (!string.IsNullOrEmpty(extension.Path))
                        {
                            if (!project.IsLibraryAsset(extension.Path))
                            {
                                refreshProject = true;  // Just in case in the future the following condition isn't anymore needed...
                                project.SetLibraryAsset(extension.Path, true);
                            }
                            var asset = project.GetAsset(extension.Path);
                            if (asset.SwfMode != SwfAssetMode.ExternalLibrary)
                            {
                                asset.SwfMode = SwfAssetMode.ExternalLibrary;
                                refreshProject = true;
                            }
                        }
                    }

                    foreach (var extension in _removedExtensions)
                    {
                        var asset = project.GetAsset(extension);
                        if (asset.SwfMode == SwfAssetMode.ExternalLibrary)
                        {
                            project.SetLibraryAsset(extension, false);
                            refreshProject = true;
                        }
                    }

                    if (refreshProject) project.Save();
                }
                // Mobile Additions tab
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V20)
                {
                    PropertyManager.SetPropertyCData("iPhone/InfoAdditions", IPhoneInfoAdditionsField);
                    PropertyManager.SetPropertyCData("iPhone/Entitlements", IPhoneEntitlementsField);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V25)
                {
                    PropertyManager.SetPropertyCData("android/manifestAdditions", AndroidManifestAdditionsField);
                    PropertyManager.SetProperty("android/colorDepth", AndroidColorDepthCombo);
                }
                if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V25)
                {
                    PropertyManager.SetProperty("iPhone/requestedDisplayResolution", IPhoneResolutionCombo);
                    PropertyManager.SetProperty("initialWindow/softKeyboardBehavior", SoftKeyboardField);
                }
                if (PropertyManager.MajorVersion >= PropertyManager.AirVersion.V32)
                {
                    PropertyManager.SetProperty("supportedLanguages", SupportedLanguagesField);
                    PropertyManager.SetProperty("initialWindow/depthAndStencil", DepthStencilField);
                }
                if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V35)
                {
                    PropertyManager.SetProperty("initialWindow/requestedDisplayResolution", DesktopResolutionCombo);
                    if (IPhoneResolutionExcludeButton.Enabled)
                        PropertyManager.SetAttribute("iPhone/requestedDisplayResolution/@excludeDevices", IPhoneResolutionExcludeButton.Tag as string);
                }
                if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V36)
                {
                    PropertyManager.SetProperty("iPhone/forceCPURenderModeForDevices", IPhoneForceCPUField);
                    PropertyManager.SetProperty("iPhone/externalSwfs", IPhoneExternalSWFsField);
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
                ExtensionIdField.ReadOnly = !string.IsNullOrEmpty(selectedExtension.Path);
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

        private void FilliPhoneAdditionsFields()
        {
            WizardHelper.SetControlValue(iPhoneAdditions.ValueOrNull("UIBackgroundModes") as string,
                                         IPhoneBGModesCombo);
            WizardHelper.SetControlValue(iPhoneAdditions.ValueOrNull("UIDeviceFamily") as IEnumerable,
                                         IPhoneDeviceCombo);
            WizardHelper.SetControlValue(iPhoneAdditions.ValueOrNull("UIApplicationExitsOnSuspend") as bool?,
                                         IPhoneExitsOnSuspendCheck);
            WizardHelper.SetControlValue(iPhoneAdditions.ValueOrNull("UIPrerenderedIcon") as string,
                                         IPhonePrerrenderedIconCheck);
            WizardHelper.SetControlValue(iPhoneAdditions.ValueOrNull("UIStatusBarStyle") as string,
                                         IPhoneStatusBarStyleCombo, 0);

            object minimumVersion;
            if (iPhoneAdditions.TryGetValue("MinimumOSVersion", out minimumVersion))
                MinimumiOsVersionField.Text = minimumVersion.ToString();

        }

        private void FilliPhoneEntitlementsFields()
        {
            if (PropertyManager.MajorVersion > PropertyManager.AirVersion.V33)
                WizardHelper.SetControlValue(iPhoneEntitlements.ValueOrNull("aps-environment") as string,
                     IPhonePushNotifcationsCombo, 0);

        }

        private void FillAndroidManifestFields()
        {
            MinimumAndroidOsField.Text = androidManifest.UsesSdk == null || androidManifest.UsesSdk.MinSdkVersion <= 0
                                             ? string.Empty : androidManifest.UsesSdk.MinSdkVersion.ToString();

            for (int i = 0, count = AndroidUserPermissionsList.Items.Count; i < count; i++)
            {
                AndroidUserPermissionsList.SetItemChecked(i,
                                                          androidManifest.UsesPermissions.ContainsName(
                                                              (string) AndroidUserPermissionsList.Items[i]));
            }
        }

        private static byte[] UnzipFile(ZipFile zfile, ZipEntry entry)
        {
            Stream stream = zfile.GetInputStream(entry);
            byte[] data = new byte[entry.Size];
            int length = stream.Read(data, 0, (int)entry.Size);
            if (length != entry.Size)
                throw new Exception("Corrupted archive");
            return data;
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

                if (!string.IsNullOrEmpty(selectedExtension.Path) && !_removedExtensions.Contains(selectedExtension.Path))
                    _removedExtensions.Add(selectedExtension.Path);
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

        private void AndroidManifestAdditionsButton_Click(object sender, EventArgs e)
        {
            if (AndroidAdvancedSettingsPanel.Visible)
            {
                if (ValidationErrorProvider.GetError(AndroidManifestAdditionsField) != string.Empty)
                    return;

                FillAndroidManifestFields();
                AndroidBasicSettingsPanel.Visible = true;
                AndroidAdvancedSettingsPanel.Visible = false;
                AndroidManifestAdditionsButton.Text = TextHelper.GetString("Label.Advanced");

            }
            else
            {
                if (ValidationErrorProvider.GetError(MinimumAndroidOsField) != string.Empty)
                    return;

                AndroidManifestAdditionsField_Validating(AndroidManifestAdditionsField, null);

                AndroidAdvancedSettingsPanel.Visible = true;
                AndroidBasicSettingsPanel.Visible = false;
                AndroidManifestAdditionsButton.Text = TextHelper.GetString("Label.Basic");
            }
        }

        private void AndroidUserPermissionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AndroidUserPermissionsList.SelectedItems.Count > 0)
            {
                AndroidPermissionsLabel.Text = _androidPermissions[AndroidUserPermissionsList.SelectedIndex].Description;
            }
            else
            {
                AndroidPermissionsLabel.Text = TextHelper.GetString("Label.SelectPermForInfo");
            }
        }

        private void IPhoneInfoAdditionsButton_Click(object sender, EventArgs e)
        {
            if (IPhoneAdvancedSettingsPanel.Visible)
            {
                if (ValidationErrorProvider.GetError(IPhoneInfoAdditionsField) != string.Empty || ValidationErrorProvider.GetError(IPhoneEntitlementsField) != string.Empty)
                    return;

                FilliPhoneAdditionsFields();
                FilliPhoneEntitlementsFields();
                IPhoneBasicSettingsPanel.Visible = true;
                IPhoneAdvancedSettingsPanel.Visible = false;
                IPhoneInfoAdditionsButton.Text = TextHelper.GetString("Label.Advanced");
            }
            else
            {
                IPhoneInfoAdditionsField_Validating(IPhoneEntitlementsField, null);
                IPhoneEntitlementsField_Validating(IPhoneEntitlementsField, null);
                
                IPhoneAdvancedSettingsPanel.Visible = true;
                IPhoneBasicSettingsPanel.Visible = false;
                IPhoneInfoAdditionsButton.Text = TextHelper.GetString("Label.Basic");
            }
        }

        private void IPhoneDeviceCombo_DropDownClosed(object sender, EventArgs e)
        {
            if (IPhoneDeviceCombo.CheckedItems.Count == 0)
            {
                String msg = TextHelper.GetString("Info.SelectAtleastOneItem");
                ErrorManager.ShowWarning(msg, null);
                IPhoneDeviceCombo.SetItemChecked(0, true);
            }
        }

        private void IPhoneResolutionExcludeButton_Click(object sender, EventArgs e)
        {
            string devicesData = (string)(IPhoneResolutionExcludeButton.Tag ?? string.Empty);
            string[] devices = devicesData.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            using (var iOSDevicesForm = new IOSDeviceManager(devices))
            {
                if (iOSDevicesForm.ShowDialog(this) == DialogResult.OK)
                    IPhoneResolutionExcludeButton.Tag = iOSDevicesForm.SelectedDevices;
            }
        }

        private void IPhoneForceCPUButton_Click(object sender, EventArgs e)
        {
            using (var iOSDevicesForm = new IOSDeviceManager(IPhoneForceCPUField.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (iOSDevicesForm.ShowDialog(this) == DialogResult.OK)
                    IPhoneForceCPUField.Text = iOSDevicesForm.SelectedDevices;
            }
        }

        private void IPhoneResolutionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            IPhoneResolutionExcludeButton.Enabled = IPhoneResolutionCombo.SelectedIndex > 0;
        }

        private void IPhoneExternalSWFsButton_Click(object sender, EventArgs e)
        {
            using (var externalsFileDialog = new OpenFileDialog())
            {
                externalsFileDialog.InitialDirectory = Path.GetDirectoryName(_propertiesFile);
                externalsFileDialog.CheckFileExists = true;
                if (externalsFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var externalsFile = ProjectPaths.GetRelativePath(_propertiesFilePath, externalsFileDialog.FileName);
                    if (externalsFile.StartsWith("..") || Path.IsPathRooted(externalsFile))
                    {
                        String msg = TextHelper.GetString("Info.CheckFileLocation");
                        ErrorManager.ShowWarning(msg, null);
                    }
                    else
                    {
                        IPhoneExternalSWFsField.Text = externalsFile.Replace('\\', '/');
                    }
                }
            }
        }

        private void ExtensionBrowseButton_Click(object sender, EventArgs e)
        {
            using (var extensionBrowser = new OpenFileDialog())
            {
                extensionBrowser.CheckFileExists = true;
                extensionBrowser.Filter = TextHelper.GetString("Info.AneFilter");
                extensionBrowser.InitialDirectory = _propertiesFilePath;

                if (extensionBrowser.ShowDialog(this) == DialogResult.OK)
                {
                    ZipFile zFile;
                    try
                    {
                        zFile = new ZipFile(extensionBrowser.FileName);
                    }
                    catch (Exception ex)
                    {
                        String msg = TextHelper.GetString("Info.CouldNotLoadANE") + "\n" + ex.Message;
                        ErrorManager.ShowWarning(msg, null);
                        return;
                    }
                    var entry = zFile.GetEntry("META-INF/ANE/extension.xml");

                    if (entry == null)
                    {
                        String msg = TextHelper.GetString("Info.ANEDescFileNotFound");
                        ErrorManager.ShowWarning(msg, null);
                        return;
                    }

                    byte[] buffer = UnzipFile(zFile, entry);

                    string extensionId = null;
                    using (var stream = new MemoryStream(buffer))
                    {
                        using (var reader = XmlReader.Create(stream))
                        {
                            reader.MoveToContent();

                            while (reader.Read())
                            {
                                if (reader.NodeType != XmlNodeType.Element) continue;

                                if (reader.Name == "id")
                                {
                                    extensionId = reader.ReadInnerXml();
                                    break;
                                }
                            }
                        }
                    }

                    if (extensionId == null)
                    {
                        String msg = TextHelper.GetString("Info.ExtensionIDNotFound");
                        ErrorManager.ShowWarning(msg, null);
                        return;
                    }

                    PropertyManager.AirExtension extension = null;

                    // We look for the extension in case it is already added, and modify its path, maybe FD is missing the external library entry and the user
                    //wants to add it.
                    foreach (var existingExtension in _extensions)
                    {
                        if (existingExtension.ExtensionId == extensionId) extension = existingExtension;
                        break;
                    }
                    if (extension == null)
                    {
                        extension = new PropertyManager.AirExtension() { ExtensionId = extensionId, IsValid = true };
                        _extensions.Add(extension);
                        //I don't validation and selection is needed in this case
                        var extensionListItem = new ListViewItem(extension.ExtensionId);
                        ExtensionsListView.Items.Add(extensionListItem);
                    }
                    extension.Path = extensionBrowser.FileName;
                }
            }
        }

        private void RenderModeField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DepthStencilField.Parent == null) return;

            DepthStencilField.Enabled = ((ListItem)RenderModeField.SelectedItem).Value == "direct";
        }

        private void AppPropertiesTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AppPropertiesTabControl.SelectedTab == ExtensionsTabPage && _isPropertiesLoaded)
            {
                bool reloadSelected = false;
                foreach (PropertyManager.AirExtension extension in _extensions)
                {
                    if (extension.IsValid && string.IsNullOrEmpty(extension.Path))
                    {
                        var project = PluginCore.PluginBase.CurrentProject as ProjectManager.Projects.Project;
                        foreach (var externalPath in (project.CompilerOptions as MxmlcOptions).ExternalLibraryPaths)
                        {
                            if (Path.GetExtension(externalPath).ToUpperInvariant() == ".ANE")
                            {

                                string absolutePath = project.GetAbsolutePath(externalPath);
                                ZipFile zFile;
                                try
                                {
                                    zFile = new ZipFile(absolutePath);
                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                                var entry = zFile.GetEntry("META-INF/ANE/extension.xml");

                                if (entry == null)
                                {
                                    continue;
                                }

                                byte[] buffer = UnzipFile(zFile, entry);

                                string extensionId = null;
                                using (var stream = new MemoryStream(buffer))
                                {
                                    using (var reader = XmlReader.Create(stream))
                                    {
                                        reader.MoveToContent();

                                        while (reader.Read())
                                        {
                                            if (reader.NodeType != XmlNodeType.Element) continue;

                                            if (reader.Name == "id")
                                            {
                                                extensionId = reader.ReadInnerXml();
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (extensionId == extension.ExtensionId)
                                {
                                    reloadSelected = true;
                                    extension.Path = absolutePath;
                                }
                            }
                        }
                    }
                }

                if (reloadSelected) // In this case, does selecting the first item by default really improve UX in any significant way?
                    LoadSelectedExtension();

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
                if (fileType.Name.Length == 0) fileType.NameIsValid = false;
                else fileType.NameIsValid = true;
                if (!Regex.IsMatch(fileType.Extension, _FileNameRegexPattern)) fileType.ExtensionIsValid = false;
                else fileType.ExtensionIsValid = true;
                if (fileType.ContentType.Length == 0) fileType.ContentTypeIsValid = false;
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
            if (VersionField.Text.Length == 0 && PropertyManager.MajorVersion < PropertyManager.AirVersion.V25)
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
                if (FileTypeNameField.Text.Length == 0)
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
                if (FileTypeContentTypeField.Text.Length == 0 && PropertyManager.MajorVersion >= PropertyManager.AirVersion.V15)
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

        private void IPhoneExternalSWFsField_Validating(object sender, CancelEventArgs e)
        {
            var externalsFile = IPhoneExternalSWFsField.Text;
            if (externalsFile == string.Empty) this.ValidationErrorProvider.SetError(IPhoneExternalSWFsField, string.Empty);
            else if (externalsFile.StartsWith("..") || Path.IsPathRooted(externalsFile))
            {
                this.ValidationErrorProvider.SetError(IPhoneExternalSWFsField, TextHelper.GetString("Info.CheckFileLocation"));

                e.Cancel = true;
            }
            else if (!File.Exists(ProjectPaths.GetAbsolutePath(Path.GetDirectoryName(_propertiesFile), externalsFile)))
            {
                this.ValidationErrorProvider.SetError(IPhoneExternalSWFsField, TextHelper.GetString("Validation.MissingSWFContainer"));

                e.Cancel = true;
            }
            else this.ValidationErrorProvider.SetError(IPhoneExternalSWFsField, string.Empty);

        }

        private void IPhoneEntitlementsField_Validating(object sender, CancelEventArgs e)
        {
            bool fillUi = iPhoneEntitlements == null;
            if (IPhoneAdvancedSettingsPanel.Visible || fillUi)
            {
                try
                {
                    iPhoneEntitlements =
                        new IphonePlistManager("<Entitlements>" + IPhoneEntitlementsField.Text + "</Entitlements>") { RemoveOnNullValue = true };
                    ValidationErrorProvider.SetError(IPhoneEntitlementsField, string.Empty);

                    if (fillUi)
                        FilliPhoneEntitlementsFields();
                }
                catch (Exception ex)
                {
                    ValidationErrorProvider.SetError(IPhoneEntitlementsField, TextHelper.GetString("Validation.WrongEntitlementsXML") + " " + ex.Message);
                    e.Cancel = true;
                }
            }
            else
            {
                iPhoneEntitlements["aps-environment"] = IPhonePushNotifcationsCombo.SelectedIndex == 0 ? null : ((ListItem)IPhonePushNotifcationsCombo.SelectedItem).Value;

                IPhoneEntitlementsField.Text = iPhoneEntitlements.GetPlistXml();
            }
        }

        private void IPhoneInfoAdditionsField_Validating(object sender, CancelEventArgs e)
        {
            bool fillUi = iPhoneAdditions == null;
            if (IPhoneAdvancedSettingsPanel.Visible || fillUi)
            {
                try
                {
                    iPhoneAdditions =
                        new IphonePlistManager("<InfoAdditions>" + IPhoneInfoAdditionsField.Text + "</InfoAdditions>") { RemoveOnNullValue = true };
                    ValidationErrorProvider.SetError(IPhoneInfoAdditionsField, string.Empty);

                    if (fillUi)
                        FilliPhoneAdditionsFields();
                }
                catch (Exception ex)
                {
                    ValidationErrorProvider.SetError(IPhoneInfoAdditionsField, TextHelper.GetString("Validation.WrongAdditionsXML") + " " + ex.Message);
                    e.Cancel = true;
                }
            }
            else
            {
                List<string> selectedValues = null;
                foreach (ListItem item in IPhoneBGModesCombo.CheckedItems)
                {
                    selectedValues = selectedValues ?? new List<string>();
                    selectedValues.Add(item.Value);
                }
                iPhoneAdditions["UIBackgroundModes"] = selectedValues;
                selectedValues = null;
                foreach (ListItem item in IPhoneDeviceCombo.CheckedItems)
                {
                    selectedValues = selectedValues ?? new List<string>();
                    selectedValues.Add(item.Value);
                }
                iPhoneAdditions["UIDeviceFamily"] = selectedValues;
                iPhoneAdditions["UIApplicationExitsOnSuspend"] = IPhoneExitsOnSuspendCheck.CheckState == CheckState.Indeterminate ? null : (bool?)IPhoneExitsOnSuspendCheck.Checked;
                iPhoneAdditions["UIPrerenderedIcon"] = IPhonePrerrenderedIconCheck.CheckState == CheckState.Indeterminate ? null : (bool?)IPhonePrerrenderedIconCheck.Checked;
                iPhoneAdditions["UIStatusBarStyle"] = IPhoneStatusBarStyleCombo.SelectedIndex == 0 ? null : ((ListItem)IPhoneStatusBarStyleCombo.SelectedItem).Value;
                iPhoneAdditions["MinimumOSVersion"] = MinimumiOsVersionField.Text == string.Empty ? null : MinimumiOsVersionField.Text;

                IPhoneInfoAdditionsField.Text = iPhoneAdditions.GetPlistXml();
            }
        }

        private void AndroidManifestAdditionsField_Validating(object sender, CancelEventArgs e)
        {
            bool fillUi = androidManifest == null;
            if (AndroidAdvancedSettingsPanel.Visible || fillUi)
            {
                try
                {
                    androidManifest =
                        new AndroidManifestManager(AndroidManifestAdditionsField.Text);
                    ValidationErrorProvider.SetError(AndroidManifestAdditionsField, string.Empty);

                    if (fillUi)
                        FillAndroidManifestFields();
                }
                catch (Exception ex)
                {
                    ValidationErrorProvider.SetError(AndroidManifestAdditionsField, TextHelper.GetString("Validation.WrongManifestXML") + " " + ex.Message);
                    e.Cancel = true;
                }
            }
            else
            {
                foreach (string item in AndroidUserPermissionsList.Items)
                {
                    if (AndroidUserPermissionsList.CheckedItems.Contains(item))
                    {
                        if (!androidManifest.UsesPermissions.ContainsName(item))
                            androidManifest.UsesPermissions.Add(new AndroidManifestManager.UsesPermissionElement() { Name = item });
                    }
                    else if (androidManifest.UsesPermissions.ContainsName(item))
                    {
                        androidManifest.UsesPermissions.RemoveByName(item);
                    }
                }

                if (MinimumAndroidOsField.Text == string.Empty)
                    androidManifest.UsesSdk = null;
                else
                {
                    var usesSdk = androidManifest.UsesSdk ?? new AndroidManifestManager.UsesSdkElement();
                    usesSdk.MinSdkVersion = int.Parse(MinimumAndroidOsField.Text);
                    androidManifest.UsesSdk = usesSdk;
                }

                AndroidManifestAdditionsField.Text = androidManifest.GetManifestXml();
            }
        }

        private void MinimumiOsVersionField_Validating(object sender, CancelEventArgs e)
        {
            if (MinimumiOsVersionField.Text != string.Empty)
            {
                try
                {
                    var version = new Version(MinimumiOsVersionField.Text);
                    if (version.Major > 4 || version.Minor > 2)
                        this.ValidationErrorProvider.SetError(MinimumiOsVersionField, string.Empty);
                    else
                    {
                        e.Cancel = true;
                        // I think it's actually 4.2, but people should nowadays target 4.3 as a minimum
                        this.ValidationErrorProvider.SetError(MinimumiOsVersionField, TextHelper.GetString("Validation.InvalidMinAIRVersion"));
                    }
                }
                catch
                {
                    e.Cancel = true;
                    this.ValidationErrorProvider.SetError(MinimumiOsVersionField, TextHelper.GetString("Validation.InvalidVersionValue"));
                }

            }
            else
            {
                this.ValidationErrorProvider.SetError(MinimumiOsVersionField, string.Empty);
            }
        }

        private void MinimumAndroidOsField_Validating(object sender, CancelEventArgs e)
        {
            int osVersion;
            if (MinimumAndroidOsField.Text == string.Empty)
            {
                this.ValidationErrorProvider.SetError(MinimumAndroidOsField, string.Empty);

                return;
            }
            if (!int.TryParse(MinimumAndroidOsField.Text, out osVersion))
            {
                e.Cancel = true;
                this.ValidationErrorProvider.SetError(MinimumAndroidOsField, TextHelper.GetString("Validation.InvalidVersionValue"));
            }
            else if (osVersion < 8) // IMHO, it should be 9, as 8 nowadays may give some problems
            {
                e.Cancel = true;
                this.ValidationErrorProvider.SetError(MinimumAndroidOsField, TextHelper.GetString("Validation.InvalidAndroidMinVersion"));
            }
            else
            {
                this.ValidationErrorProvider.SetError(MinimumAndroidOsField, string.Empty);
            }
        }

        #endregion

    }

}

