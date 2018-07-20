using System;
using System.Text.RegularExpressions;

namespace PluginCore.BBCode
{
    public class BBCodeTagMatcher : IPairTagMatcher
    {

        public BBCodeTagMatcher()
        {
            _reOpener = _buildReOpenerFor(@"[\*\~]?\w[\w\d\-_]*");
        }


        protected String _input;

        protected String _retplOpenerA = @"(?<slashA>\\*)(?<tagVal>\[(?<tagName>";
        protected String _retplOpenerB = @")(\s*\=\s*(?<tagParam>[^\]\\]*))?(?<slashB>\\*)\])";

        protected String _retplCloserA = @"(?<slashA>\\*)(?<tagVal>\[\/(?<tagName>";
        protected String _retplCloserB = @")\])";

        protected Regex _reOpener;


        public String input
        {
            get { return _input; }
            set { _input = value; }
        }


        public IPairTagMatch searchOpener()
        {
            return searchOpener(0);
        }
        public IPairTagMatch searchOpener(uint startAt)
        {
            return _searchMatch(_reOpener, true, startAt);
        }

        public IPairTagMatch searchOpenerAs(IPairTagMatch opener)
        {
            return searchOpenerAs(opener, 0);
        }
        public IPairTagMatch searchOpenerAs(IPairTagMatch opener, uint startAt)
        {
            BBCodeTagMatch tm = opener as BBCodeTagMatch;
            if (tm == null || string.IsNullOrEmpty(tm.tagName))
                return null;

            return searchOpenerByName(tm.tagName, startAt);
        }

        public IPairTagMatch searchOpenerByName(String tagName)
        {
            return searchOpenerByName(tagName, 0);
        }
        public IPairTagMatch searchOpenerByName(String tagName, uint startAt)
        {
            if (string.IsNullOrEmpty(tagName))
                return null;

            return _searchMatch(_buildReOpenerFor(tagName), true, startAt);
        }

        public IPairTagMatch searchCloserFor(IPairTagMatch opener)
        {
            return searchCloserFor(opener, 0);
        }
        public IPairTagMatch searchCloserFor(IPairTagMatch opener, uint startAt)
        {
            BBCodeTagMatch tm = opener as BBCodeTagMatch;
            if (tm == null || string.IsNullOrEmpty(tm.tagName))
                return null;

            IPairTagMatch successMatch = null;
            IPairTagMatch m;
            int j = 0xFFFFFF;
            int i = tm.tagCloserInfos.Count;
            while (i-- > 0)
            {
                m = searchCloserByName(tm.tagCloserInfos[i].tagName, tm.tagCloserInfos[i].isTagOpener, startAt);
                if (m != null && m.tagIndex < j)
                {
                    j = m.tagIndex;
                    successMatch = m;
                }
            }
            return successMatch;
        }

        public IPairTagMatch searchCloserByName(String tagName)
        {
            return searchCloserByName(tagName, false);
        }
        public IPairTagMatch searchCloserByName(String tagName, Boolean isOpener)
        {
            return searchCloserByName(tagName, isOpener, 0);
        }
        public IPairTagMatch searchCloserByName(String tagName, Boolean isOpener, uint startAt)
        {
            if (string.IsNullOrEmpty(tagName))
                return null;

            return _searchMatch(_buildReCloserFor(tagName), isOpener, startAt);
        }


        protected Regex _buildReOpenerFor()
        {
            return _buildReOpenerFor("");
        }
        protected Regex _buildReOpenerFor(String tagName)
        {
            return _buildReOpenerFor(tagName, RegexOptions.IgnoreCase);
        }
        protected Regex _buildReOpenerFor(String tagName, RegexOptions reFlags)
        {
            return new Regex(_retplOpenerA + tagName + _retplOpenerB, reFlags);
        }

        protected Regex _buildReCloserFor()
        {
            return _buildReCloserFor("");
        }
        protected Regex _buildReCloserFor(String tagName)
        {
            return _buildReCloserFor(tagName, RegexOptions.IgnoreCase);
        }
        protected Regex _buildReCloserFor(String tagName, RegexOptions reFlags)
        {
            return new Regex(_retplCloserA + tagName + _retplCloserB, reFlags);
        }

        protected BBCodeTagMatch _searchMatch(Regex regex, Boolean isOpener)
        {
            return _searchMatch(regex, isOpener, 0);
        }
        protected BBCodeTagMatch _searchMatch(Regex regex, Boolean isOpener, uint startAt)
        {
            if (regex == null)
                return null;

            int idx = (int)startAt - 16;
            if (idx < 0)
                idx = 0;

            Match m;
            BBCodeTagMatch tm;

            while (true)
            {
                m = regex.Match(_input, idx);
                if (m == null || !m.Success || m.Index < 0)
                    return null;

                idx = m.Index + m.Length;

                tm = _toTagMatch(isOpener, m);
                if (tm.tagIndex < startAt
                    || tm.numOpenBraceSlashes % 2 != 0
                    || tm.numCloseBraceSlashes % 2 != 0)
                    continue;
                break;
            }
            return tm;
        }

        protected BBCodeTagMatch _toTagMatch(Boolean isOpener, Match m)
        {
            if (m == null)
                return null;

            Group mi = m.Groups["slashA"];
            int numSlashA = (mi != null && mi.Success) ? mi.Length : 0;

            mi = m.Groups["slashB"];
            int numSlashB = (mi != null && mi.Success) ? mi.Length : 0;

            mi = m.Groups["tagVal"];
            String tagVal = (mi != null && mi.Success) ? mi.Value : "";
            int tagIndex = (mi != null && mi.Success) ? mi.Index : -1;

            mi = m.Groups["tagName"];
            String tagName = (mi != null && mi.Success) ? mi.Value : "";

            mi = m.Groups["tagParam"];
            String tagParam = (mi != null && mi.Success) ? mi.Value : "";

            BBCodeTagMatch tm = new BBCodeTagMatch(isOpener,
                                                   tagIndex,
                                                   tagVal,
                                                   tagName,
                                                   tagParam,
                                                   (uint)numSlashA,
                                                   (uint)numSlashB,
                                                   true);

            if (isOpener)
                tm.bbCodeStyle = new BBCodeStyle();

            return tm;
        }

    }
}
