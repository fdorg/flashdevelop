using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ProjectManager.Projects
{
    /// <summary>
    /// Represents a (usually large) set of options for a compiler.
    /// </summary>
    [Serializable]
    public abstract class CompilerOptions : ICloneable
    {
        static BinaryFormatter formatter = new BinaryFormatter();

        object ICloneable.Clone() { return Clone(); }

        public CompilerOptions Clone()
        {
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return (CompilerOptions)formatter.Deserialize(stream);
        }
    }
}
