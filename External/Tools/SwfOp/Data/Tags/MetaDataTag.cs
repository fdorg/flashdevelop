// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;

namespace SwfOp.Data.Tags
{
    class MetaDataTag : BaseTag
    {
        public string meta;

        public MetaDataTag(string meta)
        {
            _tagCode = (int)TagCodeEnum.MetaData;
            this.meta = meta;
        }

        /// <summary>
        /// see <see cref="SwfOp.Data.Tags.BaseTag">base class</see>
        /// </summary>
        public override void UpdateData(byte version)
        {
            throw new NotImplementedException();
        }
    }
}
