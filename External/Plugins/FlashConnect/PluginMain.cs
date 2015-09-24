using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace FlashConnect
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "FlashConnect";
        private String pluginGuid = "425ae753-fdc2-4fdf-8277-c47c39c2e26b";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds a xml socket to FlashDevelop that let's you trace messages from outside of FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private String settingFilename;
        private Settings settingObject;
        private XmlSocket xmlSocket;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return this.pluginGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary>
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.SetupSocket();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            // Nothing to do here..
        }

        #endregion
        
        #region Custom Methods
        
        // Response messages and errors
        private readonly Byte[] RESULT_INVALID = Encoding.Default.GetBytes("<flashconnect status=\"1\"/>\0");
        private readonly Byte[] RESULT_NOTFOUND = Encoding.Default.GetBytes("<flashconnect status=\"2\"/>\0");
        private readonly Exception INVALID_MSG = new Exception(TextHelper.GetString("Info.InvalidMessage"));

        /// <summary>
        /// Sets up the basic stuff
        /// </summary> 
        private void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "FlashConnect");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Setups the socket connection
        /// </summary> 
        private void SetupSocket()
        {
            if (this.settingObject.Enabled && !SingleInstanceApp.AlreadyExists)
            {
                this.xmlSocket = new XmlSocket(this.settingObject.Host, this.settingObject.Port);
                this.xmlSocket.XmlReceived += new XmlReceivedEventHandler(this.HandleXml);
            }
        }
        
        /// <summary>
        /// Handles the incoming xml message
        /// </summary>
        public void HandleXml(Object sender, XmlReceivedEventArgs e)
        {
            if (PluginBase.MainForm.MenuStrip.InvokeRequired) PluginBase.MainForm.MenuStrip.BeginInvoke((MethodInvoker)delegate
            {
                try
                {
                    XmlDocument message = e.XmlDocument;
                    XmlNode mainNode = message.FirstChild;
                    for (Int32 i = 0; i < mainNode.ChildNodes.Count; i++)
                    {
                        XmlNode cmdNode = mainNode.ChildNodes[i];
                        if (XmlHelper.HasAttribute(cmdNode, "cmd"))
                        {
                            String cmd = XmlHelper.GetAttribute(cmdNode, "cmd");
                            switch (cmd)
                            {
                                case "call":
                                    this.HandleCallMsg(cmdNode, e.Socket);
                                    break;
                                case "trace":
                                    this.HandleTraceMsg(cmdNode, e.Socket);
                                    break;
                                case "notify":
                                    this.HandleNotifyMsg(cmdNode, e.Socket);
                                    break;
                                case "return":
                                    this.HandleReturnMsg(cmdNode, e.Socket);
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
            });
        }

        /// <summary>
        /// Handles the call message
        /// </summary>
        public void HandleCallMsg(XmlNode msgNode, Socket client)
        {
            try
            {
                String command = XmlHelper.GetAttribute(msgNode, "command");
                String arguments = HttpUtility.UrlDecode(XmlHelper.GetValue(msgNode));
                if (this.settingObject.Commands.Contains(command))
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
                String message = HttpUtility.UrlDecode(XmlHelper.GetValue(msgNode));
                Int32 state = Convert.ToInt32(XmlHelper.GetAttribute(msgNode, "state"));
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
            String message;
            String guid;
            IPlugin plugin;
            try 
            {
                message = HttpUtility.UrlDecode(XmlHelper.GetValue(msgNode));
                guid = XmlHelper.GetAttribute(msgNode, "guid");
                plugin = PluginBase.MainForm.FindPlugin(guid);
                if (plugin != null)
                {
                    DataEvent de = new DataEvent(EventType.Command, "FlashConnect", message);
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
                Byte[] data = Encoding.ASCII.GetBytes(msgNode.InnerXml + "\0");
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
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            if (this.settingObject.Commands.Count == 0)
            {
                this.settingObject.Commands.Add("Edit");
                this.settingObject.Commands.Add("Browse");
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        #endregion
    
    }
    
}
