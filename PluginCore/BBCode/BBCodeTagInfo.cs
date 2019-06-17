namespace PluginCore.BBCode
{
    public class BBCodeTagInfo
    {

        public BBCodeTagInfo()
        {
            _init_BBCodeTagInfo(false, "", null);
        }
        public BBCodeTagInfo(bool isTagOpener, string tagName, string tagParam)
        {
            _init_BBCodeTagInfo(isTagOpener, tagName, tagParam);
        }

        protected void _init_BBCodeTagInfo(bool isTagOpener, string tagName, string tagParam)
        {
            _tagName = tagName != null ? tagName : "";
            _tagParam = tagParam != null ? tagParam : "";
            _isTagOpener = isTagOpener;
        }


        private bool _isTagOpener;
        private string _tagName;
        private string _tagParam;


        public bool isTagOpener
        {
            get { return _isTagOpener; }
        }
        public string tagName
        {
            get { return _tagName; }
        }
        public string tagParam
        {
            get { return _tagParam; }
        }


        override public string ToString()
        {
            return "[bbCodeTagInfo"
                   + " isTagOpener=" + _isTagOpener
                   + " tagName='" + _tagName + "'"
                   + " tagParam='" + _tagParam + "'"
                   + "]";
        }

    }
}
