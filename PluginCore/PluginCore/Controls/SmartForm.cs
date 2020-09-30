using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public class SmartForm : FormEx, IEventHandler
    {
        string formGuid;
        FormProps formProps;
        public event SavePropsHandler SaveProps;
        public event ApplyPropsHandler ApplyProps;
        public delegate void ApplyPropsHandler(SmartForm form);
        public delegate void SavePropsHandler(SmartForm form);

        public SmartForm()
        {
            formProps = new FormProps();
            Load += SmartFormLoad;
            FormClosed += SmartFormClosed;
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
            ScaleHelper.AdjustForHighDPI(this);
        }

        /// <summary>
        /// Gets or sets the help link
        /// </summary>
        public string HelpLink { get; set; }

        /// <summary>
        /// Gets or sets the form guid
        /// </summary>
        public string FormGuid
        {
            get => formGuid;
            set => formGuid = value.ToUpper();
        }

        /// <summary>
        /// Gets or sets the help link
        /// </summary>
        public override bool UseTheme => PluginBase.MainForm.GetThemeFlag("SmartForm.UseTheme", false);

        /// <summary>
        /// Path to the unique setting file
        /// </summary>
        string FormPropsFile => Path.Combine(FormStatesDir, formGuid + ".fdb");

        /// <summary>
        /// Path to the form state file directory
        /// </summary>
        static string FormStatesDir
        {
            get
            {
                var path = Path.Combine(PathHelper.SettingDir, "FormStates");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// Apply theming properties to the controls
        /// </summary>
        void ApplyTheming()
        {
            PluginBase.MainForm.SetUseTheme(this, UseTheme);
            if (!UseTheme) return;
            ScrollBarEx.Attach(this, true);
            PluginBase.MainForm.ThemeControls(this);
        }

        /// <summary>
        /// Handles the incoming theming change event and updates.
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme) ApplyTheming();
        }

        /// <summary>
        /// Load the form state from a setting file and applies it
        /// </summary>
        void SmartFormLoad(object sender, EventArgs e)
        {
            ApplyTheming();
            if (StartPosition == FormStartPosition.CenterParent)
            {
                CenterToParent();
            }
            if (!string.IsNullOrEmpty(formGuid) && File.Exists(FormPropsFile))
            {
                formProps = ObjectSerializer.Deserialize(FormPropsFile, formProps);
                if (!formProps.WindowSize.IsEmpty && FormBorderStyle == FormBorderStyle.Sizable)
                {
                    Size = formProps.WindowSize;
                }
            }
            if (!string.IsNullOrEmpty(HelpLink))
            {
                HelpButton = true;
                HelpButtonClicked += SmartFormHelpButtonClick;
            }
            ApplyProps?.Invoke(this);
        }

        /// <summary>
        /// Saves the current form state to a setting file
        /// </summary>
        void SmartFormClosed(object sender, FormClosedEventArgs e)
        {
            SaveProps?.Invoke(this);
            if (!string.IsNullOrEmpty(formGuid) && !Size.IsEmpty && FormBorderStyle == FormBorderStyle.Sizable)
            {
                formProps.WindowSize = Size;
                ObjectSerializer.Serialize(FormPropsFile, formProps);
            }
        }

        /// <summary>
        /// Browse to the specified help link
        /// </summary>
        void SmartFormHelpButtonClick(object sender, CancelEventArgs e) => PluginBase.MainForm.CallCommand("Browse", HelpLink);

        /// <summary>
        /// Get custom property value
        /// </summary>
        public string GetPropValue(string key) => formProps.ExtraProps.FirstOrDefault(it => it.Key == key)?.Value;

        /// <summary>
        /// Set custom property value
        /// </summary>
        public void SetPropValue(string key, string value)
        {
            Argument argument;
            foreach (var it in formProps.ExtraProps)
            {
                argument = it;
                if (argument.Key == key)
                {
                    argument.Value = value;
                    return;
                }
            }
            argument = new Argument(key, value);
            formProps.ExtraProps.Add(argument);
        }
    }

    [Serializable]
    public class FormProps
    {
        public Size WindowSize;
        public List<Argument> ExtraProps;

        public FormProps()
        {
            WindowSize = Size.Empty;
            ExtraProps = new List<Argument>();
        }
    }
}