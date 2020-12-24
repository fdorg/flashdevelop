using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
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

        public Icon Icon => Icon.FromHandle((Img as Bitmap).GetHicon());
    }

    /// <summary>
    /// Contains all icons used by the Project Manager
    /// </summary>
    public class Icons
    {
        // store all extension icons we've pulled from the file system
        static readonly Dictionary<string, FDImage> extensionIcons = new Dictionary<string, FDImage>();

        static IMainForm mainForm;
        static FDImageList imageList;

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

        public static ImageList ImageList => imageList;

        public static void Initialize(IMainForm mainForm)
        {
            Icons.mainForm = mainForm;

            imageList = new FDImageList();
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
            Image image = (mainForm != null) ? mainForm.FindImage(data, false) : new Bitmap(16, 16);
            image = mainForm.GetAutoAdjustedImage(ImageKonverter.ImageToGrayscale(image));
            imageList.Images.Add(image);
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage Get(int fdIndex)
        {
            return Get(fdIndex.ToString());
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
            catch
            {
                image = new Bitmap(16, 16);
            }
            image = (Bitmap) PluginBase.MainForm.GetAutoAdjustedImage(image);
            imageList.Images.Add(ScaleHelper.Scale(image));
            return new FDImage(image, imageList.Images.Count - 1);
        }

        public static FDImage GetImageForFile(string file)
        {
            if (string.IsNullOrEmpty(file))
                return BlankFile;
            string ext = Path.GetExtension(file).ToLower();
            if (FileInspector.IsActionScript(ext))
                return ActionScript;
            if (FileInspector.IsHaxeFile(ext))
                return HaxeFile;
            if (FileInspector.IsMxml(ext))
                return MxmlFile;
            if (FileInspector.IsFont(ext))
                return Font;
            if (FileInspector.IsImage(ext) || ext == ".ico")
                return ImageResource;
            if (FileInspector.IsSwf(ext))
                return SwfFile;
            if (FileInspector.IsSwc(file, ext))
                return SwcFile;
            if (FileInspector.IsHtml(ext))
                return HtmlFile;
            if (FileInspector.IsXml(ext))
                return XmlFile;
            if (FileInspector.IsText(file, ext))
                return TextFile;
            if (FileInspector.IsFLA(ext))
                return FlashCS3;
            return ExtractIconIfNecessary(file);
        }

        public static FDImage ExtractIconIfNecessary(string file)
        {
            string extension = Path.GetExtension(file);
            if (extensionIcons.ContainsKey(extension))
            {
                return extensionIcons[extension];
            }

            Icon icon = IconExtractor.GetFileIcon(file, true);
            Image image = ScaleHelper.Scale(icon.ToBitmap());
            image = (Bitmap) PluginBase.MainForm.GetAutoAdjustedImage(image);
            icon.Dispose();
            imageList.Images.Add(image);
            int index = imageList.Images.Count - 1; // of the icon we just added
            FDImage fdImage = new FDImage(image, index);
            extensionIcons.Add(extension, fdImage);
            return fdImage;
        }

        public static Image Overlay(Image image, Image overlay, int x, int y)
        {
            var composed = (Bitmap) image.Clone();
            using var destination = Graphics.FromImage(composed);
            var dest = new Rectangle(ScaleHelper.Scale(x), ScaleHelper.Scale(y), overlay.Width, overlay.Height);
            destination.DrawImage(overlay, dest, new Rectangle(0, 0, overlay.Width, overlay.Height), GraphicsUnit.Pixel);
            return composed;
        }

        class FDImageList : ImageListManager
        {
            protected override void OnRefresh()
            {
                Image[] temp = new Image[Images.Count];
                temp[BulletAdd.Index] = BulletAdd.Img;
                temp[SilkPage.Index] = SilkPage.Img;
                temp[XmlFile.Index] = XmlFile.Img;
                temp[MxmlFile.Index] = MxmlFile.Img;
                temp[MxmlFileCompile.Index] = MxmlFileCompile.Img;
                temp[HiddenItems.Index] = HiddenItems.Img;
                temp[HiddenFolder.Index] = HiddenFolder.Img;
                temp[HiddenFile.Index] = HiddenFile.Img;
                temp[BlankFile.Index] = BlankFile.Img;
                temp[Project.Index] = Project.Img;
                temp[ProjectClasspath.Index] = ProjectClasspath.Img;
                temp[Classpath.Index] = Classpath.Img;
                temp[ProjectClasspathError.Index] = ProjectClasspathError.Img;
                temp[ClasspathError.Index] = ClasspathError.Img;
                temp[Font.Index] = Font.Img;
                temp[ImageResource.Index] = ImageResource.Img;
                temp[ActionScript.Index] = ActionScript.Img;
                temp[FlashCS3.Index] = FlashCS3.Img;
                temp[HaxeFile.Index] = HaxeFile.Img;
                temp[SwfFile.Index] = SwfFile.Img;
                temp[SwfFileHidden.Index] = SwfFileHidden.Img;
                temp[SwcFile.Index] = SwcFile.Img;
                temp[Folder.Index] = Folder.Img;
                temp[FolderCompile.Index] = FolderCompile.Img;
                temp[TextFile.Index] = TextFile.Img;
                temp[ActionScriptCompile.Index] = ActionScriptCompile.Img;
                temp[HtmlFile.Index] = HtmlFile.Img;
                temp[AddFile.Index] = AddFile.Img;
                temp[OpenFile.Index] = OpenFile.Img;
                temp[EditFile.Index] = EditFile.Img;
                temp[Browse.Index] = Browse.Img;
                temp[FindAndReplace.Index] = FindAndReplace.Img;
                temp[FindInFiles.Index] = FindInFiles.Img;
                temp[Cut.Index] = Cut.Img;
                temp[Copy.Index] = Copy.Img;
                temp[Paste.Index] = Paste.Img;
                temp[Delete.Index] = Delete.Img;
                temp[Rename.Index] = Rename.Img;
                temp[Options.Index] = Options.Img;
                temp[OptionsWithIssues.Index] = OptionsWithIssues.Img;
                temp[NewProject.Index] = NewProject.Img;
                temp[GreenCheck.Index] = GreenCheck.Img;
                temp[Gear.Index] = Gear.Img;
                temp[X.Index] = X.Img;
                temp[Info.Index] = Info.Img;
                temp[Class.Index] = Class.Img;
                temp[Method.Index] = Method.Img;
                temp[Variable.Index] = Variable.Img;
                temp[Const.Index] = Const.Img;
                temp[Icons.Refresh.Index] = Icons.Refresh.Img;
                temp[Debug.Index] = Debug.Img;
                temp[UpArrow.Index] = UpArrow.Img;
                temp[DownArrow.Index] = DownArrow.Img;
                temp[AllClasses.Index] = AllClasses.Img;
                temp[SyncToFile.Index] = SyncToFile.Img;
                temp[ClasspathFolder.Index] = ClasspathFolder.Img;
                temp[LibrarypathFolder.Index] = LibrarypathFolder.Img;
                temp[DocumentClass.Index] = DocumentClass.Img;
                temp[CommandPrompt.Index] = CommandPrompt.Img;
                temp[CollapseAll.Index] = CollapseAll.Img;
                foreach (FDImage image in extensionIcons.Values)
                {
                    temp[image.Index] = image.Img;
                }
                // Add external icons...
                for (var i = 0; i < temp.Length; i++)
                {
                    temp[i] ??= Images[i];
                }
                Images.Clear();
                Images.AddRange(temp);
            }
        }
    }

}
