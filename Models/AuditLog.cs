namespace HalaqatBackend.Models
{
    public class AuditLog
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public DateTime Date { get; set; }
    }
}
