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

            Return only a json array of strings. No markdown, no explanation.
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

            if (!response.IsSuccessStatusCode)
            {
                var error= await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API request failed with status code {response.StatusCode}: {error}");
            }

            var responseText=await response.Content.ReadAsStringAsync();

            using var doc=JsonDocument.Parse(responseText);
            var text=doc
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text") 
            .GetString();

             if (string.IsNullOrWhiteSpace(text))
                throw new Exception("Gemini returned empty response");

            return JsonSerializer.Deserialize<List<string>>(text)!;
        }
        
    } 
}