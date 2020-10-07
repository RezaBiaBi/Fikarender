using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fikarender.Data
{
    public class Team
    {
        public Team()
        {
        }

        [Key]
        public int TeamId { get; set; }

        [Display(Name = "نام و نام‌خانوادگی")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [MaxLength(300, ErrorMessage = "{0}نمیتواند بیشتراز {1}کارامتر باشد")]
        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [EmailAddress(ErrorMessage = "متن وارد شده شباهتی به ایمیل ندارد.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Email { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} عدد باشد.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "سمت")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Degree { get; set; }

        [Display(Name = "عکس")]
        [StringLength(50)]
        public string Avatar { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Description { get; set; }

        [Display(Name = "ترتیب")]
        public int Sort { get; set; }

    }
}
