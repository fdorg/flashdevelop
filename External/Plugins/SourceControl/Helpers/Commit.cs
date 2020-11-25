using System;

namespace SourceControl.Helpers
{
    internal class Commit
    {
        public string Hash;
        public string Author;
        public string AuthorMail;
        public DateTime AuthorTime;
        public string AuthorTimeZone;
        public string Committer;
        public string CommitterMail;
        public DateTime CommitterTime;
        public string CommitterTimeZone;
        public string FileName;
        public string Message;
    }
}
