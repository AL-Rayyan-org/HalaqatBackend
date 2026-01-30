namespace HalaqatBackend.DTOs.Students
{
    public class StudentResponseDto
    {
        public string StudentId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string? Info { get; set; }
        public DateTime JoinedOn { get; set; }
    }
}
