namespace HalaqatBackend.Models
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public Roles Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
