namespace onboardingAPI.Models
{
    public class UserOnboarding
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TemplateId { get; set; }
        public string Status { get; set; } = "IN_PROGRESS";
        public DateTime StartDate { get; set; }
    }
}
