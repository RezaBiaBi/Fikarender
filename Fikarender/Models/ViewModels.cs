using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fikarender.Models
{
    public class EditUserVM
    {
        public string Id { get; set; }

        public DateTime? Birth { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string BirthString { get; set; }

        [Display(Name = "نام")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(10, ErrorMessage = "فیلد {0} باید {2} رقم باشد..", MinimumLength = 10)]
        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }

        [Display(Name = "تلفن همراه")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "تایید تلفن همراه")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل صحیح نیست.")]
        [StringLength(256, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 5)]
        public string Email { get; set; }

        [Display(Name = "تایید ایمیل")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "تلفن ثابت")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        public string Tel { get; set; }

        [Display(Name = "عکس پرسنلی")]
        public string Avatar { get; set; }

        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }
    }

    public class ReplyContactMessageVM
    {
        [Display(Name = "شناسه پیام")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public int ContactId { get; set; }

        [Display(Name = "متن پاسخ")]
        [StringLength(5000, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string ContactAnswer { get; set; }
    }

    #region Profile ViewModels
    public class LoginVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "من را به خاطر بسپار")]
        public bool RememberMe { get; set; }
    }

    public class RegisterVM
    {
        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public string Lastname { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string Username { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        public bool Gender { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "قوانین و مقررات")]
        [Required(ErrorMessage = "پذیرش {0} الزامی است.")]
        public bool Rules { get; set; }
    }

    public class ConfirmPhoneNumberVM
    {
        [Display(Name = "کد فعالسازی")]
        [Required(ErrorMessage = "لطفا کد را وارد نمائید.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "کد وارد شده صحیح نیست.")]
        public string Code { get; set; }

        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد نمائید.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره تلفن همراه باید 11 رقم باشد.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }
    }

    public class SendNewCodeVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "لطفا شماره تلفن همراه خود را وارد نمائید.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "شماره تلفن همراه باید 11 رقم باشد.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }
    }

    public class ResetPasswordVM
    {
        [Display(Name = "شماره تلفن همراه")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} باید متنی بین {2} الی {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور")]
        [Compare("Password", ErrorMessage = "تکرار کلمه عبور با کلمه عبور وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "کد 6 رقمی پیامک شده به شما")]
        [Required(ErrorMessage = "وارد کردن {0} الزامی است.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "کد وارد شده صحیح نیست.")]
        public string Code { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required(ErrorMessage = "** وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور فعلی")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "** وارد کردن {0} الزامی است.")]
        [StringLength(100, ErrorMessage = "{0} بین {2} تا {1} کاراکتر باشد.", MinimumLength = 6)]
        [RegularExpression(@"((?=.*\d)(?=.*[a-z]).{6,100})", ErrorMessage = "کلمه عبور باید حداقل 6 کاراکتر طول داشته باشد و ترکیبی از اعداد ('0'-'9') و حروف انگلیسی کوچک ('a'-'z') باشد.")]
        [DataType(DataType.Password)]
        [Display(Name = "کلمه عبور جدید")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "** وارد کردن {0} الزامی است.")]
        [DataType(DataType.Password)]
        [Display(Name = "تکرار کلمه عبور جدید")]
        [Compare("NewPassword", ErrorMessage = "{0} با کلمه عبور جدید وارد شده یکسان نیست.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditUserInfoVM
    {
        public string Id { get; set; }

        public DateTime? Birth { get; set; }

        [Display(Name = "تاریخ تولد")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string BirthString { get; set; }

        [Display(Name = "نام")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [StringLength(128, ErrorMessage = "فیلد {0} باید بین {2} تا {1} حرف باشد.", MinimumLength = 2)]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "لطفا {0} را وارد کنید.")]
        [StringLength(10, ErrorMessage = "فیلد {0} باید {2} رقم باشد..", MinimumLength = 10)]
        [Display(Name = "کد ملی")]
        public string NationalId { get; set; }

        [Display(Name = "تلفن ثابت")]
        [StringLength(11, ErrorMessage = "فیلد {0} باید {1} عدد باشد.", MinimumLength = 11)]
        public string Tel { get; set; }

        [Display(Name = "عکس پرسنلی")]
        public string Avatar { get; set; }
    }

    public class EditProfileVM
    {
        public string Id { get; set; }

        [Display(Name = "نام")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین 2 الی 128 حرف باشد.")]
        public string Firstname { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(128, MinimumLength = 2, ErrorMessage = "{0} باید بین 2 الی 128 حرف باشد.")]
        public string Lastname { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده صحیح نیست.")]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }

        [Display(Name = "کد ملی")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید 10 رقم باشد.")]
        public string NationalId { get; set; }

        [Display(Name = "جنسیت")]
        public bool Gender { get; set; }

        [Display(Name = "شماره تلفن ثابت")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید 11 رقم باشد.")]
        public string Tel { get; set; }
        public DateTime? Birth { get; set; }
        public string Avatar { get; set; }
    }

    public class AddressVM
    {
        public AddressVM()
        {
        }

        public int AddressId { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [Display(Name = "نام و نام خانوادگی گیرنده")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string ReceiverName { get; set; }

        [Display(Name = "تلفن همراه گیرنده")]
        [RegularExpression(@"09(0[0-9]|1[0-9]|3[1-9]|2[1-9]|9[0-9])-?[0-9]{3}-?[0-9]{4}", ErrorMessage = "شماره همراه وارد شده معتبر نیست.")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string StateName { get; set; }
        
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string CityName { get; set; }

        [Display(Name = "آدرس پستی")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        public string PostalAddress { get; set; }

        [Display(Name = "کد پستی")]
        [RegularExpression(@"\d{10}", ErrorMessage = "کد پستی وارد شده صحیح نیست.")]
        [Required(ErrorMessage = "پر کردن {0} الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "{0} باید {1} رقم باشد.")]
        public string PostalCode { get; set; }

    }
    #endregion
}
