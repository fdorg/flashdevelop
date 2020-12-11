// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using CodeRefactor.Managers;

namespace CodeRefactor.Controls
{
    internal class BatchProcessorItem
    {
        public readonly IBatchProcessor Processor;
        
        public BatchProcessorItem(IBatchProcessor processor) => Processor = processor;

        public override string ToString() => Processor.Text;
    }
}