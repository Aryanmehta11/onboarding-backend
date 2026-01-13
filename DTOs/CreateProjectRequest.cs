using System.ComponentModel.DataAnnotations;

namespace onboardingAPI.DTOs
{
    public class CreateProjectRequest
    {

    
        [MinLength(3)]
        public required string Name { get; set; }
        [Required]
        public int MentorId { get; set; }
    }
}