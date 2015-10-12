using System;
using System.IO;
using System.Text;
using PluginCore.Helpers;
using PluginCore.Managers;

namespace ASCompletion.Commands
{
    public class CreateTrustFile
    {
        private const string FULLPATH = "{0}{1}Macromedia{1}Flash Player{1}#Security{1}FlashPlayerTrust";
        
        /// <summary>
        /// Executes the command and returns if the command was successful
        /// </summary>
        /// <param name="name">Trust file name</param>
        /// <param name="path">Path to trust</param>
        /// <returns>Operation successful</returns>
        public static bool Run(string name, string path)
        {
            if (name == null || path == null) return false;
            try
            {
                path += " ";
                string separator = Path.DirectorySeparatorChar.ToString();
                string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string fixedPath = String.Format(FULLPATH, appDataDir, separator);
                if (!Directory.Exists(fixedPath)) Directory.CreateDirectory(fixedPath);
                string file = Path.Combine(fixedPath, name);
                if (File.Exists(file))
                {
                    string src = FileHelper.ReadFile(file, Encoding.Default);
                    if (src.IndexOf(path) < 0) FileHelper.AddToFile(file, "\r\n" + path, Encoding.Default);
                }
                else FileHelper.WriteFile(file, path, Encoding.Default);
                return true;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return false;
            }
        }

    }

}
