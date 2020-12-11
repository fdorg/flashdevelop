// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            => new DelegateMethods(result, selectedMembers);

        public virtual Command CreateExtractLocalVariableCommand()
            => CreateExtractLocalVariableCommand(true);

        public virtual Command CreateExtractLocalVariableCommand(bool outputResults)
            => CreateExtractLocalVariableCommand(outputResults, null);

        public virtual Command CreateExtractLocalVariableCommand(bool outputResults, string newName)
            => new ExtractLocalVariableCommand(outputResults, newName);

        public virtual ExtractMethodCommand CreateExtractMethodCommand(string newName)
            => new ExtractMethodCommand(newName);

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand()
            => CreateFindAllReferencesCommand(true);

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(bool output)
            => CreateFindAllReferencesCommand(RefactoringHelper.GetDefaultRefactorTarget(), output);

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output)
            => CreateFindAllReferencesCommand(target, output, false);

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations)
            => CreateFindAllReferencesCommand(target, output, ignoreDeclarations, true);

        /// <inheritdoc />
        public virtual Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles)
            => new FindAllReferences(target, output, ignoreDeclarations) {OnlySourceFiles = onlySourceFiles};

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath)
            => CreateMoveCommand(oldPathToNewPath, true);

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults)
            => CreateMoveCommand(oldPathToNewPath, outputResults, false);

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
            => CreateMoveCommand(oldPathToNewPath, outputResults, renaming, false);

        public virtual Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming, bool updatePackages)
            => new Move(oldPathToNewPath, outputResults, renaming, updatePackages);

        public virtual Command CreateOrganizeImportsCommand()
            => new OrganizeImports();

        public virtual Command CreateRenameCommandAndExecute()
            => CreateRenameCommandAndExecute(true);

        public virtual Command CreateRenameCommandAndExecute(bool outputResults, bool inline = false)
            => CreateRenameCommandAndExecute(RefactoringHelper.GetDefaultRefactorTarget(), outputResults, inline);

        public virtual Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, bool inline = false)
            => CreateRenameCommandAndExecute(target, outputResults, null, inline);

        public virtual Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, string newName, bool inline = false)
            => CreateRenameCommandAndExecute(target, outputResults, newName, false, inline);

        public virtual Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource, bool inline = false)
            => new Rename(target, outputResults, newName, ignoreDeclarationSource, inline);

        public virtual Command CreateRenameFileCommand(string oldPath, string newPath)
            => CreateRenameFileCommand(oldPath, newPath, true);

        public virtual Command CreateRenameFileCommand(string oldPath, string newPath, bool outputResults)
            => new RenameFile(oldPath, newPath, outputResults);

        public virtual SurroundWithCommand CreateSurroundWithCommand(string snippet)
            => new SurroundWithCommand(snippet);

        readonly Dictionary<Type, Func<ASResult, bool>> commandToValidator = new Dictionary<Type, Func<ASResult, bool>>();

        public void RegisterValidator(Type command, Func<ASResult, bool> validator) => commandToValidator[command] = validator;

        public Func<ASResult, bool>? GetValidator(Type command) => commandToValidator.TryGetValue(command, out var result) ? result : null;
    }
}
