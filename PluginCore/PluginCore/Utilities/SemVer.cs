// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace PluginCore.Utilities
{
    /// <summary>
    /// Represents a semantic version, see http://semver.org/
    /// </summary>
    public class SemVer
    {
        public static readonly SemVer Zero = new SemVer();

        public readonly int Major;
        public readonly int Minor;
        public readonly int Patch;

        private SemVer()
        {
        }

        public SemVer(string version)
        {
            // ignore the pre-release denotation if present
            int hyphenIndex = version.IndexOf('-');
            if (hyphenIndex >= 0) version = version.Substring(0, hyphenIndex);

            string[] numbers = version.Split('.');

            if (numbers.Length >= 1) int.TryParse(numbers[0], out Major);
            if (numbers.Length >= 2) int.TryParse(numbers[1], out Minor);
            if (numbers.Length >= 3) int.TryParse(numbers[2], out Patch);
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}", Major, Minor, Patch);
        }

        public bool IsOlderThan(SemVer semVer)
        {
            return (semVer.Major > Major)
                   || (semVer.Major == Major && semVer.Minor > Minor)
                   || (semVer.Major == Major && semVer.Minor == Minor && semVer.Patch > Patch);
        }

        public bool Equals(SemVer semVer)
        {
            return semVer.Major == Major && semVer.Minor == Minor && semVer.Patch == Patch;
        }

        public bool IsGreaterThan(SemVer semVer)
        {
            return (semVer.Major < Major)
                   || (semVer.Major == Major && semVer.Minor < Minor)
                   || (semVer.Major == Major && semVer.Minor == Minor && semVer.Patch < Patch);
        }

        public bool IsGreaterThanOrEquals(SemVer semVer)
        {
            return Equals(semVer) || IsGreaterThan(semVer);
        }
    }
}
