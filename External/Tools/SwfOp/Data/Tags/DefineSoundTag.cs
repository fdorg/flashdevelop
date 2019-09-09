// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
