namespace HalaqatBackend.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RecitationDays { get; set; } // e.g., "Monday,Wednesday"
        public int MembersLimit { get; set; } = 10; // Default limit of 10 students
        public bool IsDefault { get; set; } = false; // Default group for storing deleted group data
        public DateTime CreatedAt { get; set; }
    }
}
