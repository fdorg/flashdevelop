using System;
using System.Text;
using System.Drawing;

namespace ProjectManager.Projects
{
	public abstract class MovieOptions
	{
		public int Fps;
		public int Width;
		public int Height;
		public int MajorVersion;
        public int MinorVersion;
        public string Platform;
        public string Background;
        public string[] TargetBuildTypes;

		public MovieOptions()
		{
			Fps = 30;
			Width = 800;
			Height = 600;
			Background = "#FFFFFF";
		}

		public Color BackgroundColor
		{
			get { return ColorTranslator.FromHtml(Background); }
			set { Background = string.Format("#{0:X6}", (value.R << 16) + (value.G << 8) + value.B); }
		}

        public int BackgroundColorInt
        {
            get
            {
                Color c = BackgroundColor;
                return (c.R << 16) + (c.G << 8) + c.B;
            }
        }

        public abstract string[] TargetPlatforms { get; }
        public abstract string[] TargetVersions(string platform);
        public abstract string DefaultVersion(string platform);
        public abstract OutputType[] OutputTypes { get; }
        public abstract OutputType DefaultOutput(string platform);
        public abstract bool IsGraphical(string platform);

        public virtual bool HasOutput(OutputType output)
        {
            return (output == OutputType.Application || output == OutputType.Library);
        }

        public virtual string Version 
        { 
            get { return MajorVersion + "." + MinorVersion; }
            set
            {
                string[] p = value.Split('.');
                MajorVersion = p[0].Length > 0 ? int.Parse(p[0]) : 0;
                if (p.Length > 1 && p[1].Length > 0) MinorVersion = int.Parse(p[1]); else MinorVersion = 0;
            }
        }

        public abstract bool DebuggerSupported { get; }

	}
}
