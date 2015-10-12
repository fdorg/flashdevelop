using System.Collections;
using System.IO;

namespace PluginCore.Helpers
{
    public class FileInspector
    {
        public static string[] ExecutableFileTypes = null;

        public static bool IsActionScript(string path, string ext)
        {
            return ext == ".as";
        }

        public static bool IsFLA(string path, string ext)
        {
            return ext == ".fla";
        }

        public static bool IsActionScript(ICollection paths)
        {
            foreach (string path in paths)
            {
                if (!IsActionScript(path, Path.GetExtension(path).ToLower())) return false;
            }
            return true;
        }

        public static bool IsHaxeFile(string path, string ext)
        {
            return ext == ".hx" || ext == ".hxp";
        }

        public static bool IsMxml(string path, string ext)
        {
            return ext == ".mxml";
        }

        public static bool IsCss(string path, string ext)
        {
            return ext == ".css";
        }

        public static bool IsImage(string path, string ext)
        {
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif";
        }

        public static bool IsSwf(string path, string ext)
        {
            return ext == ".swf";
        }

        public static bool IsSwc(string path)
        {
            return IsSwc(path, Path.GetExtension(path).ToLower());
        }

        public static bool IsSwc(string path, string ext)
        {
            return ext == ".swc" || ext == ".ane" || ext == ".jar";
        }

        public static bool IsFont(string path, string ext)
        {
            return ext == ".ttf" || ext == ".otf";
        }

        public static bool IsSound(string path, string ext)
        {
            return ext == ".mp3";
        }

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
            string ext = Path.GetExtension(path).ToLower();
            if (ExecutableFileTypes != null)
            foreach (string type in ExecutableFileTypes)
            {
                if (type == ext) return true;
            }
            return false;
        }

        public static bool IsHtml(string path, string ext)
        {
            return ext == ".html" || ext == ".htm" || ext == ".mtt"/*haxe templo*/;
        }

        public static bool IsXml(string path, string ext)
        {
            // allow for mxml, sxml, asml, etc
            return (ext == ".xml" || (ext.Length == 5 && ext.EndsWith("ml")));
        }

        public static bool IsText(string path, string ext)
        {
            return ext == ".txt" || Path.GetFileName(path).StartsWith(".");
        }

        public static bool IsAS2Project(string path, string ext)
        {
            return ext == ".fdp" || ext == ".as2proj";
        }

        public static bool IsAS3Project(string path, string ext)
        {
            return ext == ".as3proj" || IsFlexBuilderProject(path);
        }

        public static bool IsFlexBuilderPackagedProject(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            return ext == ".fxp" || ext == ".zip" || ext == ".fxpl";
        }

        public static bool IsFlexBuilderProject(string path)
        {
            return Path.GetFileName(path).ToLower() == ".actionscriptproperties";
        }

        public static bool IsHaxeProject(string path, string ext)
        {
            return ext == ".hxproj";
        }

        public static bool IsGenericProject(string path, string ext)
        {
            return ext == ".fdproj";
        }

        public static bool IsProject(string path)
        {
            return IsProject(path, Path.GetExtension(path).ToLower());
        }

        public static bool IsProject(string path, string ext)
        {
            return IsAS2Project(path, ext) || IsAS3Project(path, ext) || IsHaxeProject(path, ext) || IsGenericProject(path, ext);
        }

        public static bool IsTemplate(string path, string ext)
        {
            return ext == ".template";
        }

    }

}
