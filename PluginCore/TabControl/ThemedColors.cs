/*
 * This code is provided under the Code Project Open Licence (CPOL)
 * See http://www.codeproject.com/info/cpol10.aspx for details
*/

using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace System.Drawing
{
	internal sealed class ThemedColors
	{
        #region "    Variables and Constants "

        const string NormalColor = "NormalColor";
        const string HomeStead = "HomeStead";
        const string Metallic = "Metallic";
        const string NoTheme = "NoTheme";

        static readonly Color[] _toolBorder;
		#endregion

		#region "    Properties "

		[Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static ColorScheme CurrentThemeIndex => GetCurrentThemeIndex();

        [Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static Color ToolBorder => _toolBorder[(int)CurrentThemeIndex];

        #endregion

		#region "    Constructors "

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ThemedColors() => _toolBorder = new[]
        {
            Color.FromArgb(127, 157, 185), Color.FromArgb(164, 185, 127), Color.FromArgb(165, 172, 178),
            Color.FromArgb(132, 130, 132)
        };

        ThemedColors(){}

		#endregion

        static ColorScheme GetCurrentThemeIndex()
		{
			var theme = ColorScheme.NoTheme;
            if (VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser && Application.RenderWithVisualStyles)
            {
                theme = VisualStyleInformation.ColorScheme switch
                {
                    NormalColor => ColorScheme.NormalColor,
                    HomeStead => ColorScheme.HomeStead,
                    Metallic => ColorScheme.Metallic,
                    _ => ColorScheme.NoTheme
                };
            }
            return theme;
		}

		public enum ColorScheme
		{
			NormalColor = 0,
			HomeStead = 1,
			Metallic = 2,
			NoTheme = 3
		}
    }
}