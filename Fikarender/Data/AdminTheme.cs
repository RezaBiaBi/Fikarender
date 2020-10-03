using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class AdminTheme
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Theme { get; set; }
    }
}
