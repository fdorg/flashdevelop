using System;
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
        public virtual DelegateMethods CreateDelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers)
        {
            return new DelegateMethods(result, selectedMembers);
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

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand()
        {
            return CreateFindAllReferencesCommand(true);
        }

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(bool output)
        {
            return CreateFindAllReferencesCommand(RefactoringHelper.GetDefaultRefactorTarget(), output);
        }

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output)
        {
            return CreateFindAllReferencesCommand(target, output, false);
        }

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations)
        {
            return CreateFindAllReferencesCommand(target, output, ignoreDeclarations, true);
        }

        /// <inheritdoc />
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

        public virtual Command CreateRenameCommandAndExecute()
        {
            return CreateRenameCommandAndExecute(true);
        }

        public virtual Command CreateRenameCommandAndExecute(bool outputResults, bool inline = false)
        {
            return CreateRenameCommandAndExecute(RefactoringHelper.GetDefaultRefactorTarget(), outputResults, inline);
        }

        public virtual Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, bool inline = false)
        {
            return CreateRenameCommandAndExecute(target, outputResults, null, inline);
        }

        public virtual Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, string newName, bool inline = false)
        {
            return CreateRenameCommandAndExecute(target, outputResults, newName, false, inline);
        }

        public virtual Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource, bool inline = false)
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

        readonly Dictionary<Type, Func<ASResult, bool>> commandToValidator = new Dictionary<Type, Func<ASResult, bool>>();

        public void RegisterValidator(Type command, Func<ASResult, bool> validator) => commandToValidator[command] = validator;
        public Func<ASResult, bool> GetValidator(Type command) => commandToValidator.ContainsKey(command) ? commandToValidator[command] : null;
    }
}
