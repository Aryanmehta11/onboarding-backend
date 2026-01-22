using Microsoft.AspNetCore.Mvc;
using onboardingAPI.DTOs;
using onboardingAPI.Services;
using onboardingAPI.Models;
using onboardingAPI.Data;

namespace onnboardinAPI.Controllers
{
    [ApiController]
    [Route("api/admin/ai")]
    public class AIController : ControllerBase
    {
        private readonly GeminiAIService _geminiAIService;
        private readonly AppDbContext _context;
        public AIController(GeminiAIService geminiAIService, AppDbContext context)
        {
            _geminiAIService = geminiAIService;
            _context = context;
        }

       [HttpPost("generate-onboarding-template")]
       public async Task<IActionResult> GenerateOnboardingTemplate([FromBody] GenerateOnboardingRequest request)
        {
            var tasks=await _geminiAIService.GenerateOnboardingTasks(request.RoleName, request.TechStack, request.ProjectName);

            var onboardingTemplate = new OnboardingTemplate
            {
                RoleName = request.RoleName,
                Description = $"AI Generated onboarding for {request.RoleName}",
                CreatedAt = DateTime.UtcNow
            };

            _context.OnboardingTemplates.Add(onboardingTemplate);
            await _context.SaveChangesAsync();

            int order = 1;
            foreach(var task in tasks)
            {
                _context.OnboardingTemplateTasks.Add(new OnboardingTemplateTask
                {
                    TemplateId = onboardingTemplate.Id,
                    Title = task,
                    Description = "",
                    IsRequired = true,
                    OrderIndex = order++
                });
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message="Onboarding template generated successfully",
                templateId=onboardingTemplate.Id,
                TaskCount=tasks.Count
            });

        }

    }
}