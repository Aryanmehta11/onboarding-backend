using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onboardingAPI.Data;
using onboardingAPI.DTOs;


[ApiController]
[Route("api/user/onboarding")]
public class UserOnboardingController:ControllerBase{
    private readonly AppDbContext _context;

    public UserOnboardingController (AppDbContext context){
        _context = context;
    }

    [HttpGet("{userId}/tasks")]
    public async Task<IActionResult> GetUserTasks (int userId){
        var onboarding=await _context.UserOnboardings
        .Where(u=>u.UserId==userId)
        .OrderByDescending(u=>u.StartDate)
        .FirstOrDefaultAsync();

        if(onboarding==null){
            return NotFound(new {message="No onboarding found for the user."});
        }

        var tasks=await _context.UserOnboardingTasks
        .Where(t=>t.UserOnboardingId==onboarding.Id)
        .Select(t=>new{
            t.Id,
            t.Title,
            t.Description,
            t.Status,
            t.CompletedAt,
        }).ToListAsync();

        return Ok(tasks);
    }

    [HttpPatch("tasks/{taskId}")]
    public async Task<IActionResult> updateTaskStatus (int taskId,[FromBody] UpdateTaskStatusRequest request){
        var task=await _context.UserOnboardingTasks.FindAsync(taskId);
        if(task==null){
            return NotFound(new {message="Task not found."});
        }

        task.Status=request.Status;
        if(request.Status=="COMPLETED")
            task.CompletedAt=DateTime.UtcNow;

        return Ok(new {message="Task status updated successfully."});    

    }
    //Optional Helper Endpoint to mark task as completed
    [HttpPost("tasks/{taskId}/complete")]
    public async Task<IActionResult> CompleteTask(int taskId){
        var task=await _context.UserOnboardingTasks.FindAsync(taskId);
        if(task==null){
            return NotFound(new {message="Task not found."});
        }

        task.Status="COMPLETED";
        task.CompletedAt=DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new {message="Task marked as completed."});    
    }

    [HttpGet("{userId}/progress")]
    public async Task<IActionResult> GetOnboardingProgress(int userId){
        var onboarding=await _context.UserOnboardings.
        Where(u=>u.UserId==userId)
        .OrderByDescending(u=>u.StartDate)
        .FirstOrDefaultAsync();

        if(onboarding==null){
            return NotFound(new {message="No onboarding found for the user."});
        }

        var tasks=await _context.UserOnboardingTasks
        .Where(t=>t.UserOnboardingId==onboarding.Id && t.IsRequired)  
        .ToListAsync(); 

       if(tasks.Count==0)
       {
            return Ok(new {
                progress=0,
                status=onboarding.Status});
       }

       var completedCount=tasks.Count(t=>t.Status=="COMPLETED");
       var totalCount=tasks.Count;
       var progress = (int)Math.Round((double)completedCount / totalCount * 100);

       if(progress==100 && onboarding.Status!="COMPLETED"){
            onboarding.Status="COMPLETED";
            await _context.SaveChangesAsync();
       }

       return Ok(new {
        onboardingId=onboarding.Id,
        progress,
        completedTasks=completedCount,
        totalTasks=totalCount,  
        status=onboarding.Status
       });

         
    }
}