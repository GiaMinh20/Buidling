using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class RegisterRequest : LoginRequest
    {
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không trùng với mật khẩu đã nhập")]
        public string ConfirmPass { get; set; }
    }
}
