using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class Blog
    {
        [Key]
        public int BlogId { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }
        
        [Display(Name = "عنوان متا")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MetaTitle { get; set; }

        [Display(Name = "شرح")]
        [StringLength(512, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "توضیحات متا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(512, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MetaDescription { get; set; }
        
        [Display(Name = "خلاصه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(512, MinimumLength = 50, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Summery { get; set; }

        [Display(Name = "تاریخ")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
        [Display(Name = "تاریخ بروزرسانی")]
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        
        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Content { get; set; }

        [Display(Name = "تصویر اصلی")]
        [StringLength(50)]
        public string DeskImage { get; set; }
        
        [Display(Name = "تصویر موبایل")]
        [StringLength(50)]
        public string MobileImage { get; set; }

        [Display(Name = "بازدید")]
        public int ViewCount { get; set; } = 0;

        
        /*[Display(Name = "نویسنده")]
        [StringLength(450)]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }*/
        public virtual ICollection<BlogTag> BlogTags { get; set; }
    }
}
