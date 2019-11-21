using System;
using System.Collections.Generic;
using System.Text;

namespace SwfOp.Data.Tags
{
    public class DefineSoundTag:DefineBitsTag
    {
        /// <summary>
        /// constructor
        /// </summary>
        public DefineSoundTag(ushort id, byte[] sound)
            : base(id, sound)
        {
        }
    }
}
