// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
// IMatchFinder.cs

using System.IO;

namespace SevenZip.Compression.LZ
{
    interface IInWindowStream
    {
        void SetStream(Stream inStream);
        void Init();
        void ReleaseStream();
        byte GetIndexByte(int index);
        uint GetMatchLen(int index, uint distance, uint limit);
        uint GetNumAvailableBytes();
    }

    interface IMatchFinder : IInWindowStream
    {
        void Create(uint historySize, uint keepAddBufferBefore,
                uint matchMaxLen, uint keepAddBufferAfter);
        uint GetMatches(uint[] distances);
        void Skip(uint num);
    }
}
