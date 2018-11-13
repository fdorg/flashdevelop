using System.Collections.Generic;
using System.Linq;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using PluginCore.FRService;

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

        protected override bool MemberTypeImported(string type, string searchInText, string sourceFile)
        {
            if (base.MemberTypeImported(type, searchInText, sourceFile)) return true;
            // Support for String interpolation(https://haxe.org/manual/lf-string-interpolation.html)
            var search = new FRSearch(type);
            search.Filter = SearchFilter.InStringLiterals;
            search.NoCase = false;
            search.WholeWord = true;
            search.SourceFile = sourceFile;
            var matches = search.Matches(searchInText);
            if (matches != null)
            {
                var ctx = ASContext.Context.CodeComplete;
                var sci = ASContext.CurSciControl;
                foreach (var m in matches)
                {
                    if (ctx.IsStringInterpolationStyle(sci, m.Index)) return true;
                }
            }
            return false;
        }
    }
}
