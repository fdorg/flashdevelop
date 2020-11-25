using System;
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using CodeRefactor.Commands;
using PluginCore.FRService;

namespace CodeRefactor.Provider
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    public interface ICommandFactory
    {
        /// <summary>
        /// Create a new DelegateMethodsCommand refactoring command.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="selectedMembers"></param>
        DelegateMethods CreateDelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers);

        /// <summary>
        /// Create a new Command refactoring command.
        /// Outputs found results.
        /// Uses the current selected text as the declaration target.
        /// </summary>
        Command CreateExtractLocalVariableCommand();

        /// <summary>
        /// Create a new ExtractLocalVariableCommand refactoring command.
        /// Uses the current text as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        Command CreateExtractLocalVariableCommand(bool outputResults);

        /// <summary>
        /// Create a new ExtractLocalVariableCommand refactoring command.
        /// Uses the current text as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        Command CreateExtractLocalVariableCommand(bool outputResults, string newName);

        /// <summary>
        /// Create a new ExtractMethodCommand refactoring command.
        /// </summary>
        /// <param name="newName"></param>
        ExtractMethodCommand CreateExtractMethodCommand(string newName);

        /// <summary>
        /// Create a new FindAllReferences refactoring command. Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        Command CreateFindAllReferencesCommand();

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        Command CreateFindAllReferencesCommand(bool output);

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        Command CreateFindAllReferencesCommand(ASResult target, bool output);

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations);

        /// <summary>
        /// Create a new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        /// <param name="onlySourceFiles"></param>
        Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles);

        /// <summary>
        /// Create a new Move refactoring command.
        /// </summary>
        /// <param name="oldPathToNewPath"></param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath);

        /// <summary>
        /// Create a new Move refactoring command.
        /// </summary>
        /// <param name="oldPathToNewPath"></param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults);

        /// <summary>
        /// Create a new Move refactoring command.
        /// </summary>
        /// <param name="oldPathToNewPath"></param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="renaming"></param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming);

        /// <summary>
        /// Create a new Move refactoring command.
        /// </summary>
        /// <param name="oldPathToNewPath"></param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="renaming"></param>
        /// <param name="updatePackages"></param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming, bool updatePackages);

        /// <summary>
        /// Create a new OrganizeImportsCommand refactoring command.
        /// </summary>
        Command CreateOrganizeImportsCommand();

        /// <summary>
        /// Create a new Rename refactoring command.
        /// Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        Command CreateRenameCommandAndExecute();

        /// <summary>
        /// Create a new Rename refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        Command CreateRenameCommandAndExecute(bool outputResults, bool inline = false);

        /// <summary>
        /// Create a new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, bool inline = false);

        /// <summary>
        /// Create a new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, string newName, bool inline = false);

        /// <summary>
        /// Create a new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="ignoreDeclarationSource">If true, will not rename the original declaration source.  Useful for Encapsulation refactoring.</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        Command CreateRenameCommandAndExecute(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource, bool inline = false);

        /// <summary>
        /// Create a new RenameFile refactoring command.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        Command CreateRenameFileCommand(string oldPath, string newPath);

        /// <summary>
        /// Create a new RenameFile refactoring command.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <param name="outputResults"></param>
        Command CreateRenameFileCommand(string oldPath, string newPath, bool outputResults);

        /// <summary>
        /// Create a new SurroundWith refactoring command.
        /// </summary>
        /// <param name="snippet"></param>
        SurroundWithCommand CreateSurroundWithCommand(string snippet);

        void RegisterValidator(Type command, Func<ASResult, bool> validator);

        Func<ASResult, bool> GetValidator(Type command);
    }
}