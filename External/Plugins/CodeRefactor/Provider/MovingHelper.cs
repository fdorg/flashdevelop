using CodeRefactor.Commands;
using PluginCore.Managers;
using System;
using System.Collections.Generic;

namespace CodeRefactor.Provider
{
    class MovingHelper
    {
        private static List<QueueItem> queue = new List<QueueItem>();
        private static Move currentCommand;

        /// <summary>
        /// 
        /// </summary>
        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath)
        {
            AddToQueue(oldPathToNewPath, true);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath, bool withMove)
        {
            queue.Add(new QueueItem(oldPathToNewPath, withMove));
            if (currentCommand == null) MoveFirst();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void MoveFirst()
        {
            try
            {
                QueueItem item = queue[0];
                Dictionary<string, string> oldPathToNewPath = item.oldPathToNewPath;
                queue.Remove(item);
                currentCommand = new Move(oldPathToNewPath, true, item.withMove);
                currentCommand.OnRefactorComplete += OnRefactorComplete;
                currentCommand.Execute();
            }
            catch(Exception ex)
            {
                queue.Clear();
                currentCommand = null;
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void OnRefactorComplete(object sender, RefactorCompleteEventArgs<IDictionary<string, List<PluginCore.FRService.SearchMatch>>> e)
        {
            if (queue.Count > 0) MoveFirst();
            else currentCommand = null;
        }
    }

    #region Helpers

    internal class QueueItem
    {
        public Dictionary<string, string> oldPathToNewPath;
        public bool withMove;

        public QueueItem(Dictionary<string, string> oldPathToNewPath, bool withMove)
        {
            this.oldPathToNewPath = oldPathToNewPath;
            this.withMove = withMove;
        }
    }

    #endregion
}