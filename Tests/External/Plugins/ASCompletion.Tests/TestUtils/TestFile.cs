using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

namespace ASCompletion.TestUtils
{
    class TestFile : IDisposable
    {
        public string ResourceFile { get; private set; }

        public string DestinationFile { get; private set; }

        public TestFile(string resourceFile)
            : this(resourceFile, Path.GetTempFileName())
        {
        }

        public TestFile(string resourceFile, string destinationFile)
        {
            var asm = Assembly.GetExecutingAssembly();
            DestinationFile = destinationFile;
            ResourceFile = resourceFile;

            using (var stream = asm.GetManifestResourceStream(resourceFile))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                File.WriteAllBytes(destinationFile, buffer);
            }
        }

        public void Dispose()
        {
            if (File.Exists(DestinationFile))
                File.Delete(DestinationFile);
        }
    }
}
