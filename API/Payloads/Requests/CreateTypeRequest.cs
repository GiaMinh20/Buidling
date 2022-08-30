using System.ComponentModel.DataAnnotations;

namespace API.Payloads.Requests
{
    public class CreateTypeRequest
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        public string Name { get; set; }
    }
}
