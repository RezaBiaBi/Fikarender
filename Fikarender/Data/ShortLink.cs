using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class ShortLink
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(5)]
        public string ShortKey { get; set; }
        public byte Type { get; set; }
        public int ItemId { get; set; }
    }
}
