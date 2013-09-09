using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ASDocGen.Utilities
{
    class ArgsProcessor
    {
        /// <summary>
        /// Processes the built in runtime arguments.
        /// </summary>
        public static string Process(String input)
        {
            try
            {
                String result = input;
                String appPath = Path.GetDirectoryName(Application.ExecutablePath);
                result = result.Replace("$(AppDir)", appPath);
                result = result.Replace("$(Quote)", "\"");
                return result;
            } 
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
                return input;
            }
        }

    }

}
