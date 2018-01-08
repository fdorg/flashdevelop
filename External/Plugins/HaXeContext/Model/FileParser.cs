using ASCompletion.Model;

namespace HaXeContext.Model
{
    class FileParser : ASFileParser
    {
        protected override void FinalizeModel()
        {
            var imports = model.Imports;
            for (var i = 0; i < imports.Count; i++)
            {
                var m = imports[i];
                if (string.IsNullOrEmpty(m.Type)) continue;
                var p1 = m.Type.LastIndexOf('.');
                if (p1 == -1) continue;
                var lpart = m.Type.Substring(0, p1);
                var fullPackage = lpart;
                var p2 = lpart.LastIndexOf('.');
                if (p2 != -1) lpart = lpart.Substring(p2 + 1);
                var type = model.Context.ResolveType(lpart, model);
                if (type == null || type.IsVoid() || type.Members == null || type.Members.Count == 0) continue;
                var rpart = m.Type.Substring(p1 + 1);
                var member = type.Members.Items.Find(it => it.Name == rpart);
                if (member == null) continue;
                imports[i] = (MemberModel) member.Clone();
                imports[i].InFile = new FileModel {FullPackage = fullPackage};
            }
            base.FinalizeModel();
        }
    }
}
