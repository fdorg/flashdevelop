// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace PluginCore.BBCode
{
    public interface IPairTag
    {
        IPairTagMatch openerMatch { get; }
        IPairTagMatch closerMatch { get; }
    }

    public interface IPairTagMatch
    {
        bool isTagOpener { get; }
        int tagIndex { get; }
        uint tagLength { get; }
        string tagValue { get; }
    }

    public interface IPairTagMatcher
    {
        string input { get; set; }

        IPairTagMatch searchOpener(uint startAt);
        IPairTagMatch searchOpenerAs(IPairTagMatch opener, uint startAt);
        IPairTagMatch searchCloserFor(IPairTagMatch opener, uint startAt);
    }

    public interface IPairTagMatchHandler
    {
        /**
         * Tag handler. Extracting raw params to final data.
         * @param   tag
         * @return  true if tag has been handled.
         */
        bool handleTag(IPairTagMatch tag);
        bool isHandleable(IPairTagMatch tag);
    }
}
