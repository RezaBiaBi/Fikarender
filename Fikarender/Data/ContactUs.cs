using System;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 4, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string FullName { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [EmailAddress(ErrorMessage = "متن وارد شده شباهتی به ایمیل ندارد.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Email { get; set; }

        [Display(Name = "موضوع")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(256, MinimumLength = 4, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Subject { get; set; }

        [Display(Name = "متن پیغام")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(1024, MinimumLength = 20, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Message { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} عدد باشد.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "وضعیت")]
        public bool Status { get; set; }

        [Display(Name = "مشاهده شده")]
        public bool IsRead { get; set; }

        [Display(Name = "تاریخ ثبت")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "تاریخ پاسخ")]
        public DateTime? AnswerDate { get; set; }

        [Display(Name = "پاسخگو")]
        [StringLength(256)]
        public string AdminName { get; set; }

        [StringLength(450)]
        public string AdminId { get; set; }

        [Display(Name = "پاسخ")]
        public string Answer { get; set; }
    }
}
