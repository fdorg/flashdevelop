using System.Collections.Generic;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;

namespace HaXeContext.CodeRefactor.Commands
{
    class HaxeOrganizeImports : OrganizeImports
    {
        protected override List<string> GetUniqueImports(List<MemberModel> imports, string searchInText, string sourceFile)
        {
            var result = new HashSet<string>(base.GetUniqueImports(imports, searchInText, sourceFile));
            foreach (var import in imports)
            {
                if (result.Contains(import.Type)) continue;
                var inFile = import.InFile ?? ASContext.Context.ResolveType(import.Name, FileModel.Ignore).InFile;
                if (inFile == null || inFile.Classes.Count == 1)
                {
                    if (MemberTypeImported(import.Name, searchInText, sourceFile)) result.Add(import.Type);
                }
                else if (inFile.Classes.Any(it => MemberTypeImported(it.Name, searchInText, sourceFile)))
                {
                    result.Add(import.Type);
                }
            }
            return result.ToList();
        }
    }
}
