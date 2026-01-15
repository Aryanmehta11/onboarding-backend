namespace onboardingAPI.Models
{
    public class OnboardingTemplate
    {
        public int Id { get; set; }

        public string RoleName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
