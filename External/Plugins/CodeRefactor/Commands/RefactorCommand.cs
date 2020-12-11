// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using ASCompletion.Completion;
using CodeRefactor.Provider;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Basic underlying Refactoring command.  Refactoring commands can derive from this.
    /// </summary>
    /// <typeparam name="TRefactorResultType">The refactoring results return type</typeparam>
    public abstract class RefactorCommand<TRefactorResultType>
    {
        #region Events

        /// <summary>
        /// Fires when the refactoring command completes its operation.
        /// </summary>
        public EventHandler<RefactorCompleteEventArgs<TRefactorResultType>> OnRefactorComplete;

        #endregion

        #region Fields and Properties

        /// <summary>
        /// The current declaration target that references are being found to.
        /// </summary>
        public ASResult CurrentTarget { get; protected set; }


        public bool OutputResults { get; protected set; }

        TRefactorResultType results;
        DocumentHelper associatedDocumentHelper;

        /// <summary>
        /// 
        /// </summary>
        public virtual TRefactorResultType Results
        {
            get => results;
            protected set => results = value;
        }

        /// <summary>
        /// A DocumentHelper that keeps track of temporarily opened files.
        /// Note: if the command currently does not have one associated with it, this will create a new one!
        /// If you need to check if it has a DocumentHelper registered with it, check the HasRegisteredDocumentHelper property.
        /// </summary>
        protected DocumentHelper AssociatedDocumentHelper
        {
            get
            {
                if (associatedDocumentHelper is null) RegisterNewDocumentHelper();
                return associatedDocumentHelper;
            }
            private set => associatedDocumentHelper = value;
        }

        /// <summary>
        /// Indicates if this command already has a DocumentHelper registered with it.
        /// </summary>
        public bool HasRegisteredDocumentHelper => associatedDocumentHelper != null;

        /// <summary>
        /// Registers a new DocumentHelper to the command.
        /// </summary>
        /// <returns></returns>
        public DocumentHelper RegisterNewDocumentHelper() => RegisterDocumentHelper(null);

        /// <summary>
        /// Registers the provided DocumentHelper to the command.
        /// Useful in persisting a DocumentHelper across multiple commands, thus persisting the temporary file state.
        /// If NULL is passed, it will create a new DocumentHelper for the command.
        /// </summary>
        public DocumentHelper RegisterDocumentHelper(DocumentHelper existingDocumentHelper)
        {
            AssociatedDocumentHelper = existingDocumentHelper ?? new DocumentHelper();
            return AssociatedDocumentHelper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute the refactoring command.
        /// </summary>
        public void Execute()
        {
            if (IsValid()) ExecutionImplementation();
        }

        #endregion

        #region Concrete Implementation Helper Methods

        /// <summary>
        /// Allows for the concrete refactoring command implementations to fire off the OnRefactorComplete event.
        /// </summary>
        protected void FireOnRefactorComplete() => OnRefactorComplete?.Invoke(this, new RefactorCompleteEventArgs<TRefactorResultType>(results));

        #endregion

        #region Concrete Implementation

        /// <summary>
        /// The concrete refactoring command execution implementation.
        /// This should be overridden by the derived classes with their custom refactoring logic.
        /// </summary>
        protected abstract void ExecutionImplementation();

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public abstract bool IsValid();

        #endregion
    }
}