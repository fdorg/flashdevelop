using System;
using System.Collections.Generic;
using System.IO;
using PluginCore.Managers;
using SourceControl.Helpers;

namespace SourceControl.Sources.Git
{
    internal class BlameCommand : BaseCommand, IBlameCommand
    {
        private string fileName;
        private AnnotatedDocument document;
        private LinkedList<string> outputLines;
        private bool running;

        public BlameCommand(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                Dispose();
            }
            else
            {
                fileName = file;
                running = false;
                CheckExisting();
                OpenAnnotatedDocument();
                Update();
            }
        }

        private void CheckExisting()
        {
            if (AnnotatedDocument.CheckExisting(fileName))
            {
                Dispose();
            }
        }

        private void OpenAnnotatedDocument()
        {
            if (!disposed)
            {
                document = AnnotatedDocument.CreateAnnotatedDocument(this, fileName);
            }
        }

        public void Update()
        {
            if (!disposed && !running)
            {
                running = true;
                document.Initialize();
                outputLines = new LinkedList<string>();
                Run("blame --incremental \"" + fileName + "\"", Path.GetDirectoryName(fileName));
            }
        }

        protected override void Runner_Output(object sender, string line)
        {
            if (!disposed)
            {
                outputLines.AddLast(line);
            }
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            if (!disposed)
            {
                try
                {
                    if (outputLines.Count > 0 && exitCode == 0)
                    {
                        var annotations = new List<AnnotationData>();
                        while (0 < outputLines.Count)
                        {
                            annotations.Add(ParseAnnotation());
                        }
                        document.Annotate(annotations.ToArray());
                        return;
                    }
                    if (errors.Count > 0)
                    {
                        document.ShowError(string.Join("\n", errors.ToArray()));
                    }
                }
                catch (Exception e)
                {
                    ErrorManager.ShowError(e);
                }
                finally
                {
                    running = false;
                    outputLines = null;
                    runner = null;
                    errors.Clear();
                }
            }
            else
            {
                runner = null;
                errors.Clear();
            }
        }

        private AnnotationData ParseAnnotation()
        {
            var data = new AnnotationData();
            string[] firstLine = GetNextOutputLine().Split(' ');
            data.Hash = firstLine[0];
            data.SourceLine = int.Parse(firstLine[1]) - 1;
            data.ResultLine = int.Parse(firstLine[2]) - 1;
            data.LineCount = int.Parse(firstLine[3]);

            while (true)
            {
                string line = GetNextOutputLine();

                int separator = line.IndexOf(' ');
                if (separator == -1) continue;

                string key = line.Substring(0, separator);
                string value = line.Substring(separator + 1);

                switch (key)
                {
                    case "author":
                        data.Author = value;
                        break;
                    case "author-mail":
                        data.AuthorMail = value;
                        break;
                    case "author-time":
                        data.AuthorTime = ParseDateTime(value);
                        break;
                    case "author-tz":
                        data.AuthorTimeZone = value;
                        break;
                    case "committer":
                        data.Committer = value;
                        break;
                    case "committer-mail":
                        data.CommitterMail = value;
                        break;
                    case "committer-time":
                        data.CommitterTime = ParseDateTime(value);
                        break;
                    case "committer-tz":
                        data.CommitterTimeZone = value;
                        break;
                    case "previous":
                        data.IsEdit = true;
                        break;
                    case "summary":
                        data.Message = value;
                        break;
                    case "filename":
                        data.FileName = value;
                        return data;
                }
            }
        }

        private static DateTime ParseDateTime(string value)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(value));
        }

        private string GetNextOutputLine()
        {
            string value = outputLines.First.Value;
            outputLines.RemoveFirst();
            return value;
        }

        #region IBlameCommand

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                fileName = null;
                document = null;
                outputLines = null;
                running = false;

                disposed = true;
            }
        }

        public void ShowOnFileHistory(string commit)
        {
            TortoiseProc.ExecuteCustom("log", "/path:\"" + fileName + "\" /rev:" + commit);
        }

        #endregion
    }
}
