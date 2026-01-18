namespace Al_Rayyan.Models
{
    public class Group
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Array Deadlines { get; set; }
        public string Info { get; set; }
        public DateOnly JoinedAt { get; set; }
        Group() { }
    }
}
