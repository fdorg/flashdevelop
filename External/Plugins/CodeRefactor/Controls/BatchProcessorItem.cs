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
