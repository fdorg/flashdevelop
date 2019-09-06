namespace ASCompletion.Model
{
    public static class ClassModelExtensions
    {
        public static MemberList GetMembers(this ClassModel @this, FlagType flags, bool recursive)
        {
            if (@this.Extends.IsVoid()) @this.ResolveExtends();
            var type = @this.Extends;
            while (!type.IsVoid())
            {
                var list = type.Members.MultipleSearch(flags);
                if (list.Count > 0) return list;
                type = type.Extends;
            }
            return null;
        }
    }
}