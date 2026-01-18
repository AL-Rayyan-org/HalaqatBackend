using Microsoft.AspNetCore.Identity;

namespace Al_Rayyan.Models
{
    public class User : IdentityUser<int>
    {
        public int ID { get; set; }
        public string? FullName { get; set; }
        public override string? Email {  get; set; }
        public override string? PasswordHash { get; set; }
        public override string? PhoneNumber { get; set; }
        public Roles UserRoles { get; set; }
        public DateOnly CreatedAt { get; set; }
        User() { }
    }
}
