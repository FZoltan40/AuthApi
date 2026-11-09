using AuthApi.Models;
using AuthApi.Models.DTOs;
using AuthApi.Services;
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
        private readonly PasswordHasher _passwordHasher;
        public UserController(AuthuserContext context, PasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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
            var (hash, salt) = _passwordHasher.HashPassword(newUserDto.Password!);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUserDto.Name,
                Email = newUserDto.Email,
                Password = hash,
                Salt = salt
            };


            if (user != null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return StatusCode(201, user);
            }

            return BadRequest();
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == loginUserDto.Name);


            if (user is null)
            {
                return NotFound( new { messgage = "Még nem rgisztrált" });
            }

            var isValid = _passwordHasher.Verify(loginUserDto.Password, user.Password, user.Salt);

            if (isValid)
            {
                return Ok(new { messgage = "Regisztrált" });
            }

            return BadRequest(new { messgage = "Hibás felhasznló név vagy jelszó" });
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
