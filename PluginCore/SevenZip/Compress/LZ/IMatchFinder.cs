// IMatchFinder.cs

using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
    interface IInWindowStream
    {
        void SetStream(Stream inStream);
        void Init();
        void ReleaseStream();
        Byte GetIndexByte(Int32 index);
        UInt32 GetMatchLen(Int32 index, UInt32 distance, UInt32 limit);
        UInt32 GetNumAvailableBytes();
    }

    interface IMatchFinder : IInWindowStream
    {
        void Create(UInt32 historySize, UInt32 keepAddBufferBefore,
                UInt32 matchMaxLen, UInt32 keepAddBufferAfter);
        UInt32 GetMatches(UInt32[] distances);
        void Skip(UInt32 num);
    }
}
