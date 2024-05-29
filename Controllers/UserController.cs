using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using PROJECTALTERAPI.Models;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.CodeDom.Compiler;

namespace PROJECTALTERAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // ademmm
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AlterDbContext _db;
        public UserController(AlterDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [HttpGet("getAllUser")] // Route at method level
        public IActionResult Get()
        {
            var users = _db.Users.ToList(); //  final

            return Ok(users); // Return successful response with users d 
        }
        [HttpGet("getUser")]
        public IActionResult GetUser()
        {
            var id = GetCurrentUser();
            var user = _db.Users.SingleOrDefault(g => g.UserId == id.UserId);
            if (user == null)
            {
                return NotFound($"User does not exist");
            }
            var user2 = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Password = user.Password,
            };
            return Ok(user2);
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

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto dto)
        {
            /* if (_db.Users.Any(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists");
            } */
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

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login2([FromBody] LoginDto dto)
        {
            var tokenDto = new TokenDto();
            var user = Auth(dto);
            if (user != null)
            {
                var token = Generate(user);
                tokenDto.Token = token;
                return Ok(tokenDto);
            }
            return NotFound("Invalid username or password");
        }
        private string Generate(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
             _configuration["Jwt:Audience"],
              claims,
               expires: DateTime.Now.AddHours(30),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private User Auth(LoginDto dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            var passwordHasher = new PasswordHasher<User>();
            if (user != null)
            {
                if (passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password) == PasswordVerificationResult.Success)
                {
                    return user;
                }
            }
            return null!;
        }
        private User GetCurrentUser()
        {
            var Identity = HttpContext.User.Identity as ClaimsIdentity;
            if (Identity != null)
            {
                var userClaim = Identity.Claims;
                return new User
                {
                    UserId = Convert.ToInt64(userClaim.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value),
                    Username = userClaim.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty
                };
            }
            return null!; // Add a return statement for the case when Identity is null
        }

        [HttpGet("getCurrentUser")]
        [Authorize]
        public IActionResult getCurrentEndpoint()
        {
            var currentUser = GetCurrentUser();
            return Ok(currentUser.UserId);
        }
        [HttpGet("getUserBySkillId/{id}")]
        public IActionResult GetUserBySkillId(long id)
        {
            var user = _db.Users.FirstOrDefault(u => u.Skills.Any(s => s.SkillId == id));
            if (user != null)
            {
                UserDto dto = new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.Username,
                    Password = "mat7awesch tafhem",
                };
                return Ok(dto);
            }
            return NotFound("User not found");
        }
        [HttpGet("GetUserById/{user_id}")]
        public IActionResult GetUserById(long user_id)
        {
            var user = _db.Users.Include(u => u.Skills).FirstOrDefault(u => u.UserId == user_id);
            if (user == null)
            {
                return NotFound($"User {user_id} does not exist");
            }
            var User = new UserNotificationDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Password = user.Password,
                Skills = user.Skills.Select(s => new SkillDto
                {
                    SkillId = s.SkillId,
                    SkillName = s.SkillName
                }).ToList()
            };
            return Ok(User);
        }

        [HttpPost("checkEmail")]
        public IActionResult CheckEmailAvailability(EmailDto dto)
        {
            var EmailCheck = _db.Emails.FirstOrDefault(e => e.EmailAdresse == dto.Email);
            if (EmailCheck != null)
            {
                return BadRequest("Email is taken");
            }
            return Ok(new { isEmailAvailable = true });
        }

        [HttpPost("checkUsername")]
        public IActionResult CheckUsernameAvalability(UsernameDto dto)
        {
            var UsernameCheck = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (UsernameCheck != null)
            {
                return BadRequest("Username is taken");
            }
            return Ok(new { isUsernameAvailable = true });
        }

        [HttpPost("sendEmail")]
        public IActionResult SendEmail([FromBody] SendEmailDto request)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Tyshawn Murazik", "tyshawn.murazik@ethereal.email"));
                email.To.Add(new MailboxAddress(request.Name, request.Email));
                email.Subject = request.Subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = request.Message };
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.ethereal.email", 587, false);
                    client.Authenticate("tyshawn.murazik@ethereal.email", "SEBxy8Rsdk6qEDWtDH");
                    client.Send(email);
                    client.Disconnect(true);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}