namespace HaXeContext
{
    public interface IHaxeCompletionHandler
    {
        string GetCompletion(string[] args);
        string GetCompletion(string[] args, string fileContent);
        void Stop();
    }
}
