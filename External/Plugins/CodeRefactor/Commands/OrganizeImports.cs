using System;
using System.Collections.Generic;
using System.IO;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Lexers;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Organizes the imports
    /// </summary>
    public class OrganizeImports : RefactorCommand<IDictionary<string, List<SearchMatch>>>
    {
        public ScintillaControl SciControl;
        public bool TruncateImports = false;
        public bool SeparatePackages = false;
        private int deletedImportsCompensation;
        private readonly List<KeyValuePair<MemberModel, int>> ImportIndents = new List<KeyValuePair<MemberModel, int>>();

        /// <summary>
        /// The actual process implementation
        /// </summary>
        protected override void ExecutionImplementation()
        {
            var context = ASContext.Context;
            if (context.CurrentModel.Imports.Count == 0)
            {
                FireOnRefactorComplete();
                return;
            }
            var imports = new List<MemberModel>(context.CurrentModel.Imports.Items);
            var sci = SciControl ?? PluginBase.MainForm.CurrentDocument.SciControl;
            var pos = sci.CurrentPos;
            var cppPpStyle = (int)CPP.PREPROCESSOR;
            for (var i = imports.Count - 1; i >= 0; i--)
            {
                if ((imports[i].Flags & (FlagType.Using | FlagType.Constant)) != 0 || sci.LineIsInPreprocessor(sci, cppPpStyle, imports[i].LineTo))
                {
                    imports.RemoveAt(i);
                }
            }
            imports.Sort(new ImportsComparerLine());
            sci.BeginUndoAction();
            try
            {
                string publicClassText;
                var privateClassText = string.Empty;
                if (context.Features.hasModules)
                {
                    var offset = 0;
                    for (var i = imports.Count - 1; i >= 0; i--)
                    {
                        var import = imports[i];
                        if (ContainsMember(context.CurrentModel, import))
                        {
                            imports.RemoveAt(i);
                            sci.GotoLine(import.LineFrom);
                            sci.LineDelete();
                            offset++;
                        }
                        else
                        {
                            import.LineFrom -= offset;
                            import.LineTo -= offset;
                        }
                    }
                }
                foreach (var import in imports)
                {
                    sci.GotoLine(import.LineFrom);
                    ImportIndents.Add(new KeyValuePair<MemberModel, int>(import, sci.GetLineIndentation(import.LineFrom)));
                    sci.LineDelete();
                }
                if (context.Features.hasModules)
                {
                    sci.SelectAll();
                    publicClassText = sci.SelText;
                }
                else
                {
                    var end = sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineTo);
                    sci.SetSel(0, end);
                    publicClassText = sci.SelText;
                    if (context.CurrentModel.Classes.Count > 1)
                    {
                        sci.SetSel(sci.SelectionEnd + 1, sci.Length);
                        privateClassText = sci.SelText;
                    }
                }
                if (TruncateImports)
                {
                    foreach (var import in imports)
                    {
                        var parts = import.Type.Split('.');
                        if (parts.Length > 0 && parts[parts.Length - 1] != "*")
                        {
                            parts[parts.Length - 1] = "*";
                        }
                        import.Type = string.Join(".", parts);
                    }
                }
                imports.Reverse();
                var separatedImports = SeparateImports(imports, context.CurrentModel.PrivateSectionIndex);
                if (separatedImports.PackageImports.Count > 0) InsertImports(separatedImports.PackageImports, publicClassText, sci, separatedImports.PackageImportsIndent);
                if (context.CurrentModel.Classes.Count > 1 && separatedImports.PrivateImports.Count > 0)
                {
                    InsertImports(separatedImports.PrivateImports, privateClassText, sci, separatedImports.PrivateImportsIndent);
                }
                sci.SetSel(pos, pos);
            }
            finally
            {
                sci.EndUndoAction();
            }
            context.UpdateCurrentFile(true);
            FireOnRefactorComplete();
        }

        /// <summary>
        /// Separates the imports to a container
        /// </summary>
        private Imports SeparateImports(List<MemberModel> imports, int privateSectionIndex)
        {
            var separatedImports = new Imports();
            separatedImports.PackageImports = new List<MemberModel>();
            separatedImports.PrivateImports = new List<MemberModel>();
            foreach (var import in imports)
            {
                if (import.LineFrom < privateSectionIndex) separatedImports.PackageImports.Add(import);
                else separatedImports.PrivateImports.Add(import);
            }
            if (separatedImports.PackageImports.Count > 0)
            {
                var first = separatedImports.PackageImports[0];
                separatedImports.PackageImportsIndent = GetLineIndentFor(first);
            }
            if (separatedImports.PrivateImports.Count > 0)
            {
                var first = separatedImports.PrivateImports[0];
                separatedImports.PrivateImportsIndent = GetLineIndentFor(first);
            }
            return separatedImports;
        }

        static bool ContainsMember(FileModel file, MemberModel member)
        {
            var dot = ASContext.Context.Features.dot;
            var package = string.IsNullOrEmpty(file.Package) ? string.Empty : $"{file.Package}{dot}";
            return $"{package}{Path.GetFileNameWithoutExtension(file.FileName)}{dot}{member.Name}" == member.Type;
        }

        /// <summary>
        /// Gets the line indent for the specified import
        /// </summary>
        private int GetLineIndentFor(MemberModel import)
        {
            foreach (var kvp in ImportIndents)
            {
                if (kvp.Key == import) return kvp.Value;
            }
            return 0;
        }

        /// <summary>
        /// Inserts the imports to the current document
        /// </summary>
        private void InsertImports(List<MemberModel> imports, string searchInText, ScintillaControl sci, int indent)
        {
            var eol = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            var line = imports[0].LineFrom - deletedImportsCompensation;
            imports.Sort(new CaseSensitiveImportComparer());
            sci.GotoLine(line);
            var curLine = 0;
            var uniques = GetUniqueImports(imports, searchInText, sci.FileName);
            // correct position compensation for private imports
            deletedImportsCompensation = imports.Count - uniques.Count;
            string prevPackage = null;
            foreach (var import in uniques)
            {
                var importStringToInsert = "import " + import + ";" + eol;
                if (SeparatePackages)
                {
                    var currentPackage = importStringToInsert.Substring(0, importStringToInsert.LastIndexOf('.'));
                    if (prevPackage != null && prevPackage != currentPackage)
                    {
                        sci.NewLine();
                        sci.GotoLine(++curLine);
                        sci.SetLineIndentation(sci.LineFromPosition(sci.CurrentPos) - 1, indent);
                        deletedImportsCompensation--;
                    }
                    prevPackage = currentPackage;
                }
                curLine = sci.LineFromPosition(sci.CurrentPos);
                sci.InsertText(sci.CurrentPos, importStringToInsert);
                sci.SetLineIndentation(curLine, indent);
                sci.GotoLine(++curLine);
            }
        }

        /// <summary>
        /// Gets the unique string list of imports
        /// </summary>
        protected virtual List<string> GetUniqueImports(List<MemberModel> imports, string searchInText, string sourceFile)
        {
            var result = new List<string>();
            foreach (var import in imports)
            {
                if (!result.Contains(import.Type) && MemberTypeImported(import.Name, searchInText, sourceFile))
                {
                    result.Add(import.Type);
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if the member type is imported
        /// </summary>
        protected bool MemberTypeImported(string type, string searchInText, string sourceFile)
        {
            if (type == "*") return true;
            var search = new FRSearch(type);
            search.Filter = SearchFilter.OutsideCodeComments | SearchFilter.OutsideStringLiterals;
            search.NoCase = false;
            search.WholeWord = true;
            search.SourceFile = sourceFile;
            return search.Match(searchInText) != null;
        }

        /// <inheritdoc />
        public override bool IsValid() => true;
    }

    /// <summary>
    /// Compare import statements based on declaration line
    /// </summary>
    class ImportsComparerLine : IComparer<MemberModel>
    {
        public Int32 Compare(MemberModel item1, MemberModel item2)
        {
            return item2.LineFrom - item1.LineFrom;
        }
    }

    /// <summary>
    /// Container for file's imports
    /// </summary>
    class Imports
    {
        public Int32 PackageImportsIndent;
        public Int32 PrivateImportsIndent;
        public List<MemberModel> PackageImports;
        public List<MemberModel> PrivateImports;
    }

}