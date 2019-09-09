// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace BasicCompletion
{
    public enum AutoInsert
    {
        CPP = 0,
        Never = 1,
        Always = 2,
    }

    [Serializable]
    public class Settings
    {
        private bool disableAutoCompletion = false;
        private AutoInsert autoInsertType = AutoInsert.CPP;

        /// <summary> 
        /// Get and sets the AutoInsertType
        /// </summary>
        [DisplayName("Auto Insert Type"), DefaultValue(AutoInsert.CPP)]
        [LocalizedDescription("BasicCompletion.Description.AutoInsertType")]
        public AutoInsert AutoInsertType
        {
            get => autoInsertType;
            set => autoInsertType = value;
        }

        /// <summary> 
        /// Get and sets the DisableAutoCompletion
        /// </summary>
        [DisplayName("Disable Auto Completion"), DefaultValue(false)]
        [LocalizedDescription("BasicCompletion.Description.DisableAutoCompletion")]
        public bool DisableAutoCompletion
        {
            get => disableAutoCompletion;
            set => disableAutoCompletion = value;
        }

    }

}

