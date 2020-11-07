using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class WorkSample
    {
        [Key]
        public int WorkSampleId { get; set; }

        [Display(Name = "عنوان نمونه‌کار")]
        [Required(ErrorMessage = "لطفا {0} راوارد کنید!")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "عنوان متا")]
        [Required(ErrorMessage = "لطفا {0} راوارد کنید!")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
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
        [StringLength(512, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(128)]
        public string DocumentFile { get; set; }
        
        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "لطفا {0} راوارد کنید!")]
        public string LongContent { get; set; } 
    }
}
