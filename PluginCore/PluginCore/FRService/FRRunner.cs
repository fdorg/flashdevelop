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
            this.CreateWorker();
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
                FRResults results = new FRResults();
                List<String> files = configuration.GetFiles();
                FRSearch search = configuration.GetSearch();
                foreach (String file in files)
                {
                    String src = configuration.GetSource(file);
                    search.SourceFile = file;
                    List<SearchMatch> matches = search.Matches(src);
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
                FRResults results = new FRResults();
                List<String> files = configuration.GetFiles();
                FRSearch search = configuration.GetSearch();
                string replacement = configuration.Replacement;
                if (replacement == null) return results;
                string src; 
                List<SearchMatch> matches;
                foreach (String file in files)
                {
                    src = configuration.GetSource(file);
                    search.SourceFile = file;
                    results[file] = matches = search.Matches(src);
                    foreach (SearchMatch match in matches)
                    {
                        src = search.ReplaceAll(src, replacement, matches);
                        configuration.SetSource(file, src);
                    }
                    matches = null;
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
            if (this.backgroundWorker == null) this.CreateWorker();
            configuration.Replacement = null;
            this.backgroundWorker.RunWorkerAsync(configuration);
        }

        /// <summary>
        /// Do a background text search/replace
        /// NOTE: You need to listen to the runner's events
        /// </summary>
        /// <param name="configuration">Replace operation parameters</param>
        public void ReplaceAsync(FRConfiguration configuration)
        {
            if (this.backgroundWorker == null) this.CreateWorker();
            this.backgroundWorker.RunWorkerAsync(configuration);
        }

        /// <summary>
        /// Cancel the background operation
        /// </summary>
        public void CancelAsync()
        {
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.CancelAsync();
            }
        }

        #region Background Work

        /// <summary>
        /// Initialize background thread
        /// </summary>
        private void CreateWorker()
        {
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new DoWorkEventHandler(this.BackgroundWork);
            this.backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(this.BackgroundReport);
            this.backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.BackgroundDone);
        }

        /// <summary>
        /// Event: background work finished or cancelled
        /// </summary>
        private void BackgroundDone(Object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (Finished != null)
                {
                    Finished((e.Cancelled) ? null : (FRResults)e.Result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while reporting end of background operation:\n" + ex + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Event: report background work status
        /// </summary>
        private void BackgroundReport(Object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (ProgressReport != null)
                {
                    ProgressReport(e.ProgressPercentage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception while reporting progress of background operation:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Background work main loop
        /// </summary>
        private void BackgroundWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                FRConfiguration configuration = e.Argument as FRConfiguration;
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
                    List<SearchMatch> matches;
                    string src;
                    foreach (String file in files)
                    {
                        if (this.backgroundWorker.CancellationPending) e.Cancel = true;
                        else
                        {
                            // work
                            src = configuration.GetSource(file);
                            search.SourceFile = file;
                            results[file] = matches = search.Matches(src);

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
                            matches = null;

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
