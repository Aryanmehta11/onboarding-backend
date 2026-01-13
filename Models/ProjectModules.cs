namespace onboardingAPI.Models
{
    public class ProjectModule
    {
         public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string Name { get; set; } = null!;
    }
}