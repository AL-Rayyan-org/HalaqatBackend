namespace HalaqatBackend.DTOs.Groups

{
    public class GroupResponseDto
    {
        
        public string GroupId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string RecitationDays { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public int MembersLimit { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}