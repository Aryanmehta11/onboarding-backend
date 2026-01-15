namespace onboardingAPI.Models
{
    public class OnboardingTemplateTask
    {
        public int Id { get; set; }

        public int TemplateId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = "";

        public bool IsRequired { get; set; } = true;

        public int OrderIndex { get; set; }
    }
}
