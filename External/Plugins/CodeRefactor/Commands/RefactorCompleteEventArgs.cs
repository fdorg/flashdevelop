using System;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Event arguments provided when a refactoring command completes.
    /// </summary>
    /// <typeparam name="TRefactorResultType">The type of the results</typeparam>
    public class RefactorCompleteEventArgs<TRefactorResultType> : EventArgs
    {
        private readonly TRefactorResultType _results;
        
        /// <summary>
        /// 
        /// </summary>
        public virtual TRefactorResultType Results
        {
            get
            {
                return _results;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultType"></param>
        public RefactorCompleteEventArgs(TRefactorResultType resultType)
        {
            _results = resultType;
        }

    }
}