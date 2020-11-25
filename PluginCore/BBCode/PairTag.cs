namespace PluginCore.BBCode
{
    public class PairTag : IPairTag
    {
        public PairTag(IPairTagMatch openerMatch, IPairTagMatch closerMatch)
        {
            _openerMatch = openerMatch;
            _closerMatch = closerMatch;
        }


        private readonly IPairTagMatch _openerMatch;
        private readonly IPairTagMatch _closerMatch;


        public IPairTagMatch openerMatch => _openerMatch;

        public IPairTagMatch closerMatch => _closerMatch;


        public override string ToString()
        {
            return "[pairTag"
                   + " openerMatch='" + (_openerMatch is null ? "null" : _openerMatch.ToString()) + "'"
                   + " closerMatch='" + (_closerMatch is null ? "null" : _closerMatch.ToString()) + "'"
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


        public bool isTagOpener => false;

        public int tagIndex => _tagIndex;

        public uint tagLength => 0;

        public string tagValue => "";

        public string tagName => null;
    }
}
