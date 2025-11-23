using Microsoft.EntityFrameworkCore;
using onboardingAPI.Models;


namespace onboardingAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
    
    }
}
