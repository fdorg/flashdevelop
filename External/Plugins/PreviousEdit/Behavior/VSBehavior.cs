namespace PreviousEdit.Behavior
{
    public class VSBehavior
    {
        readonly Queue queue = new Queue();
        public bool CanBackward => queue.CanBackward;
        public bool CanForward => queue.CanForward;

        public QueueItem CurrentItem => queue.CurrentItem;

        public void Clear() => queue.Clear();

        public void Backward() => queue.Backward();

        public void Forward() => queue.Forward();

        public void Add(string fileName, int position, int line)
        {
            var item = queue.GetBackwardItem();
            if (item.FileName == fileName && item.Line == line)
            {
                CurrentItem.FileName = fileName;
                CurrentItem.Position = position;
                CurrentItem.Line = line;
            }   
            else queue.Add(fileName, position, line);
        }

        public QueueItem GetBackwardItem() => queue.GetBackwardItem();

        public void Change(string fileName, int startPosition, int charsAdded, int linesAdded) => queue.Change(fileName, startPosition, charsAdded, linesAdded);

#if DEBUG
        public override string ToString() => queue.ToString();
#endif
    }
}