using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Services;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        //  gets the database context through Dependency Injection
        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Checks if the email is already in the database
            AppUser existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email is already registered." });
            }

            // Find the requested role in the Roles table
            Role selectedRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role);
            if (selectedRole == null)
            {
                return BadRequest(new { message = "Role must be either 'Organiser' or 'Participant'." });
            }

            // new user and hash the password
            AppUser newUser = new AppUser
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = PasswordService.HashPassword(request.Password),
                RoleID = selectedRole.RoleID,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Returns the created user details using the simple DTO
            AuthResponse response = new AuthResponse
            {
                UserId = newUser.UserID,
                FullName = newUser.FullName,
                Email = newUser.Email,
                Role = selectedRole.RoleName
            };

            return StatusCode(StatusCodes.Status201Created, response);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Finds user by email and include their Role
            AppUser user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Checks if user exists and password matches the hash
            if (user == null || !PasswordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            // Stores user details in the server session
            HttpContext.Session.SetInt32("UserId", user.UserID);
            HttpContext.Session.SetString("UserRole", user.Role.RoleName);

            AuthResponse response = new AuthResponse
            {
                UserId = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.RoleName
            };

            return Ok(response);
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Clears all data stored in the current session
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }
    }
}