using System;

namespace PluginCore.BBCode
{
    public class BBCodeTagInfo
    {

        public BBCodeTagInfo()
        {
            _init_BBCodeTagInfo(false, "", null);
        }
        public BBCodeTagInfo(Boolean isTagOpener, String tagName, String tagParam)
        {
            _init_BBCodeTagInfo(isTagOpener, tagName, tagParam);
        }

        protected void _init_BBCodeTagInfo(Boolean isTagOpener, String tagName, String tagParam)
        {
            _tagName = tagName != null ? tagName : "";
            _tagParam = tagParam != null ? tagParam : "";
            _isTagOpener = isTagOpener;
        }


        private Boolean _isTagOpener;
        private String _tagName;
        private String _tagParam;


        public Boolean isTagOpener
        {
            get { return _isTagOpener; }
        }
        public String tagName
        {
            get { return _tagName; }
        }
        public String tagParam
        {
            get { return _tagParam; }
        }


        override public String ToString()
        {
            return "[bbCodeTagInfo"
                   + " isTagOpener=" + _isTagOpener.ToString()
                   + " tagName='" + _tagName + "'"
                   + " tagParam='" + _tagParam + "'"
                   + "]";
        }

    }
}
