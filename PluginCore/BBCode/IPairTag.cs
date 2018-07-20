using System;

namespace PluginCore.BBCode
{
    public interface IPairTag
    {
        IPairTagMatch openerMatch { get; }
        IPairTagMatch closerMatch { get; }
    }

    public interface IPairTagMatch
    {
        Boolean isTagOpener { get; }
        int tagIndex { get; }
        uint tagLength { get; }
        String tagValue { get; }
    }

    public interface IPairTagMatcher
    {
        String input { get; set; }

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
        Boolean handleTag(IPairTagMatch tag);
        Boolean isHandleable(IPairTagMatch tag);
    }
}
