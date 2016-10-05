using System;
using System.ComponentModel;

namespace ConsolePanel
{
    [Serializable]
    class Settings
    {
        private ConsoleColor background = ConsoleColor.Black;
        private ConsoleColor foreground = ConsoleColor.White;

        [DisplayName("Background Color"), DefaultValue(ConsoleColor.Black)]
        public ConsoleColor BackgroundColor
        {
            get
            {
                return this.background;
            }
            set
            {
                this.background = value;
            }
        }

        [DisplayName("Foreground Color"), DefaultValue(ConsoleColor.White)]
        public ConsoleColor ForegroundColor
        {
            get
            {
                return this.foreground;
            }
            set
            {
                this.foreground = value;
            }
        }
    }
}
