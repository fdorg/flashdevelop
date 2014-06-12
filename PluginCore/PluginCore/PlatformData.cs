using System;
using System.Text;
using System.Collections.Generic;

namespace PluginCore
{
    // When updating, update also AirProperties plugin!

    public class PlatformData
    {
        public static String DEFAULT_NME_VERSION = "3.0";
        public static String DEFAULT_AIR_VERSION = "14.0";
        public static String DEFAULT_AIR_MOBILE_VERSION = "14.0";
        public static String DEFAULT_FLASH_VERSION = "14.0";
        public static String[] AIR_VERSIONS = new String[] { "1.5", "2.0", "2.5", "2.6", "2.7", "3.0", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8", "3.9", "4.0", "13.0", "14.0" };
        public static String[] AIR_MOBILE_VERSIONS = new String[] { "2.5", "2.6", "2.7", "3.0", "3.1", "3.2", "3.3", "3.4", "3.5", "3.6", "3.7", "3.8", "3.9", "4.0", "13.0", "14.0" };
        public static String[] FLASH_LEGACY_VERSIONS = new String[] { "6.0", "7.0", "8.0", "9.0", "10.0", "10.1", "10.2", "10.3", "11.0", "11.1", "11.2", "11.3", "11.4", "11.5", "11.6", "11.7", "11.8", "11.9", "12.0", "13.0", "14.0" };
        public static String[] FLASH_VERSIONS = new String[] { "9.0", "10.0", "10.1", "10.2", "10.3", "11.0", "11.1", "11.2", "11.3", "11.4", "11.5", "11.6", "11.7", "11.8", "11.9", "12.0", "13.0", "14.0" };
        public static String[] SWF_VERSIONS = new String[] { "9", "10", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25" };
        public static String[] NME_TARGETS = new String[] { "flash", "html5", "windows", "neko", "android", "webos", "blackberry" };
        public static String[] NME_VERSIONS = new String[] { "3.0" };
        
        /// <summary>
        /// 
        /// </summary>
        public static String GuessFlashPlayerForAIR(String version)
        {
            Int32 majorVersion = 10;
            Int32 minorVersion = 0;
            String[] p = (version ?? "").Split('.');
            if (p.Length > 0) Int32.TryParse(p[0], out majorVersion);
            if (p.Length > 1) Int32.TryParse(p[1], out minorVersion);
            GuessFlashPlayerForAIR(ref majorVersion, ref minorVersion);
            return majorVersion + "." + minorVersion;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void GuessFlashPlayerForAIR(ref Int32 majorVersion, ref Int32 minorVersion)
        {
            Double v = majorVersion + (Double)minorVersion / 10;
            if (v < 2) { majorVersion = 9; minorVersion = 0; }
            else if (v < 2.5) { majorVersion = 10; minorVersion = 0; }
            else if (v < 2.6) { majorVersion = 10; minorVersion = 1; }
            else if (v < 2.7) { majorVersion = 10; minorVersion = 2; }
            else if (v < 3.0) { majorVersion = 10; minorVersion = 3; }
            else if (v < 3.1) { majorVersion = 11; minorVersion = 0; }
            else if (v < 3.2) { majorVersion = 11; minorVersion = 1; }
            else if (v < 3.3) { majorVersion = 11; minorVersion = 2; }
            else if (v < 3.4) { majorVersion = 11; minorVersion = 3; }
            else if (v < 3.5) { majorVersion = 11; minorVersion = 4; }
            else if (v < 3.6) { majorVersion = 11; minorVersion = 5; }
            else if (v < 3.7) { majorVersion = 11; minorVersion = 6; }
            else if (v < 3.8) { majorVersion = 11; minorVersion = 7; }
            else if (v < 3.9) { majorVersion = 11; minorVersion = 8; }
            else if (v < 4.0) { majorVersion = 11; minorVersion = 9; }
            else if (v < 13.0) { majorVersion = 12; minorVersion = 0; }
            else if (v < 14.0) { majorVersion = 13; minorVersion = 0; }
            else { majorVersion = 14; minorVersion = 0; }
        }

    }

}

