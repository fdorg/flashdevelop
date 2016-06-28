using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using CodeRefactor.Commands;
using PluginCore.FRService;

namespace CodeRefactor.Provider
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    class CommandFactory : ICommandFactory
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

        public virtual Command CreateFindAllReferencesCommand()
        {
            return CreateFindAllReferencesCommand(true);
        }

        public virtual Command CreateFindAllReferencesCommand(bool output)
        {
            return CreateFindAllReferencesCommand(RefactoringHelper.GetDefaultRefactorTarget(), output);
        }

        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output)
        {
            return CreateFindAllReferencesCommand(target, output, false);
        }

        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations)
        {
            return CreateFindAllReferencesCommand(target, output, ignoreDeclarations, true);
        }

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

        public virtual Command CreateRenameCommand(bool outputResults)
        {
            return CreateRenameCommand(RefactoringHelper.GetDefaultRefactorTarget(), outputResults);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults)
        {
            return CreateRenameCommand(target, outputResults, null);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, string newName)
        {
            return CreateRenameCommand(target, outputResults, newName, false);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource)
        {
            return new Rename(target, outputResults, newName, ignoreDeclarationSource);
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
