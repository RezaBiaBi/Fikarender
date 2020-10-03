using System;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Data
{
    public class Config
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "تلفن")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Tel { get; set; }

        [Display(Name = "ایمیل")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Email { get; set; }

        [Display(Name = "آدرس")]
        [StringLength(512, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Address { get; set; }

        [Display(Name = "اینستاگرام")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Instagram { get; set; }

        [Display(Name = "تلگرام")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Telegram { get; set; }

        [Display(Name = "فیسبوک")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Facebook { get; set; }

        [Display(Name = "توئیتر")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Twitter { get; set; }

        [Display(Name = "گوگل")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Google { get; set; }

        [Display(Name = "واتساپ")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Whatsapp { get; set; }

        [Display(Name = "مرکز پیام")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string SmsCenter { get; set; }

        [Display(Name = "نام کاربری پیامک")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string SmsUser { get; set; }

        [Display(Name = "کلمه عبور پیامک")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string SmsPass { get; set; }

        [Display(Name = "دامنه SMTP")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailSmtpDomain { get; set; }

        [Display(Name = "نام کاربری ایمیل")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailUserName { get; set; }

        [Display(Name = "کلمه عبور ایمیل")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailPassword { get; set; }

        [Display(Name = "عنوان ایمیل")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string MailDisplayName { get; set; }

        [Display(Name = "دامنه سایت")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string Domain { get; set; }

        [Display(Name = "دامنه CDN")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ContentDeliveryNetwork { get; set; }

        [Display(Name = "عنوان صفحه خانه")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string HomeTitle { get; set; }

        [Display(Name = "توضیحات متا صفحه خانه")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string HomeMetaDesc { get; set; }

        [Display(Name = "دسته اسلایدر صفحه خانه")]
        public int HomeActiveSliderCategoryId { get; set; }

        [Display(Name = "عنوان صفحه تماس با ما")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ContactTitle { get; set; }

        [Display(Name = "توضیحات متا صفحه تماس با ما")]
        [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} باید بین {2} تا {1} کاراکتر باشد.")]
        public string ContactMetaDesc { get; set; }

        [Display(Name = "محتوا و توضیحات صفحه تماس با ما")]
        public string ContactContent { get; set; }

        [Display(Name = "پیوند فریم نقشه های گوگل")]
        public string MapUrl { get; set; }

        [Display(Name = "متن فوتر")]
        public string FooterText { get; set; }

        [Display(Name = "شماره سفارش")]
        public int OrderNumber { get; set; } = 10000;

        [Display(Name = "محدودیت ارسال رایگان سبد")]
        public double FreeShippingLimit { get; set; } = 200000;

        [Display(Name = "هزینه ارسال")]
        public double ShippingPrice { get; set; } = 10000;
    }
}
