// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace HaXeContext
{
    public interface IHaxeCompletionHandler
    {
        string GetCompletion(string[] args);
        string GetCompletion(string[] args, string fileContent);
        void Stop();
    }
}
