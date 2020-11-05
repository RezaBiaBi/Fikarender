using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class Faq
    {
        [Key]
        public int FaqId { get; set; }

        [Display(Name = "سوال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Question { get; set; }

        [Display(Name = "پاسخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Answer { get; set; }

        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public int Sort { get; set; }


        public virtual ICollection<FaqTag> FaqTags { get; set; }
    }
}
