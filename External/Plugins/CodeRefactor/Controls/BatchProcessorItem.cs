// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using CodeRefactor.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRefactor.Controls
{
    class BatchProcessorItem
    {
        public readonly IBatchProcessor Processor;
        
        public BatchProcessorItem(IBatchProcessor processor)
        {
            Processor = processor;
        }

        public override string ToString()
        {
            return Processor.Text;
        }
    }
}
