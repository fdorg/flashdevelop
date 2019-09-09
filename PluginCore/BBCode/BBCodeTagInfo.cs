// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            this.tagName = tagName ?? "";
            this.tagParam = tagParam ?? "";
            this.isTagOpener = isTagOpener;
        }

        public bool isTagOpener { get; private set; }

        public string tagName { get; private set; }

        public string tagParam { get; private set; }

        public override string ToString()
        {
            return "[bbCodeTagInfo"
                   + " isTagOpener=" + isTagOpener
                   + " tagName='" + tagName + "'"
                   + " tagParam='" + tagParam + "'"
                   + "]";
        }
    }
}