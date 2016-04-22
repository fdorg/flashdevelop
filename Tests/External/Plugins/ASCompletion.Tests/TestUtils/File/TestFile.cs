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

        public TestFile(string resourceFile, string destinationFile = null)
        {
            ResourceFile = resourceFile;
            DestinationFile = destinationFile ?? GetTempFileName(ResourceFile);

            System.IO.File.WriteAllBytes(DestinationFile, ReadAllBytes(ResourceFile));
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

        public static string ReadAllText(string resourceFile)
        {
            return ReadAllText(resourceFile, Encoding.UTF8);
        }

        public static string ReadAllText(string resourceFile, Encoding encoding)
        {
            return encoding.GetString(ReadAllBytes(resourceFile));
        }

        private static byte[] ReadAllBytes(string resourceFile)
        {
            var asm = Assembly.GetExecutingAssembly();

            using (var stream = asm.GetManifestResourceStream(resourceFile))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
