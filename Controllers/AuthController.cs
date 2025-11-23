using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onboardingAPI.Data;
using onboardingAPI.DTOs;


namespace onboardingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Email==request.Email); // Retreving Users

            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            if (user.PasswordHash != request.Password)
                return Unauthorized(new { message = "Invalid Password" });

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role 

            });

            
        }
    }
}
