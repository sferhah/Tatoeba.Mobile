using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public class SentenceDetail
    {
        public string Id { get; set; }
        public List<Contribution> Sentences { get; set; } = new List<Contribution>();
        public List<Log> Logs { get; set; } = new List<Log>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public bool IsEditable { get; set; }
    }

    public class Comment
    {
        public string Username { get; set; }
        public string DateText { get; set; }
        public string Content { get; set; }        
    }

    public class SentenceBase
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public Direction Direction { get; set; }
        public Language Language { get; set; }
    }

    public class Contribution : SentenceBase
    {      
        public string DateText { get; set; }
        public ContribType ContribType { get; set; }
        public TranslationType TranslationType { get; set; } 
    }

    public class Log : SentenceBase
    {
        public string DateText { get; set; }
        public ContribType ContribType { get; set; }
    }

    public class Language
    {
        [Key]
        public string Iso { get; set; }
        public string Label { get; set; }
        public byte[] Flag { get; set; }
    }
}