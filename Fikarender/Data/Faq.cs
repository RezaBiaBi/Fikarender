using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fikarender.Data
{
    public class Faq
    {
        public Faq()
        {
        }

        [Key]
        public int FaqId { get; set; }

        [Display(Name = "سوال")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        /*[StringLength(256, MinimumLength = 8, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]*/
        [MaxLength(ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Question { get; set; }

        [Display(Name = "پاسخ")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        /*[StringLength(256, MinimumLength = 8, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]*/
        [MaxLength(ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Answer { get; set; }

        [Display(Name = "ترتیب")]
        public int Sort { get; set; }


        public virtual ICollection<FaqTag> FaqTags { get; set; }
    }
}
