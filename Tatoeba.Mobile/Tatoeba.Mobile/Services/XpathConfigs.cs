namespace Tatoeba.Mobile.Services
{
    public class XpathLoginConfig
    {
        public readonly static string KeyPath = "string(//*[@name=\"data[_Token][key]\"]/@value)";
        public readonly static string ValuePath = "string(//*[@name=\"data[_Token][fields]\"]/@value)";
    }

    public class XpathTranslationConfig
    {
        public readonly static string IsEditablePath = "boolean((//*[@class=\"sentence mainSentence\"]//*[@class=\"text correctnessZero editableSentence\"])[1])";
        public readonly static string ItemsPath = "//*[@class=\"sentence mainSentence\"]|//*[@class=\"translations\"]/*[@data-sentence-id]|//div[@class=\"more\"]/*[@data-sentence-id]";
        public readonly static string TextPath = "string(.//*[@class=\"text correctnessZero\"]|.//*[@class=\"text correctnessZero editableSentence\"])";
        public readonly static string DirectionPath = "string(.//*[@class=\"text correctnessZero\"]/@dir|//*[@class=\"text correctnessZero editableSentence\"]/@dir)";
        public readonly static string IdPath = "string(@data-sentence-id)";
        public readonly static string LanguagePath = "string(.//img/@alt)";
        public readonly static string TranslationTypePath = "string(div/a/@class)";
    }

    public class XpathLogConfig
    {
        public readonly static string ItemsPath = "//md-list-item";
        public readonly static string TextPath = "string(div/div)";
        public readonly static string DateTextPath = "string(div/p)";  
        public readonly static string ContribTypePath = "string(@class)";
    }

    public class XpathCommentConfig
    {
        public readonly static string ItemsPath = "//md-card";
        public readonly static string UsernamePath = "string(.//*[@class=\"md-title\"])";
        public readonly static string ContentPath = "string(.//md-card-content)";
        public readonly static string DateTextPath = "string(.//*[@class=\"md-subhead ellipsis\"])";
    }

    public class XpathContribConfig
    {
        public readonly static string ItemsPath = "//md-list-item";
        public readonly static string TextPath = "string(div/div)";
        public readonly static string DirectionPath = "string(div/div/@dir)";
        public readonly static string DateTextPath = "string(div/p)";
        public readonly static string IdPath = "string(md-button/@href)";
        public readonly static string LanguagePath = "string(img/@alt)";
        public readonly static string ContribTypePath = "string(@class)";
    }
}
