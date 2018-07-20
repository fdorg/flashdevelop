using System;

namespace PluginCore.BBCode
{
    public class PairTag : IPairTag
    {
        public PairTag(IPairTagMatch openerMatch, IPairTagMatch closerMatch)
        {
            _openerMatch = openerMatch;
            _closerMatch = closerMatch;
        }


        private IPairTagMatch _openerMatch;
        private IPairTagMatch _closerMatch;


        public IPairTagMatch openerMatch
        {
            get { return _openerMatch; }
        }
        public IPairTagMatch closerMatch
        {
            get { return _closerMatch; }
        }


        override public String ToString()
        {
            return "[pairTag"
                   + " openerMatch='" + (_openerMatch == null ? "null" : _openerMatch.ToString()) + "'"
                   + " closerMatch='" + (_closerMatch == null ? "null" : _closerMatch.ToString()) + "'"
                   + "]";
        }
    }


    public class VoidCloserTagMatch : IPairTagMatch
    {
        public VoidCloserTagMatch()
        {
            _init(-1);
        }
        public VoidCloserTagMatch(int tagIndex)
        {
            _init(tagIndex);
        }
        private void _init(int tagIndex)
        {
            _tagIndex = tagIndex;
        }


        private int _tagIndex;


        public Boolean isTagOpener
        {
            get { return false; }
        }
        public int tagIndex
        {
            get { return _tagIndex; }
        }
        public uint tagLength
        {
            get { return 0; }
        }
        public String tagValue
        {
            get { return ""; }
        }
        public String tagName
        {
            get { return null; }
        }
    }
}
