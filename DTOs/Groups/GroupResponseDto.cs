namespace HalaqatBackend.DTOs.Groups

{
    public class GroupResponseDto
    {
        
        public string GroupId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string RecitationDays { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

}