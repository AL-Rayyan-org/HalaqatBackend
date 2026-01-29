using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Groups
{
    public class SetDefaultGroupRequestDto
    {
        [Required(ErrorMessage = "Group ID is required")]
        public string GroupId { get; set; } = string.Empty;
    }
}
