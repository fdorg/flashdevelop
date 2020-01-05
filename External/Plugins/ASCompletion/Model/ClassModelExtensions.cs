using System.Linq;

namespace ASCompletion.Model
{
    public static class ClassModelExtensions
    {
        /// <summary>
        /// Return the first MemberModel instance match in the ClassModel's members
        /// </summary>
        /// <param name="name">Member name to mach</param>
        /// <param name="recursive"></param>
        /// <returns>First match</returns>
        public static MemberModel SearchMember(this ClassModel @this, string name, bool recursive)
        {
            if (!recursive) return @this.Members.Search(name, 0, 0);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                var result = type.Members.Search(name, 0, 0);
                if (result != null) return result;
                type = type.Extends;
            }
            return null;
        }

        public static MemberList SearchMembers(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (!recursive) return @this.SearchMembers(flags);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                var result = type.SearchMembers(flags);
                if (result.Count > 0) return result;
                type = type.Extends;
            }
            return null;
        }

        /// <summary>
        /// Return all MemberModel instance matches in the ClassModel's members
        /// </summary>
        /// <param name="flags">Flags flags</param>
        /// <returns>All matches</returns>
        static MemberList SearchMembers(this ClassModel @this, FlagType flags) => new MemberList(@this.Members.Where(it => (it.Flags & flags) == flags));

        public static bool ContainsMember(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (!recursive) return @this.ContainsMember(flags);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                if (type.ContainsMember(flags)) return true;
                type = type.Extends;
            }
            return false;
        }

        static bool ContainsMember(this ClassModel @this, FlagType mask) => @this.Members.Any(it => (it.Flags & mask) == mask);
    }
}