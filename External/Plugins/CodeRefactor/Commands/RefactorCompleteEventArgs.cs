// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Event arguments provided when a refactoring command completes.
    /// </summary>
    /// <typeparam name="RefactorResultType">The type of the results</typeparam>
    public class RefactorCompleteEventArgs<RefactorResultType> : EventArgs
    {
        private RefactorResultType results;
        
        /// <summary>
        /// 
        /// </summary>
        public virtual RefactorResultType Results
        {
            get
            {
                return results;
            }
        }

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
