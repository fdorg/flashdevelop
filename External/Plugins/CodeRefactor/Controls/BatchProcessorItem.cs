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
