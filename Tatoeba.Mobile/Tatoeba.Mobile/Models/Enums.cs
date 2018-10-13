namespace Tatoeba.Mobile.Models
{
    public enum ContribType
    {
        Unknown = -1,
        Insert = 0,
        Update = 1,
        Obsolete = 2,
        Delete = 3,
        LinkInsert = 4,
        LinkDelete = 5,
    }

    public enum Direction
    {
        Unknown = -1,
        LeftToRight = 0,
        RightToLeft = 1,        
    }

    public enum TranslationType
    {
        Unknown = -1,
        Original = 0,
        Direct = 1,
        Indirect = 2,
    }
}