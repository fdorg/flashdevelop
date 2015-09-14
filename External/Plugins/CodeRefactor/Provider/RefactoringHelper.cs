using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Managers;
using ProjectManager.Projects;
using ScintillaNet;

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
                    {
                        searchResults[entry.Key].Add(match);
                    }
                }
            }
            return searchResults;
        }

        /// <summary>
        /// Gets if the language is valid for refactoring
        /// </summary>
        public static Boolean GetLanguageIsValid()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return false;
            string lang = document.SciControl.ConfigurationLanguage;
            return lang == "as2" || lang == "as3" || lang == "haxe" || lang == "loom"; // TODO: look for /Snippets/Generators
        }

        /// <summary>
        /// Checks if the model is not null and file exists
        /// </summary>
        public static Boolean ModelFileExists(FileModel model)
        {
            if (model != null && File.Exists(model.FileName)) return true;
            else return false;
        }

        /// <summary>
        /// Checks if the file is under the current SDK
        /// </summary>
        public static Boolean IsUnderSDKPath(FileModel model)
        {
            return IsUnderSDKPath(model.FileName);
        }
        public static Boolean IsUnderSDKPath(String file)
        {
            InstalledSDK sdk = PluginBase.CurrentSDK;
            if (sdk != null && !String.IsNullOrEmpty(sdk.Path) && file.StartsWith(sdk.Path)) return true;
            return false;
        }

        /// <summary>
        /// Retrieves the refactoring target based on the current location.
        /// Note that this will look up to the declaration source.  
        /// This allows users to execute the rename from a reference to the source rather than having to navigate to the source first.
        /// </summary>
        public static ASResult GetDefaultRefactorTarget()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (!ASContext.Context.IsFileValid || (sci == null)) return null;
            int position = sci.WordEndPosition(sci.CurrentPos, true);
            return DeclarationLookupResult(sci, position);
        }

        /// <summary>
        /// Retrieves the refactoring target based on the file.
        /// </summary>
        public static ASResult GetRefactorTargetFromFile(string path, DocumentHelper associatedDocumentHelper)
        {
            String fileName = Path.GetFileNameWithoutExtension(path);
            Int32 line = 0;
            var doc = associatedDocumentHelper.LoadDocument(path);
            ScintillaControl sci = doc != null ? doc.SciControl : null;
            if (sci == null) return null; // Should not happen...
            List<ClassModel> classes = ASContext.Context.CurrentModel.Classes;
            if (classes.Count > 0)
            {
                foreach (ClassModel classModel in classes)
                {
                    if (classModel.Name.Equals(fileName))
                    {
                        // Optimization, we don't need to make a full lookup in this case
                        return new ASResult
                        {
                            IsStatic = true,
                            Type = classModel
                        };
                    }
                }
            }
            else
            {
                foreach (MemberModel member in ASContext.Context.CurrentModel.Members)
                {
                    if (member.Name.Equals(fileName))
                    {
                        line = member.LineFrom;
                        break;
                    }
                }
            }
            if (line > 0)
            {
                sci.SelectText(fileName, sci.PositionFromLine(line));
                return GetDefaultRefactorTarget();
            }

            return null;
        }

        /// <summary>
        /// Checks if a given search match actually points to the given target source
        /// </summary>
        /// <returns>True if the SearchMatch does point to the target source.</returns>
        public static ASResult DeclarationLookupResult(ScintillaControl Sci, int position)
        {
            if (!ASContext.Context.IsFileValid || (Sci == null)) return null;
            // get type at cursor position
            ASResult result = ASComplete.GetExpressionType(Sci, position);
            if (result.IsPackage) return result;
            // open source and show declaration
            if (!result.IsNull())
            {
                if (result.Member != null && (result.Member.Flags & FlagType.AutomaticVar) > 0) return null;
                FileModel model = result.InFile ?? ((result.Member != null && result.Member.InFile != null) ? result.Member.InFile : null) ?? ((result.Type != null) ? result.Type.InFile : null);
                if (model == null || model.FileName == "") return null;
                ClassModel inClass = result.InClass ?? result.Type;
                // for Back command
                int lookupLine = Sci.CurrentLine;
                int lookupCol = Sci.CurrentPos - Sci.PositionFromLine(lookupLine);
                ASContext.Panel.SetLastLookupPosition(ASContext.Context.CurrentFile, lookupLine, lookupCol);
                // open the file
                if (model != ASContext.Context.CurrentModel)
                {
                    if (model.FileName.Length > 0 && File.Exists(model.FileName))
                    {
                        ASContext.MainForm.OpenEditableDocument(model.FileName, false);
                    }
                    else
                    {
                        ASComplete.OpenVirtualFile(model);
                        result.InFile = ASContext.Context.CurrentModel;
                        if (result.InFile == null) return null;
                        if (inClass != null)
                        {
                            inClass = result.InFile.GetClassByName(inClass.Name);
                            if (result.Member != null) result.Member = inClass.Members.Search(result.Member.Name, 0, 0);
                        }
                        else if (result.Member != null)
                        {
                            result.Member = result.InFile.Members.Search(result.Member.Name, 0, 0);
                        }
                    }
                }
                if ((inClass == null || inClass.IsVoid()) && result.Member == null) return null;
                Sci = ASContext.CurSciControl;
                if (Sci == null) return null;
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
                else if (inClass.LineFrom > 0)
                {
                    line = inClass.LineFrom;
                    name = inClass.Name;
                    isClass = true;
                    // constructor
                    foreach (MemberModel member in inClass.Members)
                    {
                        if ((member.Flags & FlagType.Constructor) > 0)
                        {
                            line = member.LineFrom;
                            name = member.Name;
                            isClass = false;
                            break;
                        }
                    }
                }
                if (line > 0) // select
                {
                    if (isClass) ASComplete.LocateMember("(class|interface)", name, line);
                    else ASComplete.LocateMember("(function|var|const|get|set|property|[,(])", name, line);
                }
                return result;
            }
            return null;
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
        public static bool IsMatchTheTarget(ScintillaControl Sci, SearchMatch match, ASResult target)
        {
            if (Sci == null || target == null || target.InFile == null || target.Member == null)
            {
                return false;
            }
            String originalFile = Sci.FileName;
            // get type at match position
            ASResult declaration = DeclarationLookupResult(Sci, Sci.MBSafePosition(match.Index) + Sci.MBSafeTextLength(match.Value));
            return (declaration.InFile != null && originalFile == declaration.InFile.FileName) && (Sci.CurrentPos == (Sci.MBSafePosition(match.Index) + Sci.MBSafeTextLength(match.Value)));
        }

        /// <summary>
        /// Checks if a given search match actually points to the given target source
        /// </summary>
        /// <returns>True if the SearchMatch does point to the target source.</returns>
        public static bool DoesMatchPointToTarget(ScintillaControl Sci, SearchMatch match, ASResult target, DocumentHelper associatedDocumentHelper)
        {
            if (Sci == null || target == null) return false;
            FileModel targetInFile = null;

            if (target.InFile != null)
                targetInFile = target.InFile;
            else if (target.Member != null && target.InClass == null)
                targetInFile = target.Member.InFile;

            Boolean matchMember = targetInFile != null && target.Member != null;
            Boolean matchType = target.Member == null && target.IsStatic && target.Type != null;
            if (!matchMember && !matchType) return false;

            ASResult result = null;
            // get type at match position
            if (match.Index < Sci.Text.Length) // TODO: find out rare cases of incorrect index reported
            {
                result = DeclarationLookupResult(Sci, Sci.MBSafePosition(match.Index) + Sci.MBSafeTextLength(match.Value));
                if (associatedDocumentHelper != null)
                {
                    // because the declaration lookup opens a document, we should register it with the document helper to be closed later
                    associatedDocumentHelper.RegisterLoadedDocument(PluginBase.MainForm.CurrentDocument);
                }
            }
            // check if the result matches the target
            if (result == null || (result.InFile == null && result.Type == null)) return false;
            if (matchMember)
            {
                if (result.Member == null) return false;

                var resultInFile = result.InClass != null ? result.InFile : result.Member.InFile;

                return resultInFile.BasePath == targetInFile.BasePath
                    && resultInFile.FileName == targetInFile.FileName
                    && result.Member.LineFrom == target.Member.LineFrom
                    && result.Member.Name == target.Member.Name;
            }
            else // type
            {
                if (result.Type == null) return false;
                if (result.Type.QualifiedName == target.Type.QualifiedName) return true;
                return false;
            }
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
            return FindTargetInFiles(target, progressReportHandler, findFinishedHandler, asynchronous, false, false);
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
        /// <param name="onlySourceFiles">searches only on defined classpaths</param>
        /// <returns>If "asynchronous" is false, will return the search results, otherwise returns null on bad input or if running in asynchronous mode.</returns>
        public static FRResults FindTargetInFiles(ASResult target, FRProgressReportHandler progressReportHandler, FRFinishedHandler findFinishedHandler, Boolean asynchronous, Boolean onlySourceFiles, Boolean ignoreSdkFiles)
        {
            Boolean currentFileOnly = false;
            // checks target is a member
            if (target == null || ((target.Member == null || String.IsNullOrEmpty(target.Member.Name))
                && (target.Type == null || !CheckFlag(target.Type.Flags, FlagType.Class) && !target.Type.IsEnum())))
            {
                return null;
            }
            else
            {
                // if the target we are trying to rename exists as a local variable or a function parameter we only need to search the current file
                if (target.Member != null && (
                        target.Member.Access == Visibility.Private
                        || CheckFlag(target.Member.Flags, FlagType.LocalVar)
                        || CheckFlag(target.Member.Flags, FlagType.ParameterVar))
                    )
                {
                    currentFileOnly = true;
                }
            }
            FRConfiguration config;
            IProject project = PluginBase.CurrentProject;
            String file = PluginBase.MainForm.CurrentDocument.FileName;
            // This is out of the project, just look for this file...
            if (currentFileOnly || !IsProjectRelatedFile(project, file))
            {
                String mask = Path.GetFileName(file);
                String path = Path.GetDirectoryName(file);
                if (mask.Contains("[model]"))
                {
                    if (findFinishedHandler != null)
                    {
                        findFinishedHandler(new FRResults());
                    }
                    return null;
                }
                config = new FRConfiguration(path, mask, false, GetFRSearch(target.Member != null ? target.Member.Name : target.Type.Name));
            }
            else if (target.Member != null && !CheckFlag(target.Member.Flags, FlagType.Constructor))
            {
                config = new FRConfiguration(GetAllProjectRelatedFiles(project, onlySourceFiles, ignoreSdkFiles), GetFRSearch(target.Member.Name));
            }
            else
            {
                target.Member = null;
                config = new FRConfiguration(GetAllProjectRelatedFiles(project, onlySourceFiles, ignoreSdkFiles), GetFRSearch(target.Type.Name));
            }
            config.CacheDocuments = true;
            FRRunner runner = new FRRunner();
            if (progressReportHandler != null)
            {
                runner.ProgressReport += progressReportHandler;
            }
            if (findFinishedHandler != null)
            {
                runner.Finished += findFinishedHandler;
            }
            if (asynchronous) runner.SearchAsync(config);
            else return runner.SearchSync(config);
            return null;
        }

        /// <summary>
        /// Checks if files is related to the project
        /// TODO support SWCs -> refactor test as IProject method
        /// </summary>
        public static Boolean IsProjectRelatedFile(IProject project, String file)
        {
            if (project == null) return false;
            IASContext context = ASContext.GetLanguageContext(project.Language);
            if (context == null) return false;
            foreach (PathModel pathModel in context.Classpath)
            {
                string absolute = project.GetAbsolutePath(pathModel.Path);
                if (file.StartsWith(absolute)) return true;
            }
            // If no source paths are defined, is it under the project?
            if (project.SourcePaths.Length == 0)
            {
                String projRoot = Path.GetDirectoryName(project.ProjectPath);
                if (file.StartsWith(projRoot)) return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all files related to the project
        /// </summary>
        private static List<String> GetAllProjectRelatedFiles(IProject project, bool onlySourceFiles)
        {
            return GetAllProjectRelatedFiles(project, onlySourceFiles, false);
        }
        private static List<String> GetAllProjectRelatedFiles(IProject project, bool onlySourceFiles, Boolean ignoreSdkFiles)
        {
            List<String> files = new List<String>();
            string filter = project.DefaultSearchFilter;
            if (string.IsNullOrEmpty(filter)) return files;
            string[] filters = project.DefaultSearchFilter.Split(';');
            if (!onlySourceFiles)
            {
                IASContext context = ASContext.GetLanguageContext(project.Language);
                if (context == null) return files;
                foreach (PathModel pathModel in context.Classpath)
                {
                    string absolute = project.GetAbsolutePath(pathModel.Path);
                    if (Directory.Exists(absolute))
                    {
                        if (ignoreSdkFiles && IsUnderSDKPath(absolute)) continue;
                        foreach (string filterMask in filters)
                        {
                            files.AddRange(Directory.GetFiles(absolute, filterMask, SearchOption.AllDirectories));
                        }
                    }
                }
            }
            else
            {
                var lookupPaths = project.SourcePaths.
                    Concat(ProjectManager.PluginMain.Settings.GetGlobalClasspaths(project.Language)).
                    Select(project.GetAbsolutePath).Distinct();
                foreach (string path in lookupPaths)
                {
                    if (Directory.Exists(path))
                    {
                        if (ignoreSdkFiles && IsUnderSDKPath(path)) continue;
                        foreach (string filterMask in filters)
                        {
                            files.AddRange(Directory.GetFiles(path, filterMask, SearchOption.AllDirectories));
                        }
                    }
                }
            }
            // If no source paths are defined, get files directly from project path
            if (project.SourcePaths.Length == 0)
            {
                String projRoot = Path.GetDirectoryName(project.ProjectPath);
                foreach (string filterMask in filters)
                {
                    files.AddRange(Directory.GetFiles(projRoot, filterMask, SearchOption.AllDirectories));
                }
            }
            return files;
        }

        /// <summary>
        /// Generates an FRSearch to find all instances of the given member name.
        /// Enables WholeWord and Match Case. No comment/string literal, escape characters, or regex searching.
        /// </summary>
        private static FRSearch GetFRSearch(string memberName)
        {
            FRSearch search = new FRSearch(memberName);
            search.IsRegex = false;
            search.IsEscaped = false;
            search.WholeWord = true;
            search.NoCase = false;
            search.Filter = SearchFilter.None | SearchFilter.OutsideCodeComments | SearchFilter.OutsideStringLiterals;
            return search;
        }

        /// <summary>
        /// Replaces only the matches in the current sci control
        /// </summary>
        public static void ReplaceMatches(IList<SearchMatch> matches, ScintillaControl sci, String replacement)
        {
            if (sci == null || matches == null || matches.Count == 0) return;
            sci.BeginUndoAction();
            try
            {
                for (int i = 0, matchCount = matches.Count; i < matchCount; i++)
                {
                    var match = matches[i];
                    SelectMatch(sci, match);
                    FRSearch.PadIndexes((List<SearchMatch>)matches, i, match.Value, replacement);
                    sci.EnsureVisible(sci.LineFromPosition(sci.MBSafePosition(match.Index)));
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
            if (sci == null || match == null) return;
            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.SetSel(start, end);
        }

        /// <summary>
        /// Copies found file or directory based on the specified paths.
        /// If affected file was designated as a Document Class, updates project accordingly.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public static void Copy(string oldPath, string newPath)
        {
            Copy(oldPath, newPath, true);
        }
        public static void Copy(string oldPath, string newPath, bool renaming)
        {
            Copy(oldPath, newPath, renaming, true);
        }
        public static void Copy(string oldPath, string newPath, bool renaming, bool simulateMove)
        {
            if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath)) return;
            Project project = (Project)PluginBase.CurrentProject;
            string newDocumentClass = null;

            if (File.Exists(oldPath) && FileHelper.ConfirmOverwrite(newPath))
            {
                File.Copy(oldPath, newPath, true);
                if (simulateMove)
                {
                    DocumentManager.MoveDocuments(oldPath, newPath);
                    if (project.IsDocumentClass(oldPath)) newDocumentClass = newPath;
                }
            }
            else if (Directory.Exists(oldPath))
            {
                newPath = renaming ? Path.Combine(Path.GetDirectoryName(oldPath), newPath) : Path.Combine(newPath, Path.GetFileName(oldPath));
                if (!FileHelper.ConfirmOverwrite(newPath)) return;
                string searchPattern = project.DefaultSearchFilter;
                if (simulateMove)
                {
                    // We need to use our own method for moving directories if folders in the new path already exist
                    FileHelper.CopyDirectory(oldPath, newPath, true);
                    foreach (string pattern in searchPattern.Split(';'))
                    {
                        foreach (string file in Directory.GetFiles(oldPath, pattern, SearchOption.AllDirectories))
                        {
                            if (project.IsDocumentClass(file))
                            {
                                newDocumentClass = file.Replace(oldPath, newPath);
                                break;
                            }
                            DocumentManager.MoveDocuments(oldPath, newPath);
                        }
                        if (newDocumentClass != null) break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(newDocumentClass))
            {
                project.SetDocumentClass(newDocumentClass, true);
                project.Save();
            }
        }

        /// <summary>
        /// Moves found file or directory based on the specified paths.
        /// If affected file was designated as a Document Class, updates project accordingly.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        public static void Move(string oldPath, string newPath)
        {
            Move(oldPath, newPath, true);
        }
        public static void Move(string oldPath, string newPath, bool renaming)
        {
            if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath)) return;
            Project project = (Project)PluginBase.CurrentProject;
            string newDocumentClass = null;

            if (File.Exists(oldPath) && FileHelper.ConfirmOverwrite(newPath))
            {
                FileHelper.ForceMove(oldPath, newPath);
                DocumentManager.MoveDocuments(oldPath, newPath);
                if (project.IsDocumentClass(oldPath)) newDocumentClass = newPath;
            }
            else if (Directory.Exists(oldPath))
            {
                newPath = renaming ? Path.Combine(Path.GetDirectoryName(oldPath), newPath) : Path.Combine(newPath, Path.GetFileName(oldPath));
                if (!FileHelper.ConfirmOverwrite(newPath)) return;
                string searchPattern = project.DefaultSearchFilter;
                foreach (string pattern in searchPattern.Split(';'))
                {
                    foreach (string file in Directory.GetFiles(oldPath, pattern, SearchOption.AllDirectories))
                    {
                        if (project.IsDocumentClass(file))
                        {
                            newDocumentClass = file.Replace(oldPath, newPath);
                            break;
                        }
                    }
                    if (newDocumentClass != null) break;
                }
                // We need to use our own method for moving directories if folders in the new path already exist
                FileHelper.ForceMoveDirectory(oldPath, newPath);
                DocumentManager.MoveDocuments(oldPath, newPath);
            }
            if (!string.IsNullOrEmpty(newDocumentClass))
            {
                project.SetDocumentClass(newDocumentClass, true);
                project.Save();
            }
        }

    }

}
