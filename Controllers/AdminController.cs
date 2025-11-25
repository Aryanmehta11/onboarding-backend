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
    public async Task<IActionResult> GetRoles()=>Ok(await _context.Roles.ToListAsync());


    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
    }

    [HttpGet("sections")]
    public async Task<IActionResult> GetSections()=>Ok( await _context.Sections.ToListAsync());

    [HttpPost("sections")]
    public async Task<IActionResult> CreateSection([FromBody] Section section)
    {
        _context.Sections.Add(section);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSections), new { id = section.Id }, section);
    }

    [HttpGet("rolepermissions/{roleId}")]
    public async Task<IActionResult> GetRolePermissions(int roleId) {
        var perms = await _context.RolePermissions.Where(rp => rp.RoleId == roleId).ToListAsync();
        return Ok(perms);
    }
    [HttpPut("rolepermissions/{roleId}")]
    public async Task<IActionResult> UpdatePermissions(int roleId,[FromBody] List<RolePermission> newPerms)
    {
         // remove existing for role
        var existing = _context.RolePermissions.Where(rp => rp.RoleId == roleId);
        _context.RolePermissions.RemoveRange(existing);
        await _context.SaveChangesAsync();

        // set roleId on newPerms to ensure correctness
        foreach(var p in newPerms) {
            p.RoleId = roleId;
            _context.RolePermissions.Add(p);
        }
        await _context.SaveChangesAsync();
        return NoContent();
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