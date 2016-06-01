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
            return new ExtractLocalVariableCommand();
        }

        public virtual Command CreateExtractLocalVariableCommand(bool outputResults)
        {
            return new ExtractLocalVariableCommand(outputResults);
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
            return new FindAllReferences();
        }

        public virtual Command CreateFindAllReferencesCommand(bool output)
        {
            return new FindAllReferences(output);
        }

        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output)
        {
            return new FindAllReferences(target, output);
        }

        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations)
        {
            return new FindAllReferences(target, output, ignoreDeclarations);
        }

        public Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
        {
            return new FindAllReferences(target, output, ignoreDeclarations) {OnlySourceFiles = onlySourceFiles};
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath)
        {
            return new Move(oldPathToNewPath);
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults)
        {
            return new Move(oldPathToNewPath, outputResults);
        }

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
        {
            return new Move(oldPathToNewPath, outputResults, renaming);
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
            return new Rename();
        }

        public virtual Command CreateRenameCommand(bool outputResults)
        {
            return new Rename(outputResults);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults)
        {
            return new Rename(target, outputResults);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, string newName)
        {
            return new Rename(target, outputResults, newName);
        }

        public virtual Command CreateRenameCommand(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource)
        {
            return new Rename(target, outputResults, newName, ignoreDeclarationSource);
        }

        public virtual Command CreateRenameFileCommand(string oldPath, string newPath)
        {
            return new RenameFile(oldPath, newPath);
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
