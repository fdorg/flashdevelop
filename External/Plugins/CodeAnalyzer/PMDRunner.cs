using System;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using Timer = System.Timers.Timer;

namespace CodeAnalyzer
{
    public class PMDRunner
    {
        private String errorLog;
        private String watchedFile;
        private ProcessRunner pmdRunner;
        private FileSystemWatcher pmdWatcher;
        private Timer deleteTimer;

        /// <summary>
        /// Runs the pmd analyzer process
        /// </summary>
        public static void Analyze(String pmdPath, String projectPath, String sourcePath, String pmdRuleset)
        {
            try
            {
                PMDRunner pr = new PMDRunner();
                String objDir = Path.Combine(projectPath, "obj");
                if (!Directory.Exists(objDir)) Directory.CreateDirectory(objDir);
                pr.RunPMD(pmdPath, objDir, sourcePath, pmdRuleset);
                pr.WatchFile(objDir);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Start background process
        /// </summary>
        private void RunPMD(String pmdPath, String projectPath, String sourcePath, String pmdRuleset)
        {
            String args = "-Xmx256m -jar \"" + pmdPath + "\" -s \"" + sourcePath + "\" -o \"" + projectPath + "\"";
            if (!string.IsNullOrEmpty(pmdRuleset) && File.Exists(pmdRuleset)) args += " -r \"" + pmdRuleset + "\"";
            this.SetStatusText(TextHelper.GetString("Info.RunningFlexPMD"));
            this.pmdRunner = new ProcessRunner();
            this.pmdRunner.ProcessEnded += new ProcessEndedHandler(this.PmdRunnerProcessEnded);
            this.pmdRunner.Error += new LineOutputHandler(this.PmdRunnerError);
            this.pmdRunner.Run("java", args, true);
            this.errorLog = String.Empty;
        }

        /// <summary>
        /// Trace process done message to the output panel
        /// </summary>
        private void PmdRunnerProcessEnded(Object sender, Int32 exitCode)
        {
            if (exitCode != 0)
            {
                this.pmdWatcher.EnableRaisingEvents = false;
                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
                TraceManager.Add(this.errorLog);
            }
        }

        /// <summary>
        /// Log output so that we can show it on error
        /// </summary>
        private void PmdRunnerError(Object sender, String line)
        {
            this.errorLog += line + "\n";
        }

        /// <summary>
        /// Watched the spcified file for creation
        /// </summary>
        private void WatchFile(String projectPath)
        {
            this.pmdWatcher = new FileSystemWatcher();
            this.pmdWatcher.EnableRaisingEvents = false;
            this.pmdWatcher.Filter = "pmd.xml";
            this.pmdWatcher.Created += new FileSystemEventHandler(this.onCreateFile);
            this.deleteTimer = new Timer();
            this.deleteTimer.Enabled = false;
            this.deleteTimer.AutoReset = false;
            this.deleteTimer.Interval = 500;
            this.deleteTimer.Elapsed += new ElapsedEventHandler(this.onTimedDelete);
            this.watchedFile = Path.Combine(projectPath, "pmd.xml");
            String oldFile = Path.ChangeExtension(this.watchedFile, "old");
            if (File.Exists(this.watchedFile))
            {
                if (File.Exists(oldFile)) File.Delete(oldFile);
                File.Move(this.watchedFile, oldFile);
            }
            this.pmdWatcher.Path = projectPath;
            this.pmdWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stops the timer after file creation
        /// </summary>
        private void onCreateFile(Object source, FileSystemEventArgs e)
        {
            if (e.Name.ToLower() == "pmd.xml")
            {
                this.pmdWatcher.EnableRaisingEvents = false;
                this.deleteTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Deletes the generated file after read
        /// </summary>
        private void onTimedDelete(Object sender, ElapsedEventArgs e)
        {
            Form mainForm = PluginBase.MainForm as Form;
            if (mainForm.InvokeRequired)
            {
                mainForm.BeginInvoke((MethodInvoker)delegate { this.onTimedDelete(sender, e); });
                return;
            }
            try
            {
                if (File.Exists(this.watchedFile))
                {
                    String currFile = String.Empty;
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
                    XmlTextReader reader = new XmlTextReader(this.watchedFile);
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "file")
                        {
                            currFile = reader.GetAttribute("name");
                        }
                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "violation")
                        {
                            Int32 state = 0;
                            Int32 line = Convert.ToInt32(reader.GetAttribute("beginline"));
                            Int32 col = Convert.ToInt32(reader.GetAttribute("begincolumn"));
                            line = line > 0 ? line : 0; col = col > 0 ? col : 0;
                            switch (reader.GetAttribute("priority"))
                            {
                                case "1": state = 3; break;
                                case "3": state = 2; break;
                                case "5": state = 0; break;
                            }
                            String item = String.Format("{0}:{1}: col: {2}: {3}", currFile, line, col, reader.ReadElementContentAsString().Trim());
                            TraceManager.Add(item, state);
                        }
                    }
                    reader.Close();
                    String oldFile = Path.ChangeExtension(this.watchedFile, "old");
                    if (File.Exists(this.watchedFile))
                    {
                        if (File.Exists(oldFile)) File.Delete(oldFile);
                        File.Move(this.watchedFile, oldFile);
                    }
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                    this.SetStatusText(TextHelper.GetString("Info.FlexPMDFinished"));
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Sets the status text
        /// </summary>
        public void SetStatusText(String text)
        {
            String status = "  " + text;
            PluginBase.MainForm.StatusStrip.Items[0].Text = status;
        }

    }

}