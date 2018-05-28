namespace PluginCore.Utilities
{
    /// <summary>
    /// Represents a semantic version, see http://semver.org/
    /// Follows Semantic Versioning 2.0.0
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
            // TODO: Parse and expose pre-release and build metadata
            // ignore the pre-release and build metadata denotation if present
            int hyphenIndex = version.IndexOf('-');
            if (hyphenIndex >= 0) version = version.Substring(0, hyphenIndex);
            int plusIndex = version.IndexOf('+');
            if (plusIndex >= 0) version = version.Substring(0, plusIndex);

            string[] numbers = version.Split('.');

            if (numbers.Length >= 1) int.TryParse(numbers[0], out Major);
            if (numbers.Length >= 2) int.TryParse(numbers[1], out Minor);
            if (numbers.Length >= 3) int.TryParse(numbers[2], out Patch);
        }

        #region Operators

        public static bool operator ==(SemVer left, SemVer right)
        {
            return left.Major == right.Major
                && left.Minor == right.Minor
                && left.Patch == right.Patch;
        }

        public static bool operator !=(SemVer left, SemVer right)
        {
            return left.Major != right.Major
                || left.Minor != right.Minor
                || left.Patch != right.Patch;
        }

        public static bool operator <(SemVer left, SemVer right)
        {
            return left.Major < right.Major
                || left.Major == right.Major && (left.Minor < right.Minor
                || left.Minor == right.Minor && (left.Patch < right.Patch));
        }

        public static bool operator >(SemVer left, SemVer right)
        {
            return left.Major > right.Major
                || left.Major == right.Major && (left.Minor > right.Minor
                || left.Minor == right.Minor && (left.Patch > right.Patch));
        }

        public static bool operator <=(SemVer left, SemVer right)
        {
            return left.Major < right.Major
                || left.Major == right.Major && (left.Minor < right.Minor
                || left.Minor == right.Minor && (left.Patch < right.Patch
                || left.Patch == right.Patch));
        }

        public static bool operator >=(SemVer left, SemVer right)
        {
            return left.Major > right.Major
                || left.Major == right.Major && (left.Minor > right.Minor
                || left.Minor == right.Minor && (left.Patch > right.Patch
                || left.Patch == right.Patch));
        }

        public static bool operator ==(SemVer left, string right)
        {
            return left == new SemVer(right);
        }

        public static bool operator !=(SemVer left, string right)
        {
            return left != new SemVer(right);
        }

        public static bool operator <(SemVer left, string right)
        {
            return left < new SemVer(right);
        }

        public static bool operator >(SemVer left, string right)
        {
            return left > new SemVer(right);
        }

        public static bool operator <=(SemVer left, string right)
        {
            return left <= new SemVer(right);
        }

        public static bool operator >=(SemVer left, string right)
        {
            return left >= new SemVer(right);
        }

        #endregion

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Patch}";
        }

        public bool IsOlderThan(SemVer semVer)
        {
            return this < semVer;
        }

        public bool Equals(SemVer semVer)
        {
            return this == semVer;
        }

        public bool IsGreaterThan(SemVer semVer)
        {
            return this > semVer;
        }

        public bool IsGreaterThanOrEquals(SemVer semVer)
        {
            return this >= semVer;
        }

        public override bool Equals(object obj)
        {
            return obj is SemVer ? this == (SemVer) obj : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Major << 16 ^ Minor << 8 ^ Patch;
        }
    }
}
