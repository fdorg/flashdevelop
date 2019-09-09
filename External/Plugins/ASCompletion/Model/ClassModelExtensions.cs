// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Linq;

namespace ASCompletion.Model
{
    public static class ClassModelExtensions
    {
        public static MemberList GetMembers(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (!recursive) @this.GetMembers(flags);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                var result = type.GetMembers(flags);
                if (result.Count > 0) return result;
                type = type.Extends;
            }
            return null;
        }

        /// <summary>
        /// Return all MemberModel instance matches in the ClassModel
        /// </summary>
        /// <param name="mask">Flags mask</param>
        /// <returns>All matches</returns>
        static MemberList GetMembers(this ClassModel @this, FlagType mask) => new MemberList(@this.Members.Where(it => (it.Flags & mask) == mask));

        public static bool HasMember(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (!recursive) @this.HasMember(flags);
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                if (type.HasMember(flags)) return true;
                type = type.Extends;
            }
            return false;
        }


        static bool HasMember(this ClassModel @this, FlagType mask) => @this.Members.Any(it => (it.Flags & mask) == mask);
    }
}