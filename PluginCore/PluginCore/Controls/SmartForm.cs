using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public class SmartForm : FormEx, IEventHandler
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
            this.FormClosed += new FormClosedEventHandler(this.SmartFormClosed);
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
            ScaleHelper.AdjustForHighDPI(this);
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
        /// Gets or sets the help link
        /// </summary>
        public override Boolean UseTheme
        {
            get { return PluginBase.MainForm.GetThemeFlag("SmartForm.UseTheme", false); }
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
                String formStatesDir = Path.Combine(PathHelper.SettingDir, "FormStates");
                if (!Directory.Exists(formStatesDir)) Directory.CreateDirectory(formStatesDir);
                return formStatesDir;
            }
        }

        /// <summary>
        /// Apply theming properties to the controls
        /// </summary>
        private void ApplyTheming()
        {
            PluginBase.MainForm.SetUseTheme(this, this.UseTheme);
            if (this.UseTheme)
            {
                ScrollBarEx.Attach(this, true);
                PluginBase.MainForm.ThemeControls(this);
            }
        }

        /// <summary>
        /// Handles the incoming theming change event and updates.
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme) this.ApplyTheming();
        }

        /// <summary>
        /// Load the form state from a setting file and applies it
        /// </summary>
        private void SmartFormLoad(Object sender, EventArgs e)
        {
            this.ApplyTheming();
            if (this.StartPosition == FormStartPosition.CenterParent)
            {
                this.CenterToParent();
            }
            if (!String.IsNullOrEmpty(this.formGuid) && File.Exists(this.FormPropsFile))
            {
                Object obj = ObjectSerializer.Deserialize(this.FormPropsFile, this.formProps);
                this.formProps = (FormProps)obj;
                if (!this.formProps.WindowSize.IsEmpty && this.FormBorderStyle == FormBorderStyle.Sizable)
                {
                    this.Size = this.formProps.WindowSize;
                }
            }
            if (!String.IsNullOrEmpty(this.helpLink))
            {
                this.HelpButton = true;
                this.HelpButtonClicked += new CancelEventHandler(this.SmartFormHelpButtonClick);
            }
            ApplyProps?.Invoke(this);
        }

        /// <summary>
        /// Saves the current form state to a setting file
        /// </summary>
        private void SmartFormClosed(Object sender, FormClosedEventArgs e)
        {
            SaveProps?.Invoke(this);
            if (!String.IsNullOrEmpty(this.formGuid) && !this.Size.IsEmpty && this.FormBorderStyle == FormBorderStyle.Sizable)
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
            for (var i = 0; i < this.formProps.ExtraProps.Count; i++)
            {
                var argument = this.formProps.ExtraProps[i];
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
