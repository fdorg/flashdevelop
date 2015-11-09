using System;
using CodeRefactor.Provider;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Basic underlying Refactoring command.  Refactoring commands can derive from this.
    /// </summary>
    /// <typeparam name="RefactorResultType">The refactoring results return type</typeparam>
    public abstract class RefactorCommand<RefactorResultType>
    {
        #region Events

        /// <summary>
        /// Fires when the refactoring command completes its operation.
        /// </summary>
        public EventHandler<RefactorCompleteEventArgs<RefactorResultType>> OnRefactorComplete;

        #endregion

        #region Fields and Properties

        private RefactorResultType results;
        private DocumentHelper associatedDocumentHelper;

        /// <summary>
        /// 
        /// </summary>
        public virtual RefactorResultType Results
        {
            get
            {
                return results;
            }
            protected set
            {
                results = value;
            }
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
                if (associatedDocumentHelper == null)
                {
                    this.RegisterNewDocumentHelper();
                }
                return associatedDocumentHelper;
            }
            private set
            {
                associatedDocumentHelper = value;
            }
        }

        /// <summary>
        /// Indicates if this command already has a DocumentHelper registered with it.
        /// </summary>
        public Boolean HasRegisteredDocumentHelper
        {
            get
            {
                return associatedDocumentHelper != null;
            }
        }

        /// <summary>
        /// Registers a new DocumentHelper to the command.
        /// </summary>
        /// <returns></returns>
        public DocumentHelper RegisterNewDocumentHelper()
        {
            return this.RegisterDocumentHelper(null);
        }

        /// <summary>
        /// Registers the provided DocumentHelper to the command.
        /// Useful in persisting a DocumentHelper across multiple commands, thus persisting the temporary file state.
        /// If NULL is passed, it will create a new DocumentHelper for the command.
        /// </summary>
        public DocumentHelper RegisterDocumentHelper(DocumentHelper existingDocumentHelper)
        {
            if (existingDocumentHelper != null)
            {
                AssociatedDocumentHelper = existingDocumentHelper;
            }
            else
            {
                AssociatedDocumentHelper = new DocumentHelper();
            }
            return AssociatedDocumentHelper;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Execute the refactoring command.
        /// </summary>
        public void Execute()
        {
            if (this.IsValid())
            {
                this.ExecutionImplementation();
            }
        }

        #endregion

        #region Concrete Implementation Helper Methods

        /// <summary>
        /// Allows for the concrete refactoring command implementations to fire off the OnRefactorComplete event.
        /// </summary>
        protected void FireOnRefactorComplete()
        {
            if (OnRefactorComplete != null)
            {
                OnRefactorComplete(this, new RefactorCompleteEventArgs<RefactorResultType>(this.results));
            }
        }

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
        public abstract Boolean IsValid();

        #endregion

    }

}
