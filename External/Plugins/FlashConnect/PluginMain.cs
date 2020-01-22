// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Net.Sockets;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

namespace FlashConnect
{
    public class PluginMain : IPlugin
    {
        private string settingFilename;
        private Settings settingObject;
        private XmlSocket xmlSocket;
        private Timer pendingSetup;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = nameof(FlashConnect);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "425ae753-fdc2-4fdf-8277-c47c39c2e26b";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "Adds a xml socket to FlashDevelop that let's you trace messages from outside of FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => settingObject;

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            SetupSocket();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            if (pendingSetup != null)
            {
                pendingSetup.Stop();
                pendingSetup = null;
            }
            SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            // Nothing to do here..
        }

        #endregion

        #region Custom Methods

        // Response messages and errors
        private readonly byte[] RESULT_INVALID = Encoding.Default.GetBytes("<flashconnect status=\"1\"/>\0");
        private readonly byte[] RESULT_NOTFOUND = Encoding.Default.GetBytes("<flashconnect status=\"2\"/>\0");
        private readonly Exception INVALID_MSG = new Exception(TextHelper.GetString("Info.InvalidMessage"));

        /// <summary>
        /// Sets up the basic stuff
        /// </summary> 
        private void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(FlashConnect));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Setups the socket connection
        /// </summary> 
        private void SetupSocket()
        {
            pendingSetup = new Timer();
            pendingSetup.Interval = 5000;
            pendingSetup.Tick += (sender, e) =>
            {
                pendingSetup.Stop();
                pendingSetup = null;
                if (settingObject.Enabled && !SingleInstanceApp.AlreadyExists)
                {
                    xmlSocket = new XmlSocket(settingObject.Host, settingObject.Port);
                    xmlSocket.XmlReceived += HandleXml;
                }
            };
            pendingSetup.Start();
        }

        /// <summary>
        /// Handles the incoming xml message
        /// </summary>
        public void HandleXml(object sender, XmlReceivedEventArgs e)
        {
            if (PluginBase.MainForm.MenuStrip.InvokeRequired) PluginBase.MainForm.MenuStrip.BeginInvoke((MethodInvoker)(
                () =>
                {
                    try
                    {
                        var message = e.XmlDocument;
                        var mainNode = message.FirstChild;
                        for (int i = 0; i < mainNode.ChildNodes.Count; i++)
                        {
                            XmlNode cmdNode = mainNode.ChildNodes[i];
                            if (XmlHelper.HasAttribute(cmdNode, "cmd"))
                            {
                                string cmd = XmlHelper.GetAttribute(cmdNode, "cmd");
                                switch (cmd)
                                {
                                    case "call":
                                        HandleCallMsg(cmdNode, e.Socket);
                                        break;
                                    case "trace":
                                        HandleTraceMsg(cmdNode, e.Socket);
                                        break;
                                    case "notify":
                                        HandleNotifyMsg(cmdNode, e.Socket);
                                        break;
                                    case "return":
                                        HandleReturnMsg(cmdNode, e.Socket);
                                        break;
                                    default:
                                        ErrorManager.ShowError(INVALID_MSG);
                                        break;
                                }
                            }
                            else ErrorManager.ShowError(INVALID_MSG);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                    }
                }));
        }

        /// <summary>
        /// Handles the call message
        /// </summary>
        public void HandleCallMsg(XmlNode msgNode, Socket client)
        {
            try
            {
                string command = XmlHelper.GetAttribute(msgNode, "command");
                string arguments = HttpUtility.UrlDecode(XmlHelper.GetValue(msgNode));
                if (settingObject.Commands.Contains(command))
                {
                    PluginBase.MainForm.CallCommand(command, arguments);
                }
            }
            catch
            {
                client.Send(RESULT_INVALID);
            }
        }

        /// <summary>
        /// Handles the trace message
        /// </summary>
        public void HandleTraceMsg(XmlNode msgNode, Socket client)
        {
            try
            {
                var message = HttpUtility.UrlDecode(XmlHelper.GetValue(msgNode));
                var state = Convert.ToInt32(XmlHelper.GetAttribute(msgNode, "state"));
                TraceManager.Add(message, state);
            }
            catch
            {
                client.Send(RESULT_INVALID);
            }
        }

        /// <summary>
        /// Handles the notify message
        /// </summary>
        public void HandleNotifyMsg(XmlNode msgNode, Socket client)
        {
            try
            {
                var message = HttpUtility.UrlDecode(XmlHelper.GetValue(msgNode));
                var guid = XmlHelper.GetAttribute(msgNode, "guid");
                var plugin = PluginBase.MainForm.FindPlugin(guid);
                if (plugin != null)
                {
                    var de = new DataEvent(EventType.Command, "FlashConnect", message);
                    plugin.HandleEvent(client, de, HandlingPriority.High);
                }
                else client.Send(RESULT_NOTFOUND);
            }
            catch
            {
                client.Send(RESULT_INVALID);
            }
        }

        /// <summary>
        /// Handles the return message
        /// </summary>
        public void HandleReturnMsg(XmlNode msgNode, Socket client)
        {
            try
            {
                var data = Encoding.ASCII.GetBytes(msgNode.InnerXml + "\0");
                client.Send(data);
            }
            catch
            {
                client.Send(RESULT_INVALID);
            }
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
            if (settingObject.Commands.Count != 0) return;
            settingObject.Commands.Add("Edit");
            settingObject.Commands.Add("Browse");
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        #endregion
    }
}