namespace HalaqatBackend.Models
{
    public class Attendance
    {
        public string Id { get; set; }
        public string StudentId { get; set; }
        public string GroupId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}