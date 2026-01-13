namespace onboardingAPI.Models
{
    public class Project
    {
        public int Id {get;set;}
        public string Name {get;set;}
        public int MentorId {get;set;}
       
        public User Mentor {get;set;}
        public DateTime CreatedAt {get;set;}

    }
}