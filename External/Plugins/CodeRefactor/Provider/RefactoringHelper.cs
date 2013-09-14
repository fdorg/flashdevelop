using System;
using System.IO;
using System.Collections.Generic;
using PluginCore.FRService;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ScintillaNet;
using PluginCore;
using PluginCore.Managers;

namespace CodeRefactor.Provider
{
    /// <summary>
    /// Central repository of miscellaneous refactoring helper methods to be used by any refactoring command.
    /// </summary>
    public static class RefactoringHelper
    {
        /// <summary>
        /// Populates the m_SearchResults with any found matches
        /// </summary>
        public static IDictionary<String, List<SearchMatch>> GetInitialResultsList(FRResults results)
        {
            IDictionary<String, List<SearchMatch>> searchResults = new Dictionary<String, List<SearchMatch>>();
            if (results == null)
            {
                // I suppose this should never happen -- 
                // normally invoked when the user cancels the FindInFiles dialogue.  
                // but since we're programmatically invoking the FRSearch, I don't think this should happen.
                // TODO: report this?
            }
            else if (results.Count == 0)
            {
                // no search results found.  Again, an interesting issue if this comes up.  
                // we should at least find the source entry the user is trying to change.
                // TODO: report this?
            }
            else
            {
                // found some matches!
                // I current separate the search listing from the FRResults.  It's probably unnecessary but this is just the initial implementation.
                // TODO: test if this is necessary
                foreach (KeyValuePair<String, List<SearchMatch>> entry in results)
                {
                    searchResults.Add(entry.Key, new List<SearchMatch>());
                    foreach (SearchMatch match in entry.Value)
                        searchResults[entry.Key].Add(match);
                }
            }
            return searchResults;
        }

        /// <summary>
        /// Retrieves the refactoring target based on the current location.
        /// Note that this will look up to the declaration source.  
        /// This allows users to execute the rename from a reference to the source rather than having to navigate to the source first.
        /// </summary>
        public static ASResult GetDefaultRefactorTarget()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (!ASContext.Context.IsFileValid || (sci == null))
                return null;
            
            int position = sci.WordEndPosition(sci.CurrentPos, true);
            return DeclarationLookupResult(sci, position);
        }

        /// <summary>
        /// Checks if a given search match actually points to the given target source
        /// </summary>
        /// <returns>True if the SearchMatch does point to the target source.</returns>
        public static ASResult DeclarationLookupResult(ScintillaControl sci, int position)
        {
            if (!ASContext.Context.IsFileValid || (sci == null))
                return null;

            // get type at cursor position
            ASResult result = ASComplete.GetExpressionType(sci, position);
            // browse to package folder
            if (result.IsPackage && result.InFile != null)
                return null;
           
            // open source and show declaration
            if (result.IsNull())
                return null;

            if (result.Member != null && (result.Member.Flags & FlagType.AutomaticVar) > 0)
                return null;

            FileModel model = result.InFile ?? ((result.Member != null && result.Member.InFile != null) ? result.Member.InFile : null) ?? ((result.Type != null) ? result.Type.InFile : null);
            if (model == null || model.FileName == "")
                return null;

            ClassModel inClass = result.InClass ?? result.Type;
            // for Back command
            int lookupLine = sci.LineFromPosition(sci.CurrentPos);
            int lookupCol = sci.CurrentPos - sci.PositionFromLine(lookupLine);
            ASContext.Panel.SetLastLookupPosition(ASContext.Context.CurrentFile, lookupLine, lookupCol);
            // open the file
            if (model != ASContext.Context.CurrentModel)
            {
                // cached files declarations have no line numbers
                if (model.CachedModel && model.Context != null)
                {
                    ASFileParser.ParseFile(model);
                    if (inClass != null && !inClass.IsVoid())
                    {
                        inClass = model.GetClassByName(inClass.Name);
                        if (result.Member != null)
                            result.Member = inClass.Members.Search(result.Member.Name, 0, 0);
                    }
                    else if (result.Member != null)
                        result.Member = model.Members.Search(result.Member.Name, 0, 0);
                }
                if (model.FileName.Length > 0 && File.Exists(model.FileName))
                    ASContext.MainForm.OpenEditableDocument(model.FileName, false);
                else
                {
                    ASComplete.OpenVirtualFile(model);
                    result.InFile = ASContext.Context.CurrentModel;
                    if (result.InFile == null)
                        return null;

                    if (inClass != null)
                    {
                        inClass = result.InFile.GetClassByName(inClass.Name);
                        if (result.Member != null)
                            result.Member = inClass.Members.Search(result.Member.Name, 0, 0);
                    }
                    else if (result.Member != null)
                        result.Member = result.InFile.Members.Search(result.Member.Name, 0, 0);
                }
            }

            if ((inClass == null || inClass.IsVoid()) && result.Member == null)
                return null;

            sci = ASContext.CurSciControl;
            if (sci == null)
                return null;

            int line = 0;
            string name = null;
            bool isClass = false;
            // member
            if (result.Member != null && result.Member.LineFrom > 0)
            {
                line = result.Member.LineFrom;
                name = result.Member.Name;
            }
                // class declaration
            else if (inClass != null && inClass.LineFrom > 0)
            {
                line = inClass.LineFrom;
                name = inClass.Name;
                isClass = true;
                // constructor
                foreach (MemberModel member in inClass.Members)
                    if ((member.Flags & FlagType.Constructor) > 0)
                    {
                        line = member.LineFrom;
                        name = member.Name;
                        isClass = false;
                        break;
                    }
            }

            if (line > 0) // select
                ASComplete.LocateMember(
                    isClass ? "(class|interface)" : "(function|var|const|get|set|property|[,(])", name, line);

            return result;
        }

