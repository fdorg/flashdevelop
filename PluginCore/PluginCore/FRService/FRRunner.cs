// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PluginCore.FRService
{
    /// <summary>
    /// Event delegates of the service
    /// </summary> 
    public delegate void FRProgressReportHandler(int percentDone);
    public delegate void FRFinishedHandler(FRResults results);

    /// <summary>
    /// "Alias" for: Dictionary<String, List<SearchMatch>>
    /// </summary>
    public class FRResults : Dictionary<string, List<SearchMatch>>
    {
    }

    /// <summary>
    /// Find/replace service for mass text operations
    /// The service can work synchronous or asynchronous
    /// </summary>
    public class FRRunner
    {
        /// <summary>
        /// Properties of the class
        /// </summary>
        BackgroundWorker backgroundWorker;

        /// <summary>
        /// Events of the class
        /// </summary>
        public event FRProgressReportHandler ProgressReport;
        public event FRFinishedHandler Finished;

        /// <summary>
        /// Creates a search/replace service instance
        /// </summary>
        public FRRunner() => CreateWorker();

        /// <summary>
        /// Do a synchronous search
        /// Use SearchAsync to run a background thread
        /// </summary>
        /// <param name="configuration">Search operation parameters</param>
        /// <returns>Search results</returns>
        public FRResults SearchSync(FRConfiguration configuration)
        {
            try
            {
                var results = new FRResults();
                var files = configuration.GetFiles();
                var search = configuration.GetSearch();
                foreach (var file in files)
                {
                    var src = configuration.GetSource(file);
                    search.SourceFile = file;
                    var matches = search.Matches(src);
                    FRSearch.ExtractResultsLineText(matches, src);
                    results[file] = matches;
                }
                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Do a synchronous text search/replace
        /// Use RepleaceAsync to run a background thread
        /// </summary>
        /// <param name="configuration">Replace operation parameters</param>
        /// <returns>Search results before replacement</returns>
        public FRResults ReplaceSync(FRConfiguration configuration)
        {
            try
            {
                var results = new FRResults();
                var files = configuration.GetFiles();
                var search = configuration.GetSearch();
                var replacement = configuration.Replacement;
                if (replacement is null) return results;
                foreach (var file in files)
                {
                    var src = configuration.GetSource(file);
                    search.SourceFile = file;
                    var matches = search.Matches(src);
                    results[file] = matches;
                    foreach (var match in matches)
                    {
                        src = search.ReplaceAll(src, replacement, matches);
                        configuration.SetSource(file, src);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Do a background text search
        /// NOTE: You need to listen to the runner's events
        /// </summary>
        /// <param name="configuration">Search operation parameters</param>
        public void SearchAsync(FRConfiguration configuration)
        {
            if (backgroundWorker is null) CreateWorker();
            configuration.Replacement = null;
            backgroundWorker.RunWorkerAsync(configuration);
        }

        /// <summary>
        /// Do a background text search/replace
        /// NOTE: You need to listen to the runner's events
        /// </summary>
        /// <param name="configuration">Replace operation parameters</param>
        public void ReplaceAsync(FRConfiguration configuration)
        {
            if (backgroundWorker is null) CreateWorker();
            backgroundWorker.RunWorkerAsync(configuration);
        }

        /// <summary>
        /// Cancel the background operation
        /// </summary>
        public void CancelAsync() => backgroundWorker?.CancelAsync();

        #region Background Work

        /// <summary>
        /// Initialize background thread
        /// </summary>
        void CreateWorker()
        {
            backgroundWorker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            backgroundWorker.DoWork += BackgroundWork;
            backgroundWorker.ProgressChanged += BackgroundReport;
            backgroundWorker.RunWorkerCompleted += BackgroundDone;
        }

        /// <summary>
        /// Event: background work finished or cancelled
        /// </summary>
        void BackgroundDone(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Finished?.Invoke(e.Cancelled ? null : (FRResults)e.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception while reporting end of background operation:\n{ex}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Event: report background work status
        /// </summary>
        void BackgroundReport(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ProgressReport?.Invoke(e.ProgressPercentage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception while reporting progress of background operation:\n{ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Background work main loop
        /// </summary>
        void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (!(e.Argument is FRConfiguration configuration))
                {
                    e.Result = null;
                    return;
                }
                var files = configuration.GetFiles();
                if (files.IsNullOrEmpty())
                {
                    e.Result = new FRResults(); // empty results
                    return;
                }

                var results = new FRResults();

                if (backgroundWorker.CancellationPending) e.Cancel = true;
                else
                {
                    var count = 0;
                    var search = configuration.GetSearch();
                    var replacement = configuration.Replacement;
                    // do search
                    var total = files.Count;
                    var lastPercent = 0;
                    foreach (var file in files)
                    {
                        if (backgroundWorker.CancellationPending) e.Cancel = true;
                        else
                        {
                            // work
                            var src = configuration.GetSource(file);
                            search.SourceFile = file;
                            var matches = search.Matches(src);
                            if (matches.Count > 0)
                            {
                                results[file] = matches;
                                if (replacement != null)
                                {
                                    // replace text
                                    src = search.ReplaceAll(src, replacement, matches);
                                    configuration.SetSource(file, src);
                                }
                                else FRSearch.ExtractResultsLineText(matches, src);
                            }

                            // progress
                            count++;
                            int percent = (100 * count) / total;
                            if (lastPercent != percent)
                            {
                                backgroundWorker.ReportProgress(percent);
                                lastPercent = percent;
                            }
                        }
                    }
                }
                e.Result = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception during background operation:\n{ex.Message}\n{ex.StackTrace}");
                e.Result = null;
            }
        }

        #endregion

    }
}