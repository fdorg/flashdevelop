// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Event arguments provided when a refactoring command completes.
    /// </summary>
    /// <typeparam name="TRefactorResultType">The type of the results</typeparam>
    public class RefactorCompleteEventArgs<TRefactorResultType> : EventArgs
    {
        public virtual TRefactorResultType Results { get; }

        /// <param name="results"></param>
        public RefactorCompleteEventArgs(TRefactorResultType resultType)
        {
            Results = resultType;
        }
    }
}