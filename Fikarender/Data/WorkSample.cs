using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fikarender.Data
{
    public class WorkSample
    {
        public WorkSample()
        {
        }

        [Key]
        public int WorkSampleId { get; set; }

        [Display(Name = "عنوان نمونه‌کار")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string Title { get; set; }

        [Display(Name = "عنوان متا")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string MetaTitle { get; set; }

        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [Display(Name = "نمایش در ")]
        public bool IsShow { get; set; }

        [Display(Name = "وضعیت")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public byte Status { get; set; }

        [Display(Name = "شرح نمونه‌کار")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(600, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string Description { get; set; }

        [Display(Name = "تصویر نمونه‌کار")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(600, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string Picture { get; set; }
        
        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(1024, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string LongContent { get; set; } 
    }
}
