using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Users
{
    public class ChangeUserRoleRequestDto
    {
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;
    }
}
