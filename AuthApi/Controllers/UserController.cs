using AuthApi.Models;
using AuthApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UserController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpGet("getAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            if (users != null)
            {
                return StatusCode(200, users);
            }

            return BadRequest();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(NewUserDto newUserDto)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUserDto.Name,
                Email = newUserDto.Email,
                Password = newUserDto.Password
            };


            if (user != null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return StatusCode(201, user);
            }

            return BadRequest();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromQuery]Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id == id);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, user);
            }

            return NotFound();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromQuery] Guid id, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                user.Name = updateUserDto.Name;
                user.Email = updateUserDto.Email;
                user.Password = updateUserDto.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, user);
            }

            return NotFound();
        }

    }
}
