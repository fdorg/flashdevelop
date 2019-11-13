using System;
using System.Collections;
using System.IO;

namespace PluginCore.Helpers
{
    public class FileInspector
    {
        public static string[] ExecutableFileTypes = null;

        public static bool IsActionScript(string path, string ext) => ext == ".as";

        public static bool IsFLA(string path, string ext) => ext == ".fla";

        public static bool IsActionScript(ICollection paths)
        {
            foreach (string path in paths)
            {
                if (!IsActionScript(path, Path.GetExtension(path).ToLower())) return false;
            }
            return true;
        }

        public static bool IsHaxeFile(string path, string ext) => ext == ".hx" || ext == ".hxp";

        public static bool IsHxml(string ext) => ext == ".hxml";

        public static bool IsMxml(string path, string ext) => ext == ".mxml";

        public static bool IsCss(string path, string ext) => ext == ".css";

        public static bool IsImage(string path, string ext)
        {
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif";
        }

        public static bool IsSwf(string path, string ext) => ext == ".swf";

        public static bool IsSwc(string path) => IsSwc(path, Path.GetExtension(path).ToLower());

        public static bool IsSwc(string path, string ext) => ext == ".swc" || ext == ".ane" || ext == ".jar";

        public static bool IsFont(string path, string ext) => ext == ".ttf" || ext == ".otf";

        public static bool IsSound(string path, string ext) => ext == ".mp3";

        public static bool IsResource(string path, string ext)
        {
            return IsImage(path, ext) || IsSwf(path, ext) || IsFont(path, ext) || IsSound(path, ext);
        }

        public static bool IsResource(ICollection paths)
        {
            foreach (string path in paths)
            {
                if (!IsResource(path, Path.GetExtension(path).ToLower())) return false;
            }
            return true;
        }

        public static bool ShouldUseShellExecute(string path)
        {
            if (ExecutableFileTypes != null)
            {
                string ext = Path.GetExtension(path).ToLower();
                foreach (string type in ExecutableFileTypes)
                {
                    if (type == ext) return true;
                }
            }
            return false;
        }

        public static bool IsHtml(string path, string ext) => ext == ".html" || ext == ".htm" || ext == ".mtt";

        public static bool IsXml(string path, string ext)
        {
            // allow for mxml, sxml, asml, etc
            return (ext == ".xml" || (ext.Length == 5 && ext.EndsWith("ml", StringComparison.Ordinal)));
        }

        public static bool IsText(string path, string ext)
        {
            return ext == ".txt" || Path.GetFileName(path).StartsWith(".", StringComparison.Ordinal);
        }

        public static bool IsAS2Project(string path, string ext) => ext == ".fdp" || ext == ".as2proj";

        public static bool IsAS3Project(string path, string ext) => ext == ".as3proj" || IsFlexBuilderProject(path);

        public static bool IsFlexBuilderPackagedProject(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext == ".fxp" || ext == ".zip" || ext == ".fxpl";
        }

        public static bool IsFlexBuilderProject(string path) => Path.GetFileName(path).ToLower() == ".actionscriptproperties";

        public static bool IsHaxeProject(string path, string ext) => ext == ".hxproj";

        public static bool IsGenericProject(string path, string ext) => ext == ".fdproj";

        public static bool IsProject(string path) => IsProject(path, Path.GetExtension(path).ToLower());

        public static bool IsProject(string path, string ext)
        {
            return IsAS2Project(path, ext) || IsAS3Project(path, ext) || IsHaxeProject(path, ext) || IsGenericProject(path, ext);
        }

        public static bool IsTemplate(string path, string ext) => ext == ".template";
    }

}
