namespace Tatoeba.Mobile.Services
{
    public class TatoebaConfig
    {
        public UrlConfig UrlConfig { get; set; } = new UrlConfig();
        public XpathConfig XpathConfig { get; set; } = new XpathConfig();        
    }

    public class UrlConfig
    {
        public string Main { get; set; } = "https://tatoeba.org/eng/";
        public string Login { get; set; } = "https://tatoeba.org/eng/users/check_login?redirectTo=%2Feng";
        public string Flags { get; set; } = "https://raw.githubusercontent.com/Tatoeba/tatoeba2/dev/app/webroot/img/flags/";
        public string Languages { get; set; } = "https://raw.githubusercontent.com/Tatoeba/tatoeba2/dev/app/Lib/LanguagesLib.php";
        public string LatestContribs { get; set; } = "https://tatoeba.org/eng/contributions/latest/";
        public string Search { get; set; } = "https://tatoeba.org/eng/sentences/search?";
        public string Sentence { get; set; } = " https://tatoeba.org/eng/sentences/show/";
        public string SaveTranslation { get; set; } = "https://tatoeba.org/eng/sentences/save_translation";
        public string EditSentence { get; set; } = "https://tatoeba.org/eng/sentences/edit_sentence";
    }

    public class XpathConfig
    {
        public XpathLoginConfig LoginConfig { get; set; } = new XpathLoginConfig();
        public XpathSentenceDetailConfig SentenceDetailConfig { get; set; } = new XpathSentenceDetailConfig();        
        public XpathContribConfig ContribConfig { get; set; } = new XpathContribConfig();        
    }

    public class XpathSentenceDetailConfig
    {
        public XpathTranslationConfig TranslationConfig { get; set; } = new XpathTranslationConfig();
        public XpathCommentConfig CommentConfig { get; set; } = new XpathCommentConfig();
        public XpathLogConfig LogConfig { get; set; } = new XpathLogConfig();
    }

    public class XpathLoginConfig
    {
        public XpathPathConfig KeyPath { get; set; } = new XpathPathConfig { Path = "string(//*[@name='data[_Token][key]']/@value)" };
        public XpathPathConfig ValuePath { get; set; } = new XpathPathConfig { Path = "string(//*[@name='data[_Token][fields]']/@value)" };
        public XpathPathConfig SuccessPath { get; set; } = new XpathPathConfig { Path = "boolean(//li[@id='profile'][1])" };
    }

    public class XpathTranslationConfig
    {
        public XpathPathConfig IsEditablePath { get; set; } = new XpathPathConfig { Path = "boolean((//*[@class='sentence mainSentence']//*[@class='text correctnessZero editableSentence'])[1])" };
        public string ItemsPath { get; set; } = "//*[@class='sentence mainSentence']|//*[@class='translations']/*[@data-sentence-id]|//div[@class='more']/*[@data-sentence-id]";        

        public XpathPathConfig TextPath { get; set; } = new XpathPathConfig
        {
            Path = "string(.//*[@class='text correctnessZero']|.//*[@class='text correctnessZero editableSentence'])",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public XpathPathConfig DirectionPath { get; set; } = new XpathPathConfig { Path = "string(.//*[@class='text correctnessZero']/@dir|//*[@class='text correctnessZero editableSentence']/@dir)" };
        public XpathPathConfig IdPath { get; set; } = new XpathPathConfig { Path = "string(@data-sentence-id)" };
        public XpathPathConfig LanguagePath { get; set; } = new XpathPathConfig { Path = "string(.//img/@alt)" };
        public XpathPathConfig TranslationTypePath { get; set; } = new XpathPathConfig { Path = "string(div/a/@class)" };
    }

    public class XpathCommentConfig
    {
        public string ItemsPath { get; set; } = "//md-card";

        public XpathPathConfig UsernamePath { get; set; } = new XpathPathConfig
        {
            Path = "string(.//*[@class='md-title'])",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
        
        public XpathPathConfig ContentPath { get; set; } = new XpathPathConfig
        {
            Path = "string(.//md-card-content)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public XpathPathConfig DateTextPath { get; set; } = new XpathPathConfig
        {
            Path = "string(.//*[@class='md-subhead ellipsis'])",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
    }

    public class XpathLogConfig
    {
        public string ItemsPath { get; set; } = "//md-list-item";
        public XpathPathConfig TextPath { get; set; } = new XpathPathConfig
        {
            Path = "string(div/div)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
        public XpathPathConfig DateTextPath { get; set; } = new XpathPathConfig
        {
            Path = "string(div/p)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };
        public XpathPathConfig ContribTypePath { get; set; } = new XpathPathConfig { Path = "string(@class)" };
    }

    public class XpathContribConfig
    {
        public string ItemsPath { get; set; } = "//md-list-item";

        public XpathPathConfig TextPath { get; set; } = new XpathPathConfig
        {
            Path = "string(div/div)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public XpathPathConfig DirectionPath { get; set; } = new XpathPathConfig { Path = "string(div/div/@dir)" };

        public XpathPathConfig DateTextPath { get; set; } = new XpathPathConfig
        {
            Path = "string(div/p)",
            Action = XpathActionConfig.HtmlDecodeAndTrim,
        };

        public XpathPathConfig IdPath { get; set; } = new XpathPathConfig { Path = "string(md-button/@href)" };
        public XpathPathConfig LanguagePath { get; set; } = new XpathPathConfig { Path = "string(img/@alt)" };
        public XpathPathConfig ContribTypePath { get; set; } = new XpathPathConfig { Path = "string(@class)" };
    }

    public enum XpathActionConfig
    {
        None = 0,
        Trim = 1,
        HtmlDecode = 2,
        HtmlDecodeAndTrim = 3,
    }

    public class XpathPathConfig
    {
        public string Path { get; set; }
        public XpathActionConfig Action { get; set; }
    }
}
