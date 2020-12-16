using System.Linq;

namespace ASCompletion.Model
{
    public static class ClassModelExtensions
    {
        /// <summary>
        /// Return all MemberModel instance matches in the ClassModel's members
        /// </summary>
        /// <param name="this">A <see cref="ClassModel" /> to return a member from.</param>
        /// <param name="flags">Flags mask</param>
        /// <param name="recursive"></param>
        /// <returns>All matches</returns>
        public static MemberList? SearchMembers(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (@this.SearchMembers(flags) is {Count: > 0} list) return list;
            if (!recursive) return null; 
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
        /// <param name="this">A <see cref="ClassModel" /> to return a member from.</param>
        /// <param name="flags">Flags mask</param>
        /// <returns>All matches</returns>
        static MemberList SearchMembers(this ClassModel @this, FlagType flags) => new MemberList(@this.Members.Where(it => (it.Flags & flags) == flags));

        /// <summary>
        /// Return the first MemberModel instance match in the ClassModel's members
        /// </summary>
        /// <param name="this">A <see cref="ClassModel" /> to return a member from.</param>
        /// <param name="name">Member name to mach</param>
        /// <param name="recursive"></param>
        /// <returns>First match</returns>
        public static MemberModel? SearchMember(this ClassModel @this, string name, bool recursive)
        {
            if (@this.Members.Search(name) is { } member) return member;
            if (!recursive) return null;
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                if (type.Members.Search(name) is { } result) return result;
                type = type.Extends;
            }
            return null;
        }
        
        public static MemberModel? SearchMember(this ClassModel @this, string name, bool recursive, out ClassModel inClass)
        {
            if (@this.Members.Search(name) is { } member)
            {
                inClass = @this;
                return member;
            }
            if (!recursive)
            {
                inClass = ClassModel.VoidClass;
                return null;
            }
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                if (type.Members.Search(name) is { } result)
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
        /// <param name="this">A <see cref="ClassModel" /> to return a member from.</param>
        /// <param name="flags">Flags mask</param>
        /// <param name="recursive"></param>
        /// <returns>First match</returns>
        public static MemberModel? SearchMember(this ClassModel @this, FlagType flags, bool recursive)
        {
            var member = @this.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
            if (member is not null) return member;
            if (!recursive) return null;
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this;
            while (!type.IsVoid())
            {
                var result = type.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
                if (result is not null) return result;
                type = type.Extends;
            }
            return null;
        }
        
        public static MemberModel? SearchMember(this ClassModel @this, FlagType flags, bool recursive, out ClassModel inClass)
        {
            var member = @this.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
            if (member is not null)
            {
                inClass = @this;
                return member;
            }
            if (!recursive)
            {
                inClass = ClassModel.VoidClass;
                return null;
            }
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                var result = type.Members.FirstOrDefault(it => (it.Flags & flags) == flags);
                if (result is not null)
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
        /// <param name="this">A <see cref="ClassModel" /> to return a member from.</param>
        /// <param name="flags">Flags mask</param>
        /// <param name="access">Visibility flags</param>
        /// <param name="recursive"></param>
        /// <returns>First match</returns>
        public static MemberModel SearchMember(this ClassModel @this, FlagType flags, Visibility access, bool recursive)
        {
            var member = @this.Members.FirstOrDefault(it => (it.Flags & flags) == flags && (it.Access & access) != 0);
            if (member is not null) return member;
            if (!recursive) return null;
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                var result = type.Members.FirstOrDefault(it => (it.Flags & flags) == flags && (it.Access & access) != 0);
                if (result is not null) return result;
                type = type.Extends;
            }
            return null;
        }

        public static bool ContainsMember(this ClassModel @this, string name, bool recursive)
        {
            if (@this.Members.Contains(name)) return true;
            if (!recursive) return false;
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                if (type.Members.Contains(name)) return true;
                type = type.Extends;
            }
            return false;
        }

        public static bool ContainsMember(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (@this.ContainsMember(flags)) return true;
            if (!recursive) return false;
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                if (type.ContainsMember(flags)) return true;
                type = type.Extends;
            }
            return false;
        }

        static bool ContainsMember(this ClassModel @this, FlagType mask)
        {
            var count = @this.Members.Count;
            for (var i = 0; i < count; i++)
            {
                if ((@this.Members[i].Flags & mask) == mask) return true;
            }
            return false;
        }

        public static bool ContainsMember(this ClassModel @this, string name, FlagType flags, bool recursive)
        {
            if (@this.Members.Contains(name, flags)) return true;
            if (!recursive) return false;
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                if (type.Members.Contains(name, flags)) return true;
                type = type.Extends;
            }
            return false;
        }
    }
}