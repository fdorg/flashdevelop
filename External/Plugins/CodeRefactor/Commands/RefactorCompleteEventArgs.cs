using System;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Event arguments provided when a refactoring command completes.
    /// </summary>
    /// <typeparam name="TRefactorResultType">The type of the results</typeparam>
    public class RefactorCompleteEventArgs<TRefactorResultType> : EventArgs
    {
        /// <param name="results"></param>
        public RefactorCompleteEventArgs(TRefactorResultType results) => Results = results;

        public virtual TRefactorResultType Results { get; }
    }
}