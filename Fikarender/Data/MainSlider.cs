using System;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class MainSlider
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Title { get; set; }

        [Display(Name = "تاریخ درج")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "چند روز نمایش داده شود؟")]
        public int? Expire { get; set; }

        [StringLength(50)]
        [Display(Name = "تصویر")]
        public string Image { get; set; }

        [StringLength(50)]
        [Display(Name = "تصویر موبایل")]
        public string ImageSm { get; set; }

        [Display(Name = "لینک")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string Link { get; set; }

        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public byte Sort { get; set; }
    }
}
