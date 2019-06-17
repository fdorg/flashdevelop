using System;
using System.Collections.Generic;

namespace PluginCore.BBCode
{
    public class BBCodeTagMatch : BBCodeTagInfo, IPairTagMatch
    {

        public BBCodeTagMatch(bool isTagOpener)
        {
            _init(isTagOpener, -1, null, null, null, 0, 0, true);
        }
        public BBCodeTagMatch(bool isTagOpener,
                               int tagIndex,
                               string tagValue,
                               string tagName,
                               string tagParam,
                               uint numOpenBraceSlashes,
                               uint numCloseBraceSlashes,
                               bool autoGenerateCloserInfo)
        {
            _init(isTagOpener, tagIndex, tagValue, tagName, tagParam, numOpenBraceSlashes, numCloseBraceSlashes, autoGenerateCloserInfo);
        }

        private void _init(bool isTagOpener,
                            int tagIndex,
                            string tagValue,
                            string tagName,
                            string tagParam,
                            uint numOpenBraceSlashes,
                            uint numCloseBraceSlashes,
                            bool autoGenerateCloserInfo)
        {
            _init_BBCodeTagInfo(isTagOpener, tagName, tagParam);

            _tagIndex = tagIndex;
            _tagValue = tagValue != null ? tagValue : "";
            _tagLength = (uint)_tagValue.Length;

            _numOpenBraceSlashes = numOpenBraceSlashes;
            _numCloseBraceSlashes = numCloseBraceSlashes;

            _tagCloserInfos = new List<BBCodeTagInfo>();

            if (autoGenerateCloserInfo && isTagOpener && !string.IsNullOrEmpty(tagName))
                _tagCloserInfos.Add(new BBCodeTagInfo(false, tagName, null));
        }


        private int _tagIndex;
        private uint _tagLength;
        private string _tagValue;

        private uint _numOpenBraceSlashes;
        private uint _numCloseBraceSlashes;

        private List<BBCodeTagInfo> _tagCloserInfos;
        private BBCodeStyle _bbCodeStyle;


        public int tagIndex
        {
            get { return _tagIndex; }
        }
        public uint tagLength
        {
            get { return _tagLength; }
        }
        public string tagValue
        {
            get { return _tagValue; }
        }

        public uint numOpenBraceSlashes
        {
            get { return _numOpenBraceSlashes; }
        }
        public uint numCloseBraceSlashes
        {
            get { return _numCloseBraceSlashes; }
        }

        public List<BBCodeTagInfo> tagCloserInfos
        {
            get { return new List<BBCodeTagInfo>(_tagCloserInfos); }
            set
            {
                if (!this.isTagOpener)
                    throw new Exception("Closer tag infos can be assigned to this only if this is opener");
                else
                    _tagCloserInfos = value != null ? new List<BBCodeTagInfo>(value) : new List<BBCodeTagInfo>();
            }
        }

        public BBCodeStyle bbCodeStyle
        {
            get { return _bbCodeStyle; }
            set
            {
                if (!this.isTagOpener)
                    throw new Exception("BBCode style can be assigned to this only if this is opener");
                else
                    _bbCodeStyle = value;
            }
        }

        public override string ToString()
        {
            return "[bbCodeTagMatch"
                   + " isTagOpener=" + this.isTagOpener
                   + " tagIndex=" + _tagIndex
                   + " tagLength=" + _tagLength
                   + " tagValue='" + _tagValue + "'"
                   + " tagName='" + this.tagName + "'"
                   + " tagParam='" + this.tagParam + "'"
                   + " numOpenBraceSlashes='" + _numOpenBraceSlashes + "'"
                   + " numCloseBraceSlashes='" + _numCloseBraceSlashes + "'"
                   + " bbCodeStyle='" + (_bbCodeStyle == null ? "null" : _bbCodeStyle.ToString()) + "'"
                   + "]";
        }

    }
}
