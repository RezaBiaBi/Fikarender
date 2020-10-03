using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class UsersFeedback
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string UserName { get; set; }

        [Display(Name = "نقش کاربر")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string UserRole { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "ترتیب")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public byte Sort { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(50)]
        public string Picture { get; set; }
    }
}
