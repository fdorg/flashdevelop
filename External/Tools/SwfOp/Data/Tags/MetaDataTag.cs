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
