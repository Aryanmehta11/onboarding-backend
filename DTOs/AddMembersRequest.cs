namespace onboardingAPI.DTOs
{
    public class AddMembersRequest
    {
        public List<int> UserIds { get; set; } = new();
    }
}