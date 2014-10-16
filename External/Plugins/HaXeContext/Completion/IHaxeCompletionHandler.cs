namespace HaXeContext
{
    interface IHaxeCompletionHandler
    {
        string GetCompletion(string[] args);
        void Stop();
    }
}
