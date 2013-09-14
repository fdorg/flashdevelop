using System;
using System.Collections;
using System.Collections.Generic;
using PluginCore.FRService;
using ASCompletion.Model;
using ASCompletion.Context;
using PluginCore.Utilities;
using ScintillaNet;
using PluginCore;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Organizes the imports
    /// </summary>
    public class OrganizeImports : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        public Boolean TruncateImports = false;
        public Boolean SeparatePackages = false;
        private Int32 _deletedImportsCompensation;
        private readonly List<KeyValuePair<MemberModel, Int32>> _importIndents = new List<KeyValuePair<MemberModel, Int32>>();

        /// <summary>
        /// The actual process implementation
        /// </summary>
        protected override void ExecutionImplementation()
        {
            IASContext context = ASContext.Context;
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            Int32 pos = sci.CurrentPos;
            List<MemberModel> imports = new List<MemberModel>(context.CurrentModel.Imports.Count);
            imports.AddRange(context.CurrentModel.Imports.Items);
            for (Int32 i = imports.Count - 1; i >= 0; i--)
                if ((imports[i].Flags & (FlagType.Using | FlagType.Constant)) != 0)
                    imports.RemoveAt(i);

            ImportsComparerLine comparerLine = new ImportsComparerLine();
            imports.Sort(comparerLine);
            sci.SetSel(sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineFrom), sci.PositionFromLine(context.CurrentModel.GetPublicClass().LineTo));
            String publicClassText = sci.SelText;
            String privateClassText = "";
            if (context.CurrentModel.Classes.Count > 1)
            {
                sci.SetSel(pos, pos);
                sci.SetSel(sci.PositionFromLine(context.CurrentModel.Classes[1].LineFrom), sci.PositionFromLine(sci.LineCount));
                privateClassText = sci.SelText;
            }

            if (imports.Count > 1 || (imports.Count > 0 && TruncateImports))
            {
                sci.BeginUndoAction();
                foreach (MemberModel import in imports)
                {
                    sci.GotoLine(import.LineFrom);
                    _importIndents.Add(new KeyValuePair<MemberModel, Int32>(import, sci.GetLineIndentation(import.LineFrom)));
                    sci.LineDelete();
                }

                if (TruncateImports)
                {
                    for (Int32 j = 0; j < imports.Count; j++)
                    {
                        MemberModel import = imports[j];
                        String[] parts = import.Type.Split('.');
                        if (parts.Length > 0 && parts[parts.Length - 1] != "*")
                            parts[parts.Length - 1] = "*";
                        
                        import.Type = String.Join(".", parts);
                    }
                }
                imports.Reverse();
                Imports separatedImports = SeparateImports(imports, context.CurrentModel.PrivateSectionIndex);
                InsertImports(separatedImports.PackageImports, publicClassText, sci, separatedImports.PackageImportsIndent);
                if (context.CurrentModel.Classes.Count > 1)
                    InsertImports(separatedImports.PrivateImports, privateClassText, sci, separatedImports.PrivateImportsIndent);
                
                sci.SetSel(pos, pos);
                sci.EndUndoAction();
            }

            context.UpdateCurrentFile(true);
            FireOnRefactorComplete();
        }

        /// <summary>
        /// Separates the imports to a container
        /// </summary>
        private Imports SeparateImports(IEnumerable<MemberModel> imports, int privateSectionIndex)
        {
            MemberModel first;
            Imports separatedImports = new Imports
            {
                PackageImports = new List<MemberModel>(),
                PrivateImports = new List<MemberModel>()
            };

            foreach (MemberModel import in imports)
            {
                if (import.LineFrom < privateSectionIndex)
                    separatedImports.PackageImports.Add(import);
                else 
                    separatedImports.PrivateImports.Add(import);
            }

            if (separatedImports.PackageImports.Count > 0)
            {
                first = separatedImports.PackageImports[0];
                separatedImports.PackageImportsIndent = GetLineIndentFor(first);
            }

            if (separatedImports.PrivateImports.Count > 0)
            {
                first = separatedImports.PrivateImports[0];
                separatedImports.PrivateImportsIndent = GetLineIndentFor(first);
            }

            return separatedImports;
        }

        /// <summary>
        /// Gets the line indent for the specified import
        /// </summary>
        private Int32 GetLineIndentFor(MemberModel import)
        {
            for (Int32 i = 0; i < _importIndents.Count; i++)
            {
                KeyValuePair<MemberModel, Int32> kvp = _importIndents[i];
                if (kvp.Key.Equals(import))
                    return kvp.Value;
            }
            return 0;
        }

        /// <summary>
        /// Inserts the imports to the current document
        /// </summary>
        private void InsertImports(List<MemberModel> imports, String searchInText, ScintillaControl sci, Int32 indent)
        {
            String eol = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            if (imports.Count <= 1 && (imports.Count <= 0 || !TruncateImports))
                return;

            Int32 line = imports[0].LineFrom - _deletedImportsCompensation;
            ImportsComparerType comparerType = new ImportsComparerType();
            imports.Sort(comparerType);
            sci.GotoLine(line);
            Int32 curLine = 0;
            List<String> uniques = GetUniqueImports(imports, searchInText);
            // correct position compensation for private imports
            _deletedImportsCompensation = imports.Count - uniques.Count;
            String prevPackage = null;
            for (Int32 i = 0; i < uniques.Count; i++)
            {
                String importStringToInsert = "import " + uniques[i] + ";" + eol;
                if (SeparatePackages)
                {
                    String currentPackage = importStringToInsert.Substring(0, importStringToInsert.LastIndexOf(".", StringComparison.Ordinal));
                    if (prevPackage != null && prevPackage != currentPackage)
                    {
                        sci.NewLine();
                        sci.GotoLine(++curLine);
                        sci.SetLineIndentation(sci.LineFromPosition(sci.CurrentPos) - 1, indent);
                        _deletedImportsCompensation--;
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
        private static List<String> GetUniqueImports(IEnumerable<MemberModel> imports, String searchInText)
        {
            List<String> results = new List<String>();
            foreach (MemberModel import in imports)
                if (!results.Contains(import.Type) && MemberTypeImported(import.Name, searchInText))
                    results.Add(import.Type);
            
            return results;
        }

        /// <summary>
        /// Checks if the member type is imported
        /// </summary>
        private static Boolean MemberTypeImported(String type, String searchInText)
        {
            if (type == "*")
                return true;

            FRSearch search = new FRSearch(type)
            {
                Filter = SearchFilter.OutsideCodeComments | SearchFilter.OutsideStringLiterals,
                NoCase = false,
                WholeWord = true
            };
            return search.Match(searchInText) != null;
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override Boolean IsValid()
        {
            return true;
        }

    }

    /// <summary>
    /// Compare import statements based on import name
    /// </summary>
    class ImportsComparerType : IComparer<MemberModel>
    {
        public Int32 Compare(MemberModel item1, MemberModel item2)
        {
            return new CaseInsensitiveComparer().Compare(item1.Type, item2.Type);
        }
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