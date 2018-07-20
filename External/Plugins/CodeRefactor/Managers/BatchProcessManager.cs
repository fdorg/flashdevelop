using PluginCore;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeRefactor.Managers
{
    public static class BatchProcessManager
    {
        private static List<IBatchProcessor> processors = new List<IBatchProcessor>();

        /// <summary>
        /// Adds <param name="processor" /> to be selectable from the BatchProcessDialog
        /// </summary>
        public static void AddBatchProcessor(IBatchProcessor processor)
        {
            processors.Add(processor);

            EventManager.DispatchEvent(null, new DataEvent(EventType.Command, "CodeRefactor.BatchProcessorAdded", processor));
        }

        /// <summary>
        /// Removes <param name="processor" /> from the list of possible operations in BatchProcessDialog
        /// </summary>
        public static void RemoveBatchProcessor(IBatchProcessor processor)
        {
            processors.Remove(processor);

            EventManager.DispatchEvent(null, new DataEvent(EventType.Command, "CodeRefactor.BatchProcessorRemoved", processor));
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

        public static IEnumerable<string> GetAllProjectFiles(IProject project)
        {
            if (project == null)
                return null;
            
            List<String> files = new List<String>();
            String[] filters = project.DefaultSearchFilter.Split(';');

            foreach (String path in project.SourcePaths)
            {
                foreach (String filter in filters)
                {
                    files.AddRange(Directory.GetFiles(project.GetAbsolutePath(path), filter,
                        SearchOption.AllDirectories));
                }
            }
            files = files.FindAll(File.Exists);

            return files;
        }
    }

    public interface IBatchProcessor
    {
        /// <summary>
        /// The text to display in BatchProcessDialog combobox.
        /// </summary>
        string Text
        {
            get;
        }

        /// <summary>
        /// Whether this processor is currently usable,
        /// If this is false, the processor will not be shown in BatchProcessDialog
        /// </summary>
        bool IsAvailable
        {
            get;
        }

        void Process(IEnumerable<string> files);
        void ProcessProject(IProject project);
    }
}
