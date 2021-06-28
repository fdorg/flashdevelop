using System;
using System.ComponentModel;

namespace PreviousEdit
{
    [Serializable]
    public class Settings
    {
        public const int MinimumBackward = 10;
        int maxBackward = MinimumBackward;

        [Category("General")]
        [DisplayName("Maximum Navigate Backward")]
        [DefaultValue(MinimumBackward)]
        public int MaxBackward
        {
            get => maxBackward;
            set => maxBackward = Math.Max(MinimumBackward, value);
        }
    }
}