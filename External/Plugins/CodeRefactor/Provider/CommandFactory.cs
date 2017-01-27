using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using CodeRefactor.Commands;
using PluginCore.FRService;

namespace CodeRefactor.Provider
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    public class CommandFactory : ICommandFactory
    {
        public virtual DelegateMethodsCommand CreateDelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers)
        {
            return new DelegateMethodsCommand(result, selectedMembers);
        }

        public virtual Command CreateExtractLocalVariableCommand()
        {
            return CreateExtractLocalVariableCommand(true);
        }

        public virtual Command CreateExtractLocalVariableCommand(bool outputResults)
        {
            return CreateExtractLocalVariableCommand(outputResults, null);
        }

        public virtual Command CreateExtractLocalVariableCommand(bool outputResults, string newName)
        {
            return new ExtractLocalVariableCommand(outputResults, newName);
        }

        public virtual ExtractMethodCommand CreateExtractMethodCommand(string newName)
        {
            return new ExtractMethodCommand(newName);
        }

        /// <summary>
        /// Create a new FindAllReferences refactoring command. Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public virtual Command CreateFindAllReferencesCommand()
        {
            return CreateFindAllReferencesCommand(true);
        }

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public virtual Command CreateFindAllReferencesCommand(bool output)
        {
            return CreateFindAllReferencesCommand(RefactoringHelper.GetDefaultRefactorTarget(), output);
        }

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output)
        {
            return CreateFindAllReferencesCommand(target, output, false);
        }

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations)
        {
            return CreateFindAllReferencesCommand(target, output, ignoreDeclarations, true);
        }

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        /// <param name="onlySourceFiles"></param>
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
        {
            return new FindAllReferences(target, output, ignoreDeclarations) {OnlySourceFiles = onlySourceFiles};
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath)
        {
            return CreateMoveCommand(oldPathToNewPath, true);
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults)
        {
            return CreateMoveCommand(oldPathToNewPath, outputResults, false);
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
        {
            return CreateMoveCommand(oldPathToNewPath, outputResults, renaming, false);
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming, bool updatePackages)
        {
            return new Move(oldPathToNewPath, outputResults, renaming, updatePackages);
        }

        public virtual Command CreateOrganizeImportsCommand()
        {
            return new OrganizeImports();
        }

        public virtual Command CreateRenameCommand()
        {
            return CreateRenameCommand(true);
        }

        public virtual Command CreateRenameCommand(bool outputResults, bool inline = false)
        {
            return CreateRenameCommand(RefactoringHelper.GetDefaultRefactorTarget(), outputResults, inline);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, bool inline = false)
        {
            return CreateRenameCommand(target, outputResults, null, inline);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, string newName, bool inline = false)
        {
            return CreateRenameCommand(target, outputResults, newName, false, inline);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource, bool inline = false)
        {
            return new Rename(target, outputResults, newName, ignoreDeclarationSource, inline);
        }

        public virtual Command CreateRenameFileCommand(string oldPath, string newPath)
        {
            return CreateRenameFileCommand(oldPath, newPath, true);
        }

        public virtual Command CreateRenameFileCommand(string oldPath, string newPath, bool outputResults)
        {
            return new RenameFile(oldPath, newPath, outputResults);
        }

        public virtual SurroundWithCommand CreateSurroundWithCommand(string snippet)
        {
            return new SurroundWithCommand(snippet);
        }
    }
}
