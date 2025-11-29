using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onboardingAPI.Data;
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

}