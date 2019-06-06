using System;
using System.Collections.Generic;
using System.ComponentModel;
using PluginCore.Localization;

namespace FlashConnect
{
    [Serializable]
    public class Settings
    {
        private int port = 1978;
        private string host = "127.0.0.1";
        private bool enabled = true;
        private List<string> commands = new List<string> { "Edit", "Browse" };

        /// <summary> 
        /// Get and sets the enabled
        /// </summary>
        [LocalizedDescription("FlashConnect.Description.Enabled"), DefaultValue(true)]
        public bool Enabled
        {
            get { return this.enabled; }
            set { this.enabled = value; }
        }

        /// <summary> 
        /// Get and sets the address
        /// </summary>
        [LocalizedDescription("FlashConnect.Description.Host"), DefaultValue("127.0.0.1")]
        public string Host
        {
            get { return this.host; }
            set { this.host = value; }
        }

        /// <summary> 
        /// Get and sets the port
        /// </summary>
        [LocalizedDescription("FlashConnect.Description.Port"), DefaultValue(1978)]
        public int Port
        {
            get { return this.port; }
            set { this.port = value; }
        }

        /// <summary> 
        /// Get and sets the allowed commands
        /// </summary>
        [LocalizedDescription("FlashConnect.Description.Commands")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<string> Commands
        {
            get { return this.commands; }
            set { this.commands = value; }
        }

    }

}
