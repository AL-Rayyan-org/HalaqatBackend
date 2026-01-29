using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Groups{

    public class CreateGroupRequestDto
    {
        [Required(ErrorMessage ="Group Name is Required")]
        public string Name {get; set; }=string.Empty;

        
        public string RecitationDays {get; set;}=string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Members limit must be greater than 0")]
        public int MembersLimit { get; set; } = 10;
    }
}