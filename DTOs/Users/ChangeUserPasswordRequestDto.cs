using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Users
{
    public class ChangeUserPasswordRequestDto
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
