using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class Service
    {
        [Key]
        public int ServiceId { get; set; }

        [Display(Name = "عنوان سرویس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ServiceTitle { get; set; }

        [Display(Name = "عنوان متا سرویس")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ServiceMetaTitle { get; set; }

        [Display(Name = "شرح متا سرویس")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ServiceMetaDesc { get; set; }

        [Display(Name = "دسته اصلی")]
        public int ParentId { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(128)]
        public string ServicePicture { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public string Content { get; set; }

        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        public byte Sort { get; set; }


        #region Navigation Property 
        public virtual ICollection<WorkSample> WorkSamples { get; set; }
        #endregion
    }
}
