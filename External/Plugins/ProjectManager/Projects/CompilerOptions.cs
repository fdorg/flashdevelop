// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        static readonly BinaryFormatter formatter = new BinaryFormatter();

        object ICloneable.Clone() => Clone();

        public CompilerOptions Clone()
        {
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return (CompilerOptions)formatter.Deserialize(stream);
        }
    }
}
