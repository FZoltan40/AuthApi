using AuthApi.Models;
using AuthApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    [Route("userrole")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UserRoleController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] NewAssignRoleDto newAssignRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles)
                    .SingleOrDefaultAsync(u => u.Id == newAssignRoleDto.UserId);

                var role = await _context.Roles.FindAsync(newAssignRoleDto.RoleId);

                if (user is null || role is null)
                {
                    return NotFound(new { message = "Nincs user vagy role ilyen ID vel" });
                }

                var isValid = user.Roles.Any(r => r.Id == newAssignRoleDto.RoleId);

                if (isValid == true)
                {
                    return Conflict(new { message = "Ilyen szerepköre már van a usernek" });
                }

                user.Roles.Add(role);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("allUsersRoles")]
        public async Task<IActionResult> AllUsersRoles()
        {
            var usersRoles = await _context.Users
                    .Include(u => u.Roles)
                    .Select(u => new
                    {
                        u.Id,
                        u.Name,
                        Roles = u.Roles
                            .Select(r => r.Name).ToList(),
                    }).ToListAsync();

            return Ok(usersRoles);
        }

        [HttpPost("loginUser")]
        public async Task<IActionResult> LoginUser([FromQuery] Guid id)
        {
            var user = await _context.Users.Include(u => u.Roles)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    Roles = u.Roles
                        .Select(r => r.Name).ToList()
                })
                .SingleOrDefaultAsync(u => u.Id == id);

            return Ok(user);
               




        }
    }
}
