using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fikarender.Data
{
    public class Service
    {
        public Service()
        {
        }

        [Key]
        public int ServiceId { get; set; }

        [Display(Name = "عنوان سرویس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید!")]
        [MaxLength(100, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string ServiceTitle { get; set; }

        [Display(Name = "عنوان متا سرویس")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string ServiceMetaTitle { get; set; }

        [Display(Name = "شرح متا سرویس")]
        [MaxLength(600, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string ServiceMetaDesc { get; set; }

        public int? ParentId { get; set; }

        [Display(Name = "تصویر")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(600, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string ServicePicture { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(200, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string Content { get; set; }


        #region Navigation Property

        public virtual ICollection<WorkSample> WorkSamples { get; set; }    

        #endregion
    }
}
