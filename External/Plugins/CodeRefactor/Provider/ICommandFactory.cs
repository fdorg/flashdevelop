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
        /// A new DelegateMethodsCommand refactoring command.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="selectedMembers"></param>
        DelegateMethodsCommand CreateDelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers);

        /// <summary>
        /// A new Command refactoring command.
        /// Outputs found results.
        /// Uses the current selected text as the declaration target.
        /// </summary>
        Command CreateExtractLocalVariableCommand();

        /// <summary>
        /// A new ExtractLocalVariableCommand refactoring command.
        /// Uses the current text as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        Command CreateExtractLocalVariableCommand(bool outputResults);

        /// <summary>
        /// A new ExtractLocalVariableCommand refactoring command.
        /// Uses the current text as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        Command CreateExtractLocalVariableCommand(bool outputResults, string newName);

        /// <summary>
        /// A new ExtractMethodCommand refactoring command.
        /// </summary>
        /// <param name="newName"></param>
        ExtractMethodCommand CreateExtractMethodCommand(string newName);

        /// <summary>
        /// A new FindAllReferences refactoring command. Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        Command CreateFindAllReferencesCommand();

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        Command CreateFindAllReferencesCommand(bool output);

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        Command CreateFindAllReferencesCommand(ASResult target, bool output);

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations);

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        /// <param name="onlySourceFiles"></param>
        Command CreateFindAllReferencesCommand(ASResult target, bool output, bool ignoreDeclarations, bool onlySourceFiles);

        /// <summary>
        /// A new Move refactoring command.
        /// </summary>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath);

        /// <summary>
        /// A new Move refactoring command.
        /// </summary>
        /// <param name="oldPathToNewPath"></param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults);

        /// <summary>
        /// A new Move refactoring command.
        /// </summary>
        /// <param name="oldPathToNewPath"></param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="renaming"></param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming);

        /// <summary>
        /// A new Move refactoring command.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="renaming"></param>
        /// <param name="updatePackages"></param>
        Command CreateMoveCommand(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming, bool updatePackages);

        /// <summary>
        /// A new OrganizeImportsCommand refactoring command.
        /// </summary>
        Command CreateOrganizeImportsCommand();

        /// <summary>
        /// A new Rename refactoring command.
        /// Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        Command CreateRenameCommand();

        /// <summary>
        /// A new Rename refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        Command CreateRenameCommand(bool outputResults);

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        Command CreateRenameCommand(ASResult target, bool outputResults);

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        Command CreateRenameCommand(ASResult target, bool outputResults, string newName);

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="ignoreDeclarationSource">If true, will not rename the original declaration source.  Useful for Encapsulation refactoring.</param>
        Command CreateRenameCommand(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource);

        /// <summary>
        /// A new RenameFile refactoring command.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        Command CreateRenameFileCommand(string oldPath, string newPath);

        /// <summary>
        /// A new RenameFile refactoring command.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="newPath"></param>
        /// <param name="outputResults"></param>
        Command CreateRenameFileCommand(string oldPath, string newPath, bool outputResults);

        /// <summary>
        /// A new SurroundWith refactoring command.
        /// </summary>
        /// <param name="snippet"></param>
        SurroundWithCommand CreateSurroundWithCommand(string snippet);
    }
}