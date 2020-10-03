using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }

        [Display(Name = "نوع")]
        public byte Type { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Name { get; set; }

        [Display(Name = "عنوان مسیر (انگلیسی)")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string UniqueName { get; set; }


        public virtual ICollection<BlogTag> BlogTags { get; set; }

        public virtual ICollection<FaqTag> FaqTags { get; set; }
    }
}
