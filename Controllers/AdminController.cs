using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using onboardingAPI.Data;
using onboardingAPI.DTOs;
using onboardingAPI.Models;


[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    // Additional admin-related endpoints can be added here


    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users=await _context.Users.Select(u => new
        {
            u.Id,
            u.Name,
            u.Email,
            u.Role
        }).ToListAsync();
        return Ok(users);
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles=await _context.Roles.Select(r => new
        {
            r.Id,
            r.Name,
            r.IsSystem,
            r.CreatedAt
        }).ToListAsync();
        return Ok(roles);
    }


    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] Role role)
    {
        if (await _context.Roles.AnyAsync(r => r.Name == role.Name))
        {
            return BadRequest("Role with the same name already exists.");
        }
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
    }


    [HttpDelete("roles/{roleId}")]
    public async Task<IActionResult> DeleteRole(int roleId)
    {
        
        var role=await _context.Roles.FindAsync(roleId);
        if (role==null)
        {
            return NotFound();
        }
        //Prevent deletion of the system roles
        if (role.IsSystem)
        {
            return BadRequest("Cannot delete a system role.");
        }

        //Check if any users are assigned to this role
        var hasUsers=await _context.UserRoles.AnyAsync(userRole=>userRole.RoleId==roleId);
        if (hasUsers)
        {
            return BadRequest("Cannot delete role assigned to users.");
        }

        var permissions=await _context.RolePermissions.Where(RolePermission=>RolePermission.RoleId==roleId).ToListAsync();
        _context.RolePermissions.RemoveRange(permissions);

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return NoContent();


    }

    [HttpGet("sections")]
    public async Task<IActionResult> GetSections([FromBody] Section section)
    {
        var sections=await _context.Sections.Select(s => new
        {
            s.Id,
            s.KeyName,
            s.DisplayName,
            s.CreatedAt
        }).ToListAsync();
        return Ok(sections);
    }

    [HttpPost("sections")]
    public async Task<IActionResult> CreateSection([FromBody] Section section)
    {
        //Prevent Duplicate keynames
        if(await _context.Sections.AnyAsync(s => s.KeyName.ToLower() == section.KeyName.ToLower()))
        {
            return BadRequest(new{message="Section with the same key name already exists."});
        }
        section.CreatedAt=DateTime.UtcNow;
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSections), new { id = section.Id }, section);
    }

    [HttpDelete("sections/{sectionId}")]
    public async Task<IActionResult> DeleteSection(int sectionId){
        var section=await _context.Sections.FindAsync(sectionId);
        if(section==null){
            return NotFound();
        }
        //Check if any permissions are assigned to this section
        var hasPermissions=await _context.RolePermissions.AnyAsync(rp=>rp.SectionId==sectionId);
        if(hasPermissions){
            return BadRequest(new{message="Cannot delete section assigned to permissions."});
        }
        _context.Sections.Remove(section);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("rolepermissions/{roleId}")]
    public async Task<IActionResult> GetRolePermissions(int roleId) {
         var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId);
        if (!roleExists) return NotFound(new { message = "Role not found" });

        var perms = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => new {
                rp.Id,
                rp.RoleId,
                rp.SectionId,
                rp.CanView,
                rp.CanEdit,
                rp.CreatedAt
            })
            .ToListAsync();
        return Ok(perms);
     }


    [HttpPut("rolepermissions/{roleId}")]
    public async Task<IActionResult> UpdatePermissions(int roleId,[FromBody] List<RolePermission> newPerms)
    {
         // Validate role Existence
         var roleExists= await _context.Roles.AnyAsync(r=>r.Id==roleId);
        if(!roleExists) return NotFound(new {message="Role not found"});

        //Validate all section Ids
        var sectionIds=newPerms.Select(np=>np.SectionId).Distinct().ToList();
        var validSectionIds=await _context.Sections.Where(s=>sectionIds.Contains(s.Id)).Select(s=>s.Id).ToListAsync();
        if (validSectionIds.Count != sectionIds.Count)
        {
            return BadRequest(new {message="One or more invalid section IDs."});
        }

        var existing=_context.RolePermissions.Where(rp=>rp.RoleId==roleId);
        _context.RolePermissions.RemoveRange(existing);
        await _context.SaveChangesAsync();

        // Add new permissions
        foreach(var perm in newPerms)
        {
            perm.Id=0; //Ensure EF treats this as new
            perm.RoleId=roleId;
            perm.CreatedAt=DateTime.UtcNow;
            _context.RolePermissions.Add(perm);
        }
        await _context.SaveChangesAsync();

        return Ok(new{message="Permissions updated successfully.",count=newPerms.Count});
    }

    [HttpGet("permissions-for-user/{userId}")]
    public async Task<IActionResult> PermissionsForUser(int userId) {
        // find role(s) for user — assuming UserRoles table:
        var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == userId);
        if (userRole == null) return Ok(new { permissions = new List<object>() });

        var perms = await _context.RolePermissions.Where(rp => rp.RoleId == userRole.RoleId).ToListAsync();
        return Ok(perms);
    }

    //Projects Section 

    
    [HttpGet("projects")]
    public async Task<IActionResult> GetProjects()
    {
        var projects=await _context.Projects.Select(p => new
        {
            p.Id,
            p.Name,
            p.MentorId,
            p.CreatedAt
        }).ToListAsync();
        return Ok(projects);
    }

    [HttpPost("projects")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
       
       if(!ModelState.IsValid)
       {
        return BadRequest(ModelState);
       }
       var mentor= await _context.Users.FindAsync(request.MentorId);

       
       if (mentor==null)
       {
        return BadRequest(new {message="Mentor user not found."});
       }

       var project=new Project
       {
           Name=request.Name,
           MentorId=request.MentorId,
           CreatedAt=DateTime.UtcNow
       };

       _context.Projects.Add(project);
       await _context.SaveChangesAsync();

       return Ok(project);
    }
    
    [HttpGet("projects/{projectId}")]
    public async Task<IActionResult> GetProjectCard(int projectId)


    {
        var project=await  _context.Projects
        .Where(p=>p.Id==projectId)
        .Select(p=>new{p.Id,
          p.Name,
          Mentor=_context.Users
            .Where(u=>u.Id==p.MentorId)
            .Select(u=>u.Name)
            .FirstOrDefault(),

         MembersCount=_context.ProjectMembers
            .Count(pm=>pm.ProjectId==p.Id),

         TechStack=_context.ProjectTechStack
            .Where(pts=>pts.ProjectId==p.Id)
            .Select(pts=>pts.Name)
            .ToList(),

          Modules=_context.ProjectModules
            .Where(pm=>pm.ProjectId==p.Id)
            .Select(pm=>pm.Name)
            .ToList(),


           CreatedAt=p.CreatedAt

        }) .FirstOrDefaultAsync();     

       
        if(project==null)
        {
            return NotFound(new {message="Project not found."});
        }


        return Ok(project);
    }


    [HttpPost("projects/{projectId}/tech-stack")]
    public async Task<IActionResult> AddTechStack(int projectId, [FromBody] AddTechStackRequest request)
    {
        
        var projectExists=await _context.Projects.AnyAsync(p=>p.Id==projectId);
        if(!projectExists)
         return NotFound(new {message="Project not found."});

        if(request.TechNames==null || !request.TechNames.Any())
         return BadRequest(new {message="No tech stack names provided."});

        var techItems=request.TechNames.Select(name=>new ProjectTechStack
        {
            ProjectId=projectId,
            Name=name.Trim()
        });

        _context.ProjectTechStack.AddRange(techItems);
        await _context.SaveChangesAsync();
        return Ok(new{message="Tech stack added successfully.",count=request.TechNames.Count});
    }


    [HttpPost("projects/{projectId}/modules")]
    public async Task<IActionResult> AddModules(int projectId, [FromBody] AddModulesRequest request)

    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists)
            return NotFound(new { message = "Project not found." });

        if (request.ModulesNames == null || request.ModulesNames.Count == 0)
            return BadRequest(new { message = "No module names provided." });

        var moduleItems = request.ModulesNames.Select(name => new ProjectModule
        {
            ProjectId = projectId,
            Name = name.Trim()
        });

        _context.ProjectModules.AddRange(moduleItems);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Modules added successfully.", count = request.ModulesNames.Count });
    }


   [HttpPost("projects/{projectId}/members")]
   public async Task<IActionResult> AddMembers(int projectId, [FromBody] AddMembersRequest request)
    {
       var projectExists = await _context.Projects.AnyAsync(p => p.Id == projectId);
        if (!projectExists)
            return NotFound(new { message = "Project not found." });

        // 2. Validate request
        if (request.UserIds == null || request.UserIds.Count == 0)
            return BadRequest(new { message = "No user ids provided." });

        // 3. Create member mappings
        var members = request.UserIds.Select(userId => new ProjectMember
        {
            ProjectId = projectId,
            UserId = userId
        });

        // 4. Save
        _context.ProjectMembers.AddRange(members);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Members added successfully.", count = request.UserIds.Count }); 
    } 

    #region Onbpoarding Templates
    [HttpPost("onboarding/templates")]
    public async Task<IActionResult> CreateOnboardingTemplate([FromBody] CreateOnboardingTemplateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName))
        {
            return BadRequest(new{message="RoleName is required."});
        }

        var exists=await _context.OnboardingTemplates.AnyAsync(t=>t.RoleName.ToLower()==request.RoleName.ToLower());    

        if(exists)  
        {
            return BadRequest(new{message="Onboarding template for this role already exists."});
        }                   
        
        var template=new OnboardingTemplate
        {
            RoleName=request.RoleName,
            Description=request.Description,
            CreatedAt=DateTime.UtcNow
        };

        _context.OnboardingTemplates.Add(template);
        await _context.SaveChangesAsync();  
        return Ok(template);
    }

  #endregion

}