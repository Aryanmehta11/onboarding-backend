using Microsoft.EntityFrameworkCore;
using onboardingAPI.Models;


namespace onboardingAPI.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Project> Projects {get;set;}
        public DbSet<ProjectModule> ProjectModules {get;set;}
        public DbSet<ProjectTechStack> ProjectTechStack {get;set;}   
        public DbSet<ProjectMember> ProjectMembers {get;set;}
        public DbSet<OnboardingTemplate> OnboardingTemplates { get; set; }
    
    }
}
