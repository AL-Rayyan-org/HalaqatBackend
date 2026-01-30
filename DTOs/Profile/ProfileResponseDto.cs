using HalaqatBackend.Enums;

namespace HalaqatBackend.DTOs.Profile
{
    public class ProfileResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public Roles Role { get; set; }
        public Gender Gender { get; set; }
    }
}
