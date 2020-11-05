using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class Assist
    {
        [Key]
        public int AssistId { get; set; }

        [Required]
        [Display(Name = "نام و نام ‌خانوادگی")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string FullName { get; set; }

        /*public int StateId { get; set; }
        public int CityId { get; set; }*/
        [Display(Name = "شهر محل‌سکونت")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Location { get; set; }

        [Display(Name = "نشانی پیج اینستاگرام")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        public string SocialId { get; set; }

        [Display(Name = "تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0}راوارد کنید!")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "گرافیک")]
        public bool Graphic { get; set; }
        [Display(Name = "موشن گرافیک")]
        public bool MotionGraphic { get; set; }
        [Display(Name = "استاپ موشن")]
        public bool StopMotion { get; set; }
        [Display(Name = "انیمیت")]
        public bool Animate { get; set; }
        [Display(Name = "لوگو و هویت بصری")]
        public bool Logo { get; set; }
        [Display(Name = "رابط‌کاربری(UI)")]
        public bool Ui { get; set; }
        [Display(Name = "تجربه‌کاربری(UX)")]
        public bool Ux { get; set; }
        [Display(Name = "مدل‌سازی(3D)")]
        public bool Modeling3D { get; set; }
        [Display(Name = "تدوین")]
        public bool EditingVideo { get; set; }
        [Display(Name = "عکاسی")]
        public bool Photography { get; set; }
        [Display(Name = "فیلم‌برداری(تیزر رئال)")]
        public bool Filming { get; set; }
        [Display(Name = "نویسنده محتوا")]
        public bool ContentAuthor { get; set; }
        [Display(Name = "تصویرساز دیجیتال")]
        public bool DigitalIllustrator { get; set; }


        [Display(Name = "سایر تخصص‌ها")]
        public string OtherSpecialty { get; set; }

        [Display(Name = "فایل رزومه")]
        [Required]
        public string CvFileName { get; set; }
    }
}
