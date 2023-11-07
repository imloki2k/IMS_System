using System.ComponentModel.DataAnnotations;

namespace IMS_System.ModelViews
{
    public class LoginModelView
    {
        [MaxLength(300)]
        [Required(ErrorMessage = "Vui lòng nhập Email hoặc số điện thoại")]
        [Display(Name = "Địa chỉ Email hoặc số điện thoại")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }

    }
}
