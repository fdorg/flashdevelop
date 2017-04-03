﻿using PluginCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeRefactor.Managers
{
    public class BatchProcessManager
    {
        private static List<IBatchProcessor> processors = new List<IBatchProcessor>();

        /// <summary>
        /// Adds <param name="processor" /> to be selectable from the BatchProcessDialog
        /// </summary>
        public static void AddBatchProcessor(IBatchProcessor processor)
        {
            processors.Add(processor);
        }
        
        /// <summary>
        /// Returns a list of available batch processors
        /// </summary>
        /// <returns></returns>
        public static IList<IBatchProcessor> GetAvailableProcessors()
        {
            var procs = new List<IBatchProcessor>();
            foreach (var processor in processors)
            {
                if (processor.IsAvailable)
                {
                    procs.Add(processor);
                }
            }
            return procs;
        }
    }

    public interface IBatchProcessor
    {
        string Text
        {
            get;
        }

        bool IsAvailable
        {
            get;
        }

        void Process(ITabbedDocument document);
    }
}
