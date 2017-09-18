using CodeFormatter.Preferences;
using PluginCore.Helpers;
using PluginCore.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeFormatter.Utilities
{
    public class HaxeAStyleHelper
    {
        /// <summary>
        /// Returns the brace style identifier used for AStyle for the given name.
        /// </summary>
        public static string GetBraceStyleFromName(string name)
        {
            switch (name)
            {
                case "Allman":
                    return "allman";
                case "Java":
                    return "java";
                case "Kernighan & Ritchie":
                    return "kr";
                case "Stroustrup":
                    return "stroustrup";
                case "Whitesmith":
                    return "whitesmith";
                case "VTK":
                    return "vtk";
                case "Banner":
                    return "banner";
                case "GNU":
                    return "gnu";
                case "Linux":
                    return "linux";
                case "Horstmann":
                    return "horstmann";
                case "One True Brace":
                    return "otbs";
                case "Google":
                    return "google";
                //case "Mozilla":
                //    return "mozilla";
                case "Pico":
                    return "pico";
                case "Lisp":
                    return "lisp";
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the brace style denoted by the given identifier.
        /// The inverse function is: <see cref="GetBraceStyleFromName"/>
        /// </summary>
        public static string GetNameFromBraceStyle(string identifier)
        {
            switch (identifier)
            {
                case "allman":
                    return "Allman";
                case "java":
                    return "Java";
                case "kr":
                    return "Kernighan & Ritchie";
                case "stroustrup":
                    return "Stroustrup";
                case "whitesmith":
                    return "Whitesmith";
                case "vtk":
                    return "VTK";
                case "banner":
                    return "Banner";
                case "gnu":
                    return "GNU";
                case "linux":
                    return "Linux";
                case "horstmann":
                    return "Horstmann";
                case "otbs":
                    return "One True Brace";
                case "google":
                    return "Google";
                //case "mozilla":
                //    return "Mozilla";
                case "pico":
                    return "Pico";
                case "lisp":
                    return "Lisp";
            }

            return null;
        }

        public static HaxeAStyleOptions GetDefaultOptions()
        {
            HaxeAStyleOptions options = new HaxeAStyleOptions();
            AddDefaultOptions(options);

            return options;
        }

        public static string GetAStyleArguments(Settings settings)
        {
            return string.Join(" ", settings.Pref_AStyle_Haxe.ToStringArray());
        }

        public static void AddDefaultOptions(HaxeAStyleOptions options)
        {
            if (!options.Exists("--mode"))
            {
                options.Add(new HaxeAStyleOption("--mode", "cs"));
            }
            if (!options.Exists("--style"))
            {
                options.Add(new HaxeAStyleOption("--style", "allman"));
            }
            if (!options.Exists(o => o.Name != null && o.Name.StartsWith("--indent")))
            {
                options.Add(new HaxeAStyleOption("--indent=spaces", 4));
            }

            //Not supported by old version of AStyle
            //if (!options.Exists("--options"))
            //{
            //    options.Add(new HaxeAStyleOption("--options", "none"));
            //}

        }
    }
}
