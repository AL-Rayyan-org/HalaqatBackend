namespace HalaqatBackend.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RecitationDays { get; set; } // e.g., "Monday,Wednesday"
        public DateTime CreatedAt { get; set; }
    }
}
