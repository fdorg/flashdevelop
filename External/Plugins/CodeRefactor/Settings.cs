using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace CodeRefactor
{
    [Serializable]
    public class Settings
    {
        const bool DEFAULT_USE_INLINE_RENAMING = false;

        private bool separatePackages = false;
        private bool disableMoveRefactoring = false;
        bool useInlineRenaming = DEFAULT_USE_INLINE_RENAMING;

        [DisplayName("Separate Packages")]
        [LocalizedDescription("CodeRefactor.Description.SeparatePackages"), DefaultValue(false)]
        public bool SeparatePackages
        {
            get { return separatePackages; }
            set { separatePackages = value; }
        }

        [DisplayName("Disable Move Refactoring")]
        [LocalizedDescription("CodeRefactor.Description.DisableMoveRefactoring"), DefaultValue(false)]
        public bool DisableMoveRefactoring
        {
            get { return disableMoveRefactoring; }
            set { disableMoveRefactoring = value; }
        }

        [DisplayName("Use Inline Renaming")]
        [LocalizedDescription("CodeRefactor.Description.UseInlineRenaming"), DefaultValue(DEFAULT_USE_INLINE_RENAMING)]
        public bool UseInlineRenaming
        {
            get { return useInlineRenaming; }
            set { useInlineRenaming = value; }
        }
    }
}
