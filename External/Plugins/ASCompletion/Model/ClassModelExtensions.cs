using System.Linq;

namespace ASCompletion.Model
{
    public static class ClassModelExtensions
    {
        /// <summary>
        /// Return all MemberModel instance matches in the ClassModel's members
        /// </summary>
        /// <param name="flags">Flags mask</param>
        /// <param name="recursive"></param>
        /// <returns>All matches</returns>
        public static MemberList SearchMembers(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (!recursive) @this.SearchMembers(flags);
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
        /// <param name="flags">Flags mask</param>
        /// <returns>All matches</returns>
        static MemberList SearchMembers(this ClassModel @this, FlagType flags) => new MemberList(@this.Members.Where(it => (it.Flags & flags) == flags));

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
        
        public static MemberModel SearchMember(this ClassModel @this, string name, bool recursive, out ClassModel inClass)
        {
            if (!recursive)
            {
                inClass = @this;
                return @this.Members.Search(name, 0, 0);
            }
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                var result = type.Members.Search(name, 0, 0);
                if (result != null)
                {
                    inClass = type;
                    return result;
                }
                type = type.Extends;
            }
            inClass = ClassModel.VoidClass;
            return null;
        }

        /// <summary>
        /// Return the first MemberModel instance match in the ClassModel's members
        /// </summary>
        /// <param name="flags">Flags mask</param>
        /// <param name="recursive"></param>
        /// <returns>First match</returns>
        public static MemberModel SearchMember(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (!recursive) return @this.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                var result = type.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
                if (result != null) return result;
                type = type.Extends;
            }
            return null;
        }
        
        public static MemberModel SearchMember(this ClassModel @this, FlagType flags, bool recursive, out ClassModel inClass)
        {
            if (!recursive)
            {
                inClass = @this;
                return @this.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
            }
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                var result = type.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
                if (result != null)
                {
                    inClass = type;
                    return result;
                }
                type = type.Extends;
            }
            inClass = null;
            return null;
        }

        /// <summary>
        /// Return the first MemberModel instance match in the ClassModel's members
        /// </summary>
        /// <param name="flags">Flags mask</param>
        /// <param name="access">Visibility flags</param>
        /// <param name="recursive"></param>
        /// <returns>First match</returns>
        public static MemberModel SearchMember(this ClassModel @this, FlagType flags, Visibility access, bool recursive)
        {
            if (!recursive) return @this.Members.FirstOrDefault(it => (it.Flags & flags) == flags && (it.Access & access) != 0);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                var result = type.Members.FirstOrDefault(it => (it.Flags & flags) == flags && (it.Access & access) != 0);
                if (result != null) return result;
                type = type.Extends;
            }
            return null;
        }

        public static bool ContainsMember(this ClassModel @this, string name, bool recursive)
        {
            if (!recursive) return @this.Members.Contains(name, 0, 0);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                if (type.Members.Contains(name, 0, 0)) return true;
                type = type.Extends;
            }
            return false;
        }

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

        public static bool ContainsMember(this ClassModel @this, string name, FlagType flags, bool recursive)
        {
            if (!recursive) return @this.Members.Contains(name, flags, 0);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                if (type.Members.Contains(name, flags, 0)) return true;
                type = type.Extends;
            }
            return false;
        }
    }
}