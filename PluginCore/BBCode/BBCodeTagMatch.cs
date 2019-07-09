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

            this.tagIndex = tagIndex;
            this.tagValue = tagValue ?? "";
            tagLength = (uint)this.tagValue.Length;

            this.numOpenBraceSlashes = numOpenBraceSlashes;
            this.numCloseBraceSlashes = numCloseBraceSlashes;

            _tagCloserInfos = new List<BBCodeTagInfo>();

            if (autoGenerateCloserInfo && isTagOpener && !string.IsNullOrEmpty(tagName))
                _tagCloserInfos.Add(new BBCodeTagInfo(false, tagName, null));
        }

        private List<BBCodeTagInfo> _tagCloserInfos;
        private BBCodeStyle _bbCodeStyle;

        public int tagIndex { get; private set; }

        public uint tagLength { get; set; }

        public string tagValue { get; set; }

        public uint numOpenBraceSlashes { get; set; }

        public uint numCloseBraceSlashes { get; private set; }

        public List<BBCodeTagInfo> tagCloserInfos
        {
            get => new List<BBCodeTagInfo>(_tagCloserInfos);
            set
            {
                if (!isTagOpener)
                    throw new Exception("Closer tag infos can be assigned to this only if this is opener");
                _tagCloserInfos = value != null ? new List<BBCodeTagInfo>(value) : new List<BBCodeTagInfo>();
            }
        }

        public BBCodeStyle bbCodeStyle
        {
            get => _bbCodeStyle;
            set
            {
                if (!isTagOpener)
                    throw new Exception("BBCode style can be assigned to this only if this is opener");
                _bbCodeStyle = value;
            }
        }

        public override string ToString()
        {
            return "[bbCodeTagMatch"
                   + " isTagOpener=" + isTagOpener
                   + " tagIndex=" + tagIndex
                   + " tagLength=" + tagLength
                   + " tagValue='" + tagValue + "'"
                   + " tagName='" + tagName + "'"
                   + " tagParam='" + tagParam + "'"
                   + " numOpenBraceSlashes='" + numOpenBraceSlashes + "'"
                   + " numCloseBraceSlashes='" + numCloseBraceSlashes + "'"
                   + " bbCodeStyle='" + (_bbCodeStyle is null ? "null" : _bbCodeStyle.ToString()) + "'"
                   + "]";
        }

    }
}
