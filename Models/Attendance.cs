namespace Al_Rayyan.Models
{
    public class Attendance
    {
        public int ID { get; set; }
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public DateOnly Date { get; set; }
        public string Status { get; set; }
        Attendance() { }
    }
}