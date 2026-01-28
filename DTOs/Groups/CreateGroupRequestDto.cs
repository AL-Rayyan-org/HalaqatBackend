using System.ComponentModel.DataAnnotations;

namespace HalaqatBackend.DTOs.Groups{

    public class CreateGroupRequestDto
    {
        [Required(ErrorMessage ="Group Name is Required")]
        public string Name {get; set; }=string.Empty;

        
        public string RecitationDays {get; set;}=string.Empty;

        
    }
}