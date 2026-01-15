namespace onboardingAPI.Models
{
    public class UserOnboardingTask
    {
        public int Id { get; set; }
        public int UserOnboardingId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = "";
        public bool IsRequired { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime? CompletedAt { get; set; }
    }
}
