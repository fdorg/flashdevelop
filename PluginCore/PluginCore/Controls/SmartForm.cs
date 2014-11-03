using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore.Helpers;

namespace PluginCore.Controls
{
    public class SmartForm : Form
    {
        private String formGuid;
        private String helpLink;
        private FormProps formProps;
        public event SavePropsHandler SaveProps;
        public event ApplyPropsHandler ApplyProps;
        public delegate void ApplyPropsHandler(SmartForm form);
        public delegate void SavePropsHandler(SmartForm form);

        public SmartForm()
        {
            this.formProps = new FormProps();
            this.Load += new EventHandler(this.SmartFormLoad);
            this.Shown += new EventHandler(this.SmartFormShown);
            this.FormClosed += new FormClosedEventHandler(this.SmartFormClosed);
        }

        /// <summary>
        /// Gets or sets the help link
        /// </summary>
        public String HelpLink
        {
            get { return this.helpLink; }
            set { this.helpLink = value; }
        }

        /// <summary>
        /// Gets or sets the form guid
        /// </summary>
        public String FormGuid
        {
            get { return this.formGuid; }
            set { this.formGuid = value.ToUpper(); }
        }

        /// <summary>
        /// Path to the unique setting file
        /// </summary>
        private String FormPropsFile
        {
            get { return Path.Combine(this.FormStatesDir, this.formGuid + ".fdb"); }
        }

        /// <summary>
        /// Path to the form state file directory
        /// </summary>
        private String FormStatesDir
        {
            get
            {
                String settingDir = PathHelper.SettingDir;
                String formStatesDir = Path.Combine(settingDir, "FormStates");
                if (!Directory.Exists(formStatesDir)) Directory.CreateDirectory(formStatesDir);
                return formStatesDir;
            }
        }

        /// <summary>
        /// Center the dialog to parent if requested
        /// </summary>
        private void SmartFormShown(Object sender, EventArgs e)
        {
            if (this.StartPosition == FormStartPosition.CenterParent)
            {
                this.CenterToParent();
            }
        }

        /// <summary>
        /// Load the form state from a setting file and applies it
        /// </summary>
        private void SmartFormLoad(Object sender, EventArgs e)
        {
            ScaleHelper.AdjustForHighDPI(this);
            if (!String.IsNullOrEmpty(this.formGuid) && File.Exists(this.FormPropsFile))
            {
                Object obj = ObjectSerializer.Deserialize(this.FormPropsFile, this.formProps);
                this.formProps = (FormProps)obj;
                if (!this.formProps.WindowSize.IsEmpty)
                {
                    this.Size = this.formProps.WindowSize;
                }
            }
            if (!String.IsNullOrEmpty(this.helpLink))
            {
                this.HelpButton = true;
                this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SmartFormHelpButtonClick);
            }
            if (this.ApplyProps != null) this.ApplyProps(this);
        }

        /// <summary>
        /// Saves the current form state to a setting file
        /// </summary>
        private void SmartFormClosed(Object sender, FormClosedEventArgs e)
        {
            if (this.SaveProps != null) this.SaveProps(this);
            if (!String.IsNullOrEmpty(this.formGuid) && !this.Size.IsEmpty)
            {
                this.formProps.WindowSize = this.Size;
                ObjectSerializer.Serialize(this.FormPropsFile, this.formProps);
            }
        }

        /// <summary>
        /// Browse to the specified help link
        /// </summary>
        private void SmartFormHelpButtonClick(Object sender, CancelEventArgs e)
        {
            PluginBase.MainForm.CallCommand("Browse", this.helpLink);
        }

        /// <summary>
        /// Get custom property value
        /// </summary>
        public String GetPropValue(String key)
        {
            Argument argument;
            for (var i = 0; i < this.formProps.ExtraProps.Count; i++)
            {
                argument = this.formProps.ExtraProps[i];
                if (argument.Key == key) return argument.Value;
            }
            return null;
        }

        /// <summary>
        /// Set custom property value
        /// </summary>
        public void SetPropValue(String key, String value)
        {
            Argument argument;
            for (var i = 0; i < this.formProps.ExtraProps.Count; i++)
            {
                argument = this.formProps.ExtraProps[i];
                if (argument.Key == key)
                {
                    argument.Value = value;
                    return;
                }
            }
            argument = new Argument(key, value);
            this.formProps.ExtraProps.Add(argument);
        }
    }

    [Serializable]
    public class FormProps
    {
        public Size WindowSize;
        public List<Argument> ExtraProps;

        public FormProps()
        {
            this.WindowSize = Size.Empty;
            this.ExtraProps = new List<Argument>();
        }
    }

}
