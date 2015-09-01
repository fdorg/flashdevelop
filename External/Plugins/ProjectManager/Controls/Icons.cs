using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace ProjectManager.Controls
{
    public class FDImage
    {
        public readonly Image Img;
        public readonly int Index;

        public FDImage(Image img, int index)
        {
            Img = img;
            Index = index;
        }

        public Icon Icon { get { return Icon.FromHandle((Img as Bitmap).GetHicon()); } }
    }

    /// <summary>
    /// Contains all icons used by the Project Manager
    /// </summary>
    public class Icons
    {
        // store all extension icons we've pulled from the file system
        static Dictionary<string, FDImage> extensionIcons = new Dictionary<string, FDImage>();

        private static IMainForm mainForm;
        private static ImageList imageList;

        public static FDImage BulletAdd;
        public static FDImage SilkPage;
        public static FDImage XmlFile;
        public static FDImage MxmlFile;
        public static FDImage MxmlFileCompile;
        public static FDImage HiddenItems;
        public static FDImage HiddenFolder;
        public static FDImage HiddenFile;
        public static FDImage BlankFile;
        public static FDImage Project;
        public static FDImage ProjectClasspath;
        public static FDImage Classpath;
        public static FDImage ProjectClasspathError;
        public static FDImage ClasspathError;
        public static FDImage Font;
        public static FDImage ImageResource;
        public static FDImage ActionScript;
        public static FDImage FlashCS3;
        public static FDImage HaxeFile;
        public static FDImage SwfFile;
        public static FDImage SwfFileHidden;
        public static FDImage SwcFile;
        public static FDImage Folder;
        public static FDImage FolderCompile;
        public static FDImage TextFile;
        public static FDImage ActionScriptCompile;
        public static FDImage HtmlFile;
        public static FDImage AddFile;
        public static FDImage OpenFile;
        public static FDImage EditFile;
        public static FDImage Browse;
        public static FDImage FindAndReplace;
        public static FDImage FindInFiles;
        public static FDImage Cut;
        public static FDImage Copy;
        public static FDImage Paste;
        public static FDImage Delete;
        public static FDImage Rename;
        public static FDImage Options;
        public static FDImage OptionsWithIssues;
        public static FDImage NewProject;
        public static FDImage GreenCheck;
        public static FDImage Gear;
        public static FDImage X;
        public static FDImage Info;
        public static FDImage Class;
        public static FDImage Method;
        public static FDImage Variable;
        public static FDImage Const;
        public static FDImage Refresh;
        public static FDImage Debug;
        public static FDImage UpArrow;
        public static FDImage DownArrow;
        public static FDImage AllClasses;
        public static FDImage SyncToFile;
        public static FDImage ClasspathFolder;
        public static FDImage LibrarypathFolder;
        public static FDImage DocumentClass;
        public static FDImage CommandPrompt;
        public static FDImage CollapseAll;

        public static ImageList ImageList { get { return imageList; } }

        public static void Initialize(IMainForm mainForm)
        {
            Icons.mainForm = mainForm;

            imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.TransparentColor = Color.Transparent;

            BulletAdd = Get(0);
            SilkPage = Get(526);
            XmlFile = GetResource("Icons.XmlFile.png");
            MxmlFile = GetResource("Icons.MxmlFile.png");
            MxmlFileCompile = GetResource("Icons.MxmlFileCompile.png");
            HiddenItems = GetGray(292);
            HiddenFolder = GetGray(203);
            HiddenFile = GetGray(526);
            BlankFile = Get(526);
            Project = Get(274);
            ProjectClasspath = Get(98);
            Classpath = Get(98);
            ProjectClasspathError = Get("98|6|3|3");
            ClasspathError = Get("98|6|3|3");
            Font = Get(408);
            ImageResource = Get(336);
            ActionScript = GetResource("Icons.ActionscriptFile.png");
            HaxeFile = GetResource("Icons.HaxeFile.png");
            SwfFile = GetResource("Icons.SwfMovie.png");
            SwfFileHidden = GetResource("Icons.SwfMovieHidden.png");
            SwcFile = GetResource("Icons.SwcFile.png");
            Folder = Get(203);
            FolderCompile = Get("203|22|-3|3");
            TextFile = Get(315);
            FlashCS3 = GetResource("Icons.FlashCS3.png");
            ActionScriptCompile = GetResource("Icons.ActionscriptCompile.png");
            HtmlFile = GetResource("Icons.HtmlFile.png");
            AddFile = Get("526|0|5|4");
            OpenFile = Get(214);
            EditFile = Get(282);
            Browse = Get(46);

            FindAndReplace = Get(484);
            FindInFiles = Get(209);
            Cut = Get(158);
            Copy = Get(292);
            Paste = Get(283);
            Delete = Get(111);
            Rename = Get(331);
            Options = Get(54);
            OptionsWithIssues = Get("54|6|3|3");
            NewProject = Get("274|0|5|4");
            GreenCheck = Get(351);
            Gear = Get(127);
            X = Get(153);
            Info = Get(229);
            Class = GetResource("Icons.Class.png");
            Method = GetResource("Icons.Method.png");
            Variable = GetResource("Icons.Variable.png");
            Const = GetResource("Icons.Const.png");
            Refresh = Get(66);
            Debug = Get(101);
            UpArrow = Get(74);
            DownArrow = Get(60);
            AllClasses = Get(202);
            SyncToFile = Get("315|9|-3|-4");
            ClasspathFolder = Get(544);
            LibrarypathFolder = Get(208);
            DocumentClass = Get(147);
            CommandPrompt = Get(57);
            CollapseAll = GetGray(166);
        }

        public static FDImage GetGray(int fdIndex)
        {
            return GetGray(fdIndex.ToString());
        }

        public static FDImage GetGray(string data)
        {
            Image image = (mainForm != null) ? mainForm.FindImage(data) : new Bitmap(16, 16);
            Image converted = ImageKonverter.ImageToGrayscale(image);
            imageList.Images.Add(converted);
            return new FDImage(converted, imageList.Images.Count - 1);
        }

        public static FDImage Get(int fdIndex)
        {
            Image image = (mainForm != null) ? mainForm.FindImage(fdIndex.ToString()) : new Bitmap(16, 16);
            imageList.Images.Add(image);
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage Get(string data)
        {
            Image image = (mainForm != null) ? mainForm.FindImage(data) : new Bitmap(16, 16);
            imageList.Images.Add(image);
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage GetResource(string resourceID)
        {
            Bitmap image;
            try
            {
                resourceID = "ProjectManager." + resourceID;
                Assembly assembly = Assembly.GetExecutingAssembly();
                image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            }
            catch {
                image = new Bitmap(16, 16);
            }
            image = (Bitmap)PluginBase.MainForm.ImageSetAdjust(image);
            imageList.Images.Add(ScaleHelper.Scale(image));
            return new FDImage(image,imageList.Images.Count-1);
        }

        public static FDImage GetImageForFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return BlankFile;
            string ext = Path.GetExtension(file).ToLower();
            if (FileInspector.IsActionScript(file, ext))
                return ActionScript;
            else if (FileInspector.IsHaxeFile(file, ext))
                return HaxeFile;
            else if (FileInspector.IsMxml(file, ext))
                return MxmlFile;
            else if (FileInspector.IsFont(file, ext))
                return Font;
            else if (FileInspector.IsImage(file, ext) || ext == ".ico")
                return ImageResource;
            else if (FileInspector.IsSwf(file, ext))
                return SwfFile;
            else if (FileInspector.IsSwc(file, ext))
                return SwcFile;
            else if (FileInspector.IsHtml(file, ext))
                return HtmlFile;
            else if (FileInspector.IsXml(file, ext))
                return XmlFile;
            else if (FileInspector.IsText(file, ext))
                return TextFile;
            else if (FileInspector.IsFLA(file, ext))
                return FlashCS3;
            else
                return ExtractIconIfNecessary(file);
        }

        public static FDImage ExtractIconIfNecessary(string file)
        {
            string extension = Path.GetExtension(file);
            if (extensionIcons.ContainsKey(extension))
            {
                return extensionIcons[extension];
            }
            else
            {
                Icon icon = IconExtractor.GetFileIcon(file, true);
                Image image = ScaleHelper.Scale(icon.ToBitmap());
                image = (Bitmap)PluginBase.MainForm.ImageSetAdjust(image);
                icon.Dispose(); imageList.Images.Add(image);
                int index = imageList.Images.Count - 1; // of the icon we just added
                FDImage fdImage = new FDImage(image, index);
                extensionIcons.Add(extension, fdImage);
                return fdImage;
            }
        }

        public static Image Overlay(Image image, Image overlay, int x, int y)
        {
            Bitmap composed = image.Clone() as Bitmap;
            using (Graphics destination = Graphics.FromImage(composed))
            {
                Rectangle dest = new Rectangle(ScaleHelper.Scale(x), ScaleHelper.Scale(y), overlay.Width, overlay.Height);
                destination.DrawImage(overlay, dest, new Rectangle(0, 0, overlay.Width, overlay.Height), GraphicsUnit.Pixel);
            }
            return composed;
        }

    }

}
