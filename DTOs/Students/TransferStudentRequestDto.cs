using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Students
{
    public class TransferStudentRequestDto
    {
        [Required(ErrorMessage = "Student ID is required")]
        public string StudentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target Group ID is required")]
        public string TargetGroupId { get; set; } = string.Empty;
    }
}
