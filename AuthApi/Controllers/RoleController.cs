using AuthApi.Models;
using AuthApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace AuthApi.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public RoleController(AuthuserContext context)
        {
            _context = context;
        }


        [HttpGet("getAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            var roles = await _context.Roles.ToListAsync();

            if (roles != null || roles.Count > 0) 
            {
                return StatusCode(200, roles);
            }

            return StatusCode(400, roles);
        }

        [HttpPost("newRole")]
        public async Task<IActionResult> NewRole(NewRoleDto newRoleDto)
        {
            try
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = newRoleDto.Name,
                    RegTime = DateTime.Now
                };

                if (role != null)
                {
                    await _context.Roles.AddAsync(role);
                    await _context.SaveChangesAsync();

                    return StatusCode(201, role);
                }

                return StatusCode(404, role);
            }
            catch (Exception e)
            {

                return StatusCode(400, new { message = "Ilyen szerepkör mát létezik!"});
            }
        }


        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole([FromQuery] Guid id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return StatusCode(200, role);
            }

            return StatusCode(404, role);
        }

        [HttpPut("updateRole")]
        public async Task<IActionResult> UpdateRole([FromQuery] Guid id, UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);

            if (role != null)
            {
                role.Name = updateRoleDto.Name;

                _context.Roles.Update(role);
                await _context.SaveChangesAsync();

                return StatusCode(200, role);
            }

            return StatusCode(404, role);
        }
    }
}
