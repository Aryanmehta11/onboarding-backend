namespace onboardingAPI.DTOs
{
    public class CreateTemplateTaskRequest
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = "";
        public bool IsRequired { get; set; } = true;
        public int OrderIndex { get; set; }
    }
}
