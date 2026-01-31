namespace HalaqatBackend.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RecitationDays { get; set; } // e.g., "Monday,Wednesday"
        public bool IsDefault { get; set; }
        public int MembersLimit { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
