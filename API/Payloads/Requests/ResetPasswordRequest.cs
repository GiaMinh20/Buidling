using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không trùng với mật khẩu đã nhập")]
        public string ConfirmPassword { get; set; }
        public string Token { get; set; }
    }
}
