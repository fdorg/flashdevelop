using CodeRefactor.Managers;

namespace CodeRefactor.Controls
{
    internal class BatchProcessorItem
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
