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
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == login.Username);
                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }
                var passwordHasher = new PasswordHasher<User>();
                var verifyResult = passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);
                string token = CreateToken(user);
                if (verifyResult == PasswordVerificationResult.Success)
                {
                    // return Ok("You are logged in successfully!");
                    return Ok(token);
                }
                else
                {
                    return Unauthorized("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
                return BadRequest("An error occurred during login.");
            }
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegisterDto dto)
        {
            //var passwordHasher = new PasswordHasher<User>();
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = dto.Password,
                //Password = passwordHasher.HashPassword(null!, dto.Password),
            };
            _db.Add(user);
            _db.SaveChanges();
            /*             var email = new Email
                        {
                            UserId = user.UserId,
                            //EmailAdresse = dto.Email
                        };
                        _db.Emails.Add(email);
                        _db.SaveChanges(); */
            return Ok(dto);
        }
        [HttpPost("register2")]
        public IActionResult Register2(UserRegisterDto dto)
        {
            // Check if the username already exists
            if (_db.Users.Any(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = passwordHash,
            };
            _db.Add(user);
            _db.SaveChanges();
            return Ok("register successfully");
        }
        [AllowAnonymous]
        [HttpPost("login2")]
        public IActionResult Login2([FromBody] LoginDto dto)
        {
            var user = Auth(dto);
            if (user != null)
            {
                var token = Generate(user);
                return Ok(token);
            }
            return NotFound("Invalid username or password");
            /*             var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
                        if (user == null)
                        {
                            return BadRequest("Invalid username");
                        } */
            /*             if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                        {
                            return BadRequest("Invalid password");
                        } */
            /*             if (dto.Password != user.Password)
                        {
                            return BadRequest("Invalid password");
                        }
                        string token = CreateToken(user);
                        return Ok(dto); */
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
               expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private User Auth(LoginDto dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user != null)
            {
                /* if (BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
                {
                    return user;
                } */
                if (dto.Password == user.Password)
                {
                    return user;
                }
            }
            return null;
        }
        private string CreateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("your_secret_key_here"); // Replace with your secret key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    // Add other claims as needed
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Set token expiration as needed
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"], // Add this line
                Audience = _configuration["Jwt:Audience"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
        public IActionResult SendEmail([FromBody] EmailDto request)
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
        /*  email.From.Add(MailboxAddress.Parse("andreane.cassin@ethereal.email"));
         email.To.Add(MailboxAddress.Parse("andreane.cassin@ethereal.email"));
         email.Subject = "YOUR VERIFICATION CODE";
         email.Body = new TextPart(TextFormat.Html) { Text = body };
         using var smtp = new SmtpClient();
         smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
         smtp.Authenticate("andreane.cassin@ethereal.email", "2xp83cH1gGWg3Y67NF");
         smtp.Send(email);
         smtp.Disconnect(true); */
    }
}