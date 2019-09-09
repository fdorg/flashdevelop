namespace PluginCore.Localization
{
    public enum LocaleVersion
    {
        /// <summary>
        /// English - UNITED STATES (US)
        /// </summary>
        [StringValue("en_US")]
        en_US = 0,

        /// <summary>
        /// Japanese - JAPAN (JP)
        /// </summary>
        [StringValue("ja_JP")]
        ja_JP = 1,

        /// <summary>
        /// German - GERMANY (DE)
        /// </summary>
        [StringValue("de_DE")]
        de_DE = 2,

        /// <summary>
        /// Basque - SPAIN (ES)
        /// </summary>
        [StringValue("eu_ES")]
        eu_ES = 3,

        /// <summary>
        /// Chinese - CHINA (CN)
        /// </summary>
        [StringValue("zh_CN")]
        zh_CN = 4,

        /// <summary>
        /// Korean - KOREA, REPUBLIC OF (KR)
        /// </summary>
        [StringValue("ko_KR")]
        ko_KR = 5,

        /// <summary>
        /// Represents an invalid locale.
        /// </summary>
        [System.ComponentModel.Browsable(false)]
        Invalid = -1
    }

}
