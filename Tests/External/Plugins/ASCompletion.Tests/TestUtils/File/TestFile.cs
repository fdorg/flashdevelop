using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ASCompletion.TestUtils.File
{
    class TestFile : IDisposable
    {
        public string ResourceFile { get; private set; }

        public string DestinationFile { get; private set; }

        public TestFile(QualifiedFilePathInfo pathInfo, string resourceFile, string destinationFile = null)
        {
            ResourceFile = pathInfo.GetPath(resourceFile);
            DestinationFile = destinationFile ?? GetTempFileName(ResourceFile);

            System.IO.File.WriteAllBytes(DestinationFile, ReadAllBytes(pathInfo, resourceFile));
        }

        private static string GetTempFileName(string baseFileName)
        {
            string temp = Path.GetTempFileName();
            return Path.GetFileNameWithoutExtension(temp) + Path.GetExtension(baseFileName);
        }

        public void Dispose()
        {
            if (System.IO.File.Exists(DestinationFile))
                System.IO.File.Delete(DestinationFile);
        }

        public static string ReadAllText(QualifiedFilePathInfo pathInfo, string resourceFile)
        {
            return ReadAllText(pathInfo, resourceFile, Encoding.UTF8);
        }

        public static string ReadAllText(QualifiedFilePathInfo pathInfo, string resourceFile, Encoding encoding)
        {
            return encoding.GetString(ReadAllBytes(pathInfo, resourceFile));
        }

        private static byte[] ReadAllBytes(QualifiedFilePathInfo pathInfo, string resourceFile)
        {
            var asm = Assembly.GetExecutingAssembly();
            string path = pathInfo.GetPath(resourceFile);

            using (var stream = asm.GetManifestResourceStream(path))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
