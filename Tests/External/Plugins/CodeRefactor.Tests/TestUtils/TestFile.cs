using System;
using System.Reflection;
using System.Text;
using System.IO;

namespace CodeRefactor.TestUtils
{
    class TestFile : IDisposable
    {
        public string ResourceFile { get; private set; }

        public string DestinationFile { get; private set; }

        public TestFile(string resourceFile)
            : this(resourceFile, GetTempFileName(resourceFile))
        {
        }

        public TestFile(string resourceFile, string destinationFile)
        {
            DestinationFile = destinationFile;
            ResourceFile = resourceFile;
            File.WriteAllBytes(destinationFile, ReadAllBytes(resourceFile));
        }

        private static string GetTempFileName(string baseFileName)
        {
            var temp = Path.GetTempFileName();
            return Path.GetFileNameWithoutExtension(temp) + Path.GetExtension(baseFileName);
        }

        public void Dispose()
        {
            if (File.Exists(DestinationFile))
                File.Delete(DestinationFile);
        }

        public static string ReadAllText(string resourceFile)
        {
            return ReadAllText(resourceFile, Encoding.UTF8);
        }

        public static string ReadAllText(string resourceFile, Encoding encoding)
        {
            var buffer = ReadAllBytes(resourceFile);
            return encoding.GetString(buffer);
        }

        public static byte[] ReadAllBytes(string resourceFile)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(resourceFile))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}