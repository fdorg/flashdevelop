using System;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Event arguments provided when a refactoring command completes.
    /// </summary>
    /// <typeparam name="RefactorResultType">The type of the results</typeparam>
    public class RefactorCompleteEventArgs<RefactorResultType> : EventArgs
    {
        private readonly RefactorResultType results;
        
        /// <summary>
        /// 
        /// </summary>
        public virtual RefactorResultType Results => results;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="results"></param>
        public RefactorCompleteEventArgs(RefactorResultType resultType)
        {
            results = resultType;
        }

    }

}
