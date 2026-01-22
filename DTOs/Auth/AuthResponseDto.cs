namespace HalaqatBackend.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public Roles Role { get; set; }
        public string AccessToken { get; set; } = string.Empty;
    }
}
