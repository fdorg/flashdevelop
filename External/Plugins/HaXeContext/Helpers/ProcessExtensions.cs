using System.Diagnostics;

namespace HaXeContext.Helpers
{
    internal static class ProcessExtensions
    {
        public static ProcessStartInfo Clone(this ProcessStartInfo src)
        {
            return new ProcessStartInfo
            {
                Arguments = src.Arguments,
                CreateNoWindow = src.CreateNoWindow,
                Domain = src.Domain,
                ErrorDialog = src.ErrorDialog,
                ErrorDialogParentHandle = src.ErrorDialogParentHandle,
                FileName = src.FileName,
                LoadUserProfile = src.LoadUserProfile,
                Password = src.Password,
                RedirectStandardError = src.RedirectStandardError,
                RedirectStandardInput = src.RedirectStandardInput,
                RedirectStandardOutput = src.RedirectStandardOutput,
                StandardErrorEncoding = src.StandardErrorEncoding,
                StandardOutputEncoding = src.StandardOutputEncoding,
                UserName = src.UserName,
                UseShellExecute = src.UseShellExecute,
                Verb = src.Verb,
                WindowStyle = src.WindowStyle,
                WorkingDirectory = src.WorkingDirectory
            };
        }
    }
}
