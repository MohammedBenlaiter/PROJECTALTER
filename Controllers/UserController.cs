using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using PROJECTALTERAPI.Models;
using PROJECTALTERAPI.Services;
using Microsoft.AspNetCore.Identity;

namespace PROJECTALTERAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AlterDbContext _db;

        public UserController(AlterDbContext db)
        {
            _db = db;
        }

        [HttpGet("getAllUser")] // Route at method level
        public IActionResult Get()
        {
            var users = _db.Users.ToList(); //  final

            return Ok(users); // Return successful response with users d 
        }
        [HttpPost("createUser")]
        public IActionResult Create(UserDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = dto.Password,
            };
            _db.Add(user);
            _db.SaveChanges();
            return Ok(dto);
        }
        [HttpPut("updateUser/{id}")]
        public IActionResult Update(int id, UserDto dto)
        {
            var user = _db.Users.SingleOrDefault(g => g.UserId == id);
            if (user == null)
            {
                return NotFound($"User {id} does not exist");
            }
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Username = dto.Username;
            user.Password = dto.Password;
            _db.SaveChanges();
            return Ok(user);
        }
        [HttpDelete("deleteUser/{id}")]
        public IActionResult Delete(int id)
        {
            var user = _db.Users.SingleOrDefault(g => g.UserId == id);
            if (user == null)
            {
                return NotFound($"User {id} does not exist");
            }
            _db.Remove(user);
            _db.SaveChanges();
            return Ok("the user " + id + " is deleted");
        }
        [HttpPost("login")]
        public IActionResult Login(LoginDto login)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == login.Username && u.Password == login.Password);
            //var user = _db.Users.Where(u => u.Username == login.Username && u.Password == login.Password).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized("amchi trawe7");
            }
            else
            {
                return Ok("you are th real user");
            }
        }
        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto dto)
        {
            var UsernameCheck = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (UsernameCheck != null)
            {
                return BadRequest("Username is taken");
            }
            var EmailCheck = _db.Emails.FirstOrDefault(e => e.EmailAdresse == dto.Email);
            if (EmailCheck != null)
            {
                return BadRequest("Email is taken");
            }
            var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = passwordHasher.HashPassword(null!, dto.Password),
            };
            _db.Add(user);
            _db.SaveChanges();
            var email = new Email
            {
                UserId = user.UserId,
                EmailAdresse = dto.Email
            };
            _db.Emails.Add(email);
            _db.SaveChanges();
            return Ok(dto);
        }
    }
}