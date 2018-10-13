namespace Tatoeba.Mobile.Services
{
    public class XpathLoginConfig
    {
        public readonly static XpathPathConfig KeyPath = new XpathPathConfig { Path = "string(//*[@name=\"data[_Token][key]\"]/@value)" };
        public readonly static XpathPathConfig ValuePath = new XpathPathConfig { Path = "string(//*[@name=\"data[_Token][fields]\"]/@value)" };
        public readonly static XpathPathConfig SuccessPath = new XpathPathConfig { Path = "boolean(//li[@id='profile'][1])" };
    }

    public class XpathTranslationConfig
    {
        public readonly static XpathPathConfig IsEditablePath = new XpathPathConfig { Path = "boolean((//*[@class=\"sentence mainSentence\"]//*[@class=\"text correctnessZero editableSentence\"])[1])" };
        public readonly static string ItemsPath = "//*[@class=\"sentence mainSentence\"]|//*[@class=\"translations\"]/*[@data-sentence-id]|//div[@class=\"more\"]/*[@data-sentence-id]";        

        public readonly static XpathPathConfig TextPath = new XpathPathConfig
        {
            Path = "string(.//*[@class=\"text correctnessZero\"]|.//*[@class=\"text correctnessZero editableSentence\"])",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public readonly static XpathPathConfig DirectionPath = new XpathPathConfig { Path = "string(.//*[@class=\"text correctnessZero\"]/@dir|//*[@class=\"text correctnessZero editableSentence\"]/@dir)" };
        public readonly static XpathPathConfig IdPath = new XpathPathConfig { Path = "string(@data-sentence-id)" };
        public readonly static XpathPathConfig LanguagePath = new XpathPathConfig { Path = "string(.//img/@alt)" };
        public readonly static XpathPathConfig TranslationTypePath = new XpathPathConfig { Path = "string(div/a/@class)" };
    }

    public class XpathCommentConfig
    {
        public readonly static string ItemsPath = "//md-card";

        public readonly static XpathPathConfig UsernamePath = new XpathPathConfig
        {
            Path = "string(.//*[@class=\"md-title\"])",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
        
        public readonly static XpathPathConfig ContentPath = new XpathPathConfig
        {
            Path = "string(.//md-card-content)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public readonly static XpathPathConfig DateTextPath = new XpathPathConfig
        {
            Path = "string(.//*[@class=\"md-subhead ellipsis\"])",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
    }

    public class XpathLogConfig
    {
        public readonly static string ItemsPath = "//md-list-item";
        public readonly static XpathPathConfig TextPath = new XpathPathConfig
        {
            Path = "string(div/div)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
        public readonly static XpathPathConfig DateTextPath = new XpathPathConfig
        {
            Path = "string(div/p)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
        public readonly static XpathPathConfig ContribTypePath = new XpathPathConfig { Path = "string(@class)" };
    }

    public class XpathContribConfig
    {
        public readonly static string ItemsPath = "//md-list-item";

        public readonly static XpathPathConfig TextPath = new XpathPathConfig
        {
            Path = "string(div/div)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public readonly static XpathPathConfig DirectionPath = new XpathPathConfig { Path = "string(div/div/@dir)" };

        public readonly static XpathPathConfig DateTextPath = new XpathPathConfig
        {
            Path = "string(div/p)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public readonly static XpathPathConfig IdPath = new XpathPathConfig { Path = "string(md-button/@href)" };
        public readonly static XpathPathConfig LanguagePath = new XpathPathConfig { Path = "string(img/@alt)" };
        public readonly static XpathPathConfig ContribTypePath = new XpathPathConfig { Path = "string(@class)" };
    }

    public enum XpathActionConfig
    {
        None,
        Trim,
        HtmlDecode,
        HtmlDecodeAndTrim,
    }

    public class XpathPathConfig
    {
        public string Path { get; set; }
        public XpathActionConfig Action { get; set; }
    }
}
