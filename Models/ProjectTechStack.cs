namespace onboardingAPI.Models
{
    public class ProjectTechStack
    {
        public int Id {get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;  
        public string Name { get; set; } = null!;
    }
}