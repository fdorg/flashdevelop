using CodeRefactor.Commands;
using PluginCore.Managers;
using System;
using System.Collections.Generic;

namespace CodeRefactor.Provider
{
    class MovingHelper
    {
        private static List<Dictionary<string, string>> queue = new List<Dictionary<string, string>>();
        private static Move currentCommand;

        /// <summary>
        /// 
        /// </summary>
        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath)
        {
            queue.Add(oldPathToNewPath);
            if (currentCommand == null) MoveFirst();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void MoveFirst()
        {
            try
            {
                Dictionary<string, string> oldPathToNewPath = queue[0];
                queue.Remove(oldPathToNewPath);
                currentCommand = new Move(oldPathToNewPath);
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
}