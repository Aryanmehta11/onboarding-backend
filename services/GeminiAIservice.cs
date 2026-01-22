using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace onboardingAPI.Services
{
    public  class GeminiAIService
    {
        private readonly HttpClient httpClient;
        private readonly string _apiKey;


        public GeminiAIService(HttpClient httpClient, IConfiguration config)
        {
            this.httpClient = httpClient;
            _apiKey =config["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini Api Key is missing");
        }

        public async Task<List<string>> GenerateOnboardingTasks(string role , string techStack, string project)
        {
            var prompt = $@"
            You are an enterprise onboarding expert.
            Create onboarding tasks.

            Role: {role}
            Project: {project}
            Tech Stack: {techStack}

            Return output strictly as JSON array of task titles.
            ";  

            var body= new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new{text=prompt}
                        }
                    }
                }
            };



            var json= JsonSerializer.Serialize(body);
            var content= new StringContent(json, Encoding.UTF8, "application/json");

            var response =await httpClient.
            PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}", content);

            var responseText=await response.Content.ReadAsStringAsync();

            //Extract JSON array from response 
            var start=responseText.IndexOf('[');
            var end=responseText.LastIndexOf(']');  
            var jsonArrayString=responseText.Substring(start, end - start +1);

            return JsonSerializer.Deserialize<List<string>>(jsonArrayString) ?? new List<string>();
        }
        
    } 
}