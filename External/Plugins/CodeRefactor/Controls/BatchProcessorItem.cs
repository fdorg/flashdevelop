// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using CodeRefactor.Managers;

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
