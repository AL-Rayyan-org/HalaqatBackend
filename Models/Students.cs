namespace Al_Rayyan.Models
{
    public class Students
    {
        public int ID { get; set; }
        public int User_id { get; set; }
        public int GroupId { get; set; }
        public string Info { get; set; }
        public int DaysMissed { get; set; }
        public int DeadlinesMissed { get; set; }
        public DateOnly JoinedAt { get; set; }
        Students() { }
    }
}
