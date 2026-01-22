using Microsoft.AspNetCore.Identity;

namespace HalaqatBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string? Email {  get; set; }
        public override string? PasswordHash { get; set; }
        public override string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public Roles Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
