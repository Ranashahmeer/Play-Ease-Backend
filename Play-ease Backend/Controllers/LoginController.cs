using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Play_ease_Backend.NewFolder.LoginModels;

namespace Play_ease_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (registerRequest == null || string.IsNullOrEmpty(registerRequest.Email) || string.IsNullOrEmpty(registerRequest.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            // Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerRequest.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email already registered." });
            }

            var user = new User
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                Phone = registerRequest.Phone,
                PasswordHash = registerRequest.Password, // ⚠️ Storing plain password
                RoleID = registerRequest.RoleID,
                Age = registerRequest.Age,
                Cnic = registerRequest.Cnic,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully", userId = user.UserID });
        }

        // ✅ Login user
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Email) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest(new { message = "Email and Password are required." });
            }

            var user = await _context.Users
                .Where(u => u.Email == loginRequest.Email && u.PasswordHash == loginRequest.Password)
                .Select(u => new { u.UserID, u.FullName, u.Email, u.RoleID })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new
            {
                message = "Login successful",
                userId = user.UserID,
                fullName = user.FullName,
                email = user.Email,
                roleId = user.RoleID
            });
        }


    }

    // ✅ DTOs
    public class RegisterRequestDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public int Age { get; set; }
        public string Cnic { get; set; }
    }

    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
