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

namespace PROJECTALTERAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // ademmm
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

                if (verifyResult == PasswordVerificationResult.Success)
                {
                    return Ok("You are logged in successfully!");
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