        /// <summary>
        /// Simply checks the given flag combination if they contain a specific flag
        /// </summary>
        public static bool CheckFlag(FlagType flags, FlagType checkForThisFlag)
        {
            return (flags & checkForThisFlag) == checkForThisFlag;
        }

        /// <summary>
        /// Checks if the given match actually is the declaration.
        /// </summary>
        static public bool IsMatchTheTarget(ScintillaControl sci, SearchMatch match, ASResult target)
        {
            if (sci == null || target == null || target.InFile == null || target.Member == null)
                return false;

            String originalFile = sci.FileName;
            // get type at match position
            ASResult declaration = DeclarationLookupResult(sci, sci.MBSafePosition(match.Index) + sci.MBSafeTextLength(match.Value));
            return (declaration.InFile != null && originalFile == declaration.InFile.FileName) && (sci.CurrentPos == (sci.MBSafePosition(match.Index) + sci.MBSafeTextLength(match.Value)));
        }

        /// <summary>
        /// Checks if a given search match actually points to the given target source
        /// </summary>
        /// <returns>True if the SearchMatch does point to the target source.</returns>
        static public bool DoesMatchPointToTarget(ScintillaControl sci, SearchMatch match, ASResult target, DocumentHelper associatedDocumentHelper)
        {
            if (sci == null || target == null)
                return false;

            FileModel targetInFile = null;

            if (target.InFile != null)
                targetInFile = target.InFile;
            else if (target.Member != null && target.InClass == null)
                targetInFile = target.Member.InFile;

            Boolean matchMember = targetInFile != null && target.Member != null;
            Boolean matchType = target.Member == null && target.IsStatic && target.Type != null;
            if (!matchMember && !matchType)
                return false;

            ASResult result = null;
            // get type at match position
            if (match.Index < sci.Text.Length) // TODO: find out rare cases of incorrect index reported
            {
                result = DeclarationLookupResult(sci, sci.MBSafePosition(match.Index) + sci.MBSafeTextLength(match.Value));
                if (associatedDocumentHelper != null)
                {
                    // because the declaration lookup opens a document, we should register it with the document helper to be closed later
                    associatedDocumentHelper.RegisterLoadedDocument(PluginBase.MainForm.CurrentDocument);
                }
            }

            // check if the result matches the target
            if (result == null || (result.InFile == null && result.Type == null))
                return false;

            if (!matchMember)
                return result.Type != null && result.Type.QualifiedName == target.Type.QualifiedName;
            
            if (result.Member == null)
                return false;

            var resultInFile = result.InClass != null ? result.InFile : result.Member.InFile;

            return resultInFile != null
                   && resultInFile.BasePath == targetInFile.BasePath
                   && resultInFile.FileName == targetInFile.FileName
                   && (target.InClass == null || (result.Member.LineFrom == target.Member.LineFrom))
                   && result.Member.Name == target.Member.Name;
        }

        /// <summary>
        /// Finds the given target in all project files.
        /// If the target is a local variable or function parameter, it will only search the associated file.
        /// Note: if running asynchronously, you must supply a listener to "findFinishedHandler" to retrieve the results.
        /// If running synchronously, do not set listeners and instead use the return value.
        /// </summary>
        /// <param name="target">the source member to find references to</param>
        /// <param name="progressReportHandler">event to fire as search results are compiled</param>
        /// <param name="findFinishedHandler">event to fire once searching is finished</param>
        /// <param name="asynchronous">executes in asynchronous mode</param>
        /// <returns>If "asynchronous" is false, will return the search results, otherwise returns null on bad input or if running in asynchronous mode.</returns>
        public static FRResults FindTargetInFiles(ASResult target, FRProgressReportHandler progressReportHandler, FRFinishedHandler findFinishedHandler, Boolean asynchronous)
        {
            // checks target is a member
            if (target == null || ((target.Member == null || String.IsNullOrEmpty(target.Member.Name))
                && (target.Type == null || !CheckFlag(target.Type.Flags, FlagType.Class) && !target.Type.IsEnum())))
            {
                return null;
            }
            
            // if the target we are trying to rename exists as a local variable or a function parameter we only need to search the current file
            bool currentFileOnly = target.Member != null
                                   && (target.Member.Access == Visibility.Private
                                       || CheckFlag(target.Member.Flags, FlagType.LocalVar)
                                       || CheckFlag(target.Member.Flags, FlagType.ParameterVar));

            FRConfiguration config;
            IProject project = PluginBase.CurrentProject;
            String file = PluginBase.MainForm.CurrentDocument.FileName;
            // This is out of the project, just look for this file...
            if (currentFileOnly || !IsProjectRelatedFile(project, file))
            {
                String mask = Path.GetFileName(file);
                String path = Path.GetDirectoryName(file);
                if (mask != null && mask.Contains("[model]"))
                {
                    if (findFinishedHandler != null)
                        findFinishedHandler(new FRResults());
                    
                    return null;
                }
                config = new FRConfiguration(path, mask, false, GetFRSearch(target.Member != null ? target.Member.Name : target.Type.Name));
            }
            else if (target.Member != null && !CheckFlag(target.Member.Flags, FlagType.Constructor))
                config = new FRConfiguration(GetAllProjectRelatedFiles(project), GetFRSearch(target.Member.Name));
            else
            {
                target.Member = null;
                config = new FRConfiguration(GetAllProjectRelatedFiles(project), GetFRSearch(target.Type.Name));
            }

            config.CacheDocuments = CanRenameWithFile(target);

            FRRunner runner = new FRRunner();
            if (progressReportHandler != null)
                runner.ProgressReport += progressReportHandler;
            
            if (findFinishedHandler != null)
                runner.Finished += findFinishedHandler;

            if (!asynchronous)
                return runner.SearchSync(config);

            runner.SearchAsync(config);
            return null;
        }

        /// <summary>
        /// Checks if files is related to the project
        /// TODO support SWCs -> refactor test as IProject method
        /// </summary>
        public static Boolean IsProjectRelatedFile(IProject project, String file)
        {
            foreach (String path in project.SourcePaths)
            {
                String absolute = project.GetAbsolutePath(path);
                if (file.StartsWith(absolute))
                    return true;
            }

            // If no source paths are defined, is it under the project?
            if (project.SourcePaths.Length != 0)
                return false;

            String projRoot = Path.GetDirectoryName(project.ProjectPath);
            return projRoot != null && file.StartsWith(projRoot);
        }

        /// <summary>
        /// Gets all files related to the project
        /// </summary>
        private static List<String> GetAllProjectRelatedFiles(IProject project)
        {
            List<String> files = new List<String>();
            String filter = project.Language.ToLower() == "haxe" ? "*.hx" : "*.as";//TODO: loom, typescript
            foreach (String path in project.SourcePaths)
            {
                String absolute = project.GetAbsolutePath(path);
                if (Directory.Exists(path))
                    files.AddRange(Directory.GetFiles(absolute, filter, SearchOption.AllDirectories));
            }

            // If no source paths are defined, get files directly from project path
            if (project.SourcePaths.Length == 0)
            {
                String projRoot = Path.GetDirectoryName(project.ProjectPath);
                if(projRoot != null)
                    files.AddRange(Directory.GetFiles(projRoot, filter, SearchOption.AllDirectories));
            }

            return files;
        }


        /// <summary>
        /// Generates an FRSearch to find all instances of the given member name.
        /// Enables WholeWord and Match Case. No comment/string literal, escape characters, or regex searching.
        /// </summary>
        private static FRSearch GetFRSearch(string memberName)
        {
            String pattern = memberName;
            FRSearch search = new FRSearch(pattern)
            {
                IsRegex = false,
                IsEscaped = false,
                WholeWord = true,
                NoCase = false,
                Filter = SearchFilter.None | SearchFilter.OutsideCodeComments | SearchFilter.OutsideStringLiterals
            };

            return search;
        }

        /// <summary>
        /// Replaces only the matches in the current sci control
        /// </summary>
        public static void ReplaceMatches(IList<SearchMatch> matches, ScintillaControl sci, String replacement, String src)
        {
            if (sci == null || matches == null || matches.Count == 0)
                return;

            sci.BeginUndoAction();
            try
            {
                for (Int32 i = 0; i < matches.Count; i++)
                {
                    SelectMatch(sci, matches[i]);
                    FRSearch.PadIndexes((List<SearchMatch>)matches, i, matches[i].Value, replacement);
                    sci.EnsureVisible(sci.LineFromPosition(sci.MBSafePosition(matches[i].Index)));
                    sci.ReplaceSel(replacement);
                }
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        /// <summary>
        /// Selects a search match
        /// </summary>
        public static void SelectMatch(ScintillaControl sci, SearchMatch match)
        {
            if (sci == null || match == null)
                return;

            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.SetSel(start, end);
        }

        public static Boolean CanRenameWithFile(ASResult target)
        {
            if (target == null)
                return false;

            if (target.Type.IsEnum())
                return true;

            Boolean isVoid = target.Type.IsVoid();
            if (!isVoid && target.IsStatic && target.Member == null)
                return true;

            FlagType flags = target.Member.Flags;

            if (!isVoid && CheckFlag(flags, FlagType.Constructor))
                return true;

            if (CheckFlag(flags, FlagType.TypeDef))
                return true;

            if (target.InClass != null && !target.InClass.IsVoid())
                return false;

            return CheckFlag(flags, FlagType.Function) || CheckFlag(flags, FlagType.Namespace);
        }

    }
}