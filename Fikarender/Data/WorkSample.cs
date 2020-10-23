using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Fikarender.Models;

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
        [Required(ErrorMessage = "لطفا {0} راوارد کنید!")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1} کاراکتر باشد")]
        public string Title { get; set; }

        [Display(Name = "عنوان متا")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1} کاراکتر باشد")]
        public string MetaTitle { get; set; }

        [Required]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }

        [Display(Name = "نمایش در صفحه خدمات؟ ")]
        public bool IsShow { get; set; }

        [Display(Name = "وضعیت")]
        public bool Status { get; set; }

        /*[Display(Name = "نوع فایل")]
        [Required(ErrorMessage = "لطفا {0} راوارد کنید!")]
        public byte SampleType { get; set; }  */  

        [Display(Name = "شرح نمونه‌کار")]
        [Required(ErrorMessage = "لطفا {0} راوارد کنید!")]
        [MaxLength(600, ErrorMessage = "{0}نمیتواند بیشتراز {1} کاراکتر باشد")]
        public string Description { get; set; }

        [Display(Name = "فایل نمونه‌کار")]
        [MaxLength(1024, ErrorMessage = "{0}نمیتواند بیشتراز {1} کاراکتر باشد")]
        public string DocumentFile { get; set; }

        [Display(Name = "لینک ویدئو نمونه‌کار")]
        [MaxLength(1024, ErrorMessage = "{0}نمیتواند بیشتراز {1} کاراکتر باشد")]
        public string SampleVideoLink { get; set; }
        
        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(ErrorMessage = "{0}نمیتواند بیشتراز {1} کاراکتر باشد")]
        public string LongContent { get; set; } 
    }
}
