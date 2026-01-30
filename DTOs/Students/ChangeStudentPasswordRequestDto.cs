using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Students
{
    public class ChangeStudentPasswordRequestDto
    {
        [Required(ErrorMessage = "Student ID is required")]
        public string StudentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
