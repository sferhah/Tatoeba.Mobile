using System.ComponentModel.DataAnnotations;

namespace Tatoeba.Mobile.Models
{
    public class Language
    {
        [Key]
        public string Iso { get; set; }
        public string Label { get; set; }
        public byte[] Flag { get; set; }
    }
}
