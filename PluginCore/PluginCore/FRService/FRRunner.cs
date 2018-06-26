using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PluginCore.FRService
{
    /// <summary>
    /// Event delegates of the service
    /// </summary> 
    public delegate void FRProgressReportHandler(Int32 percentDone);
    public delegate void FRFinishedHandler(FRResults results);

    /// <summary>
    /// "Alias" for: Dictionary<String, List<SearchMatch>>
    /// </summary>
    public class FRResults : Dictionary<String, List<SearchMatch>>
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
        private BackgroundWorker backgroundWorker;

        /// <summary>
        /// Events of the class
        /// </summary>
        public event FRProgressReportHandler ProgressReport;
        public event FRFinishedHandler Finished;

        /// <summary>
        /// Creates a search/replace service instance
        /// </summary>
        public FRRunner()
        {
            CreateWorker();
        }

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
                MessageBox.Show("Exception " + ex.Message + "\n" + ex.StackTrace);
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
                if (replacement == null) return results;
                foreach (var file in files)
                {
                    var src = configuration.GetSource(file);
                    search.SourceFile = file;
                    var matches = search.Matches(src);;
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
                MessageBox.Show("Exception " + ex.Message + "\n" + ex.StackTrace);
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
            if (backgroundWorker == null) CreateWorker();
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
            if (backgroundWorker == null) CreateWorker();
            backgroundWorker.RunWorkerAsync(configuration);
        }

        /// <summary>
        /// Cancel the background operation
        /// </summary>
        public void CancelAsync()
        {
            backgroundWorker?.CancelAsync();
        }

        #region Background Work

        /// <summary>
        /// Initialize background thread
        /// </summary>
        private void CreateWorker()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWork;
            backgroundWorker.ProgressChanged += BackgroundReport;
            backgroundWorker.RunWorkerCompleted += BackgroundDone;
        }

        /// <summary>
        /// Event: background work finished or cancelled
        /// </summary>
        private void BackgroundDone(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Finished?.Invoke(e.Cancelled ? null : (FRResults)e.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while reporting end of background operation:\n" + ex + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Event: report background work status
        /// </summary>
        private void BackgroundReport(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ProgressReport?.Invoke(e.ProgressPercentage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while reporting progress of background operation:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Background work main loop
        /// </summary>
        private void BackgroundWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var configuration = e.Argument as FRConfiguration;
                if (configuration == null)
                {
                    e.Result = null;
                    return;
                }
                // get files
                Int32 count = 0;
                List<string> files = configuration.GetFiles();
                if (files == null || files.Count == 0)
                {
                    e.Result = new FRResults(); // empty results
                    return;
                }

                FRResults results = new FRResults();
                FRSearch search = configuration.GetSearch();
                string replacement = configuration.Replacement;

                if (this.backgroundWorker.CancellationPending) e.Cancel = true;
                else
                {
                    // do search
                    Int32 total = files.Count;
                    Int32 lastPercent = 0;
                    foreach (String file in files)
                    {
                        if (this.backgroundWorker.CancellationPending) e.Cancel = true;
                        else
                        {
                            // work
                            var src = configuration.GetSource(file);
                            search.SourceFile = file;
                            var matches = search.Matches(src);
                            results[file] = matches;

                            if (matches.Count > 0)
                            {
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
                            Int32 percent = (100 * count) / total;
                            if (lastPercent != percent)
                            {
                                this.backgroundWorker.ReportProgress(percent);
                                lastPercent = percent;
                            }
                        }
                    }
                }
                e.Result = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception during background operation:\n" + ex.Message + "\n" + ex.StackTrace);
                e.Result = null;
            }
        }

        #endregion

    }

}
