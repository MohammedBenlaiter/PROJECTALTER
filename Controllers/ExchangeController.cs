using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using PROJECTALTERAPI.Dtos;
using PROJECTALTERAPI.Hubs;

namespace PROJECTALTERAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly AlterDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;
        private IConfiguration _configuration;


        public ExchangeController(AlterDbContext context, IHubContext<ChatHub> hubContext, IConfiguration configuration)
        {
            _context = context;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        [HttpPost("CreateExchange/{id_rec}/{id_s_rec}")]
        public IActionResult CreateExchange(long id_rec, long id_s_rec)
        {
            var user = GetCurrentUser();
            var dto = new ExchangeDto
            {
                ReciverId = id_rec,
                SenderId = user.UserId,
                SkillReceiveId = id_s_rec,
                Statues = "sended"
            };
            var exchange = new Exchange
            {
                ReciverId = dto.ReciverId,
                SenderId = dto.SenderId,
                SkillReceiveId = dto.SkillReceiveId,
                Statues = "sended"
            };
            _context.Exchanges.Add(exchange);
            _context.SaveChanges();

            // Send notification to id_rec users with SignalR
            var notification = new NotificationDto
            {
                ReciverId = id_rec,
                Message = "You have a new exchange request"
            };
            _hubContext.Clients.User(id_rec.ToString()).SendAsync("ReceiveNotification", notification);

            return Ok(dto);
        }
        [HttpGet("ExchangeNotification")]
        public IActionResult ExchangeNotification()
        {
            var user = GetCurrentUser();
            var exchanges = _context.Exchanges.Where(e => e.ReciverId == user.UserId && e.Statues == "sended").ToList();
            var ExchangeNotificationDto = new List<ExchangeNotificationDto>();
            foreach (var exchange in exchanges)
            {
                var sender = _context.Users.FirstOrDefault(u => u.UserId == exchange.SenderId);
                var skill = _context.Skills.FirstOrDefault(s => s.SkillId == exchange.SkillReceiveId);
                ExchangeNotificationDto.Add(new ExchangeNotificationDto
                {
                    ExchangeId = exchange.ExchangeId,
                    ReciverId = exchange.ReciverId,
                    SenderId = exchange.SenderId,
                    SkillReceiveId = exchange.SkillReceiveId,
                    Statues = exchange.Statues,
                    senderFirstName = sender.FirstName,
                    senderLastName = sender.LastName,
                    senderUserName = sender.Username
                });
            }
            return Ok(ExchangeNotificationDto);
        }
        [HttpPost("AcceptExchange/{exchangeId}")]
        public IActionResult AcceptExchange(long exchangeId)
        {
            var user = GetCurrentUser();
            var exchange = _context.Exchanges.FirstOrDefault(e => e.ExchangeId == exchangeId);
            if (exchange != null)
            {
                exchange.Statues = "accepted";
                _context.SaveChanges();
                return Ok(exchange);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("RefuseExchange/{exchangeId}")]
        public IActionResult RefuseExchange(long exchangeId)
        {
            var user = GetCurrentUser();
            var exchange = _context.Exchanges.FirstOrDefault(e => e.ExchangeId == exchangeId);
            if (exchange != null)
            {
                exchange.Statues = "refused";
                _context.SaveChanges();
                return Ok(exchange);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("GetUsersExchanges/{id}")]
        public IActionResult GetUsersExchanges(long id)
        {
            var exchanges = _context.Exchanges
                .Where(e => (e.ReciverId == id || e.SenderId == id) && e.Statues == "accepted")
                .ToList();

            var users = new List<UserDto>();

            foreach (var exchange in exchanges)
            {
                var sender = _context.Users.FirstOrDefault(u => u.UserId == exchange.SenderId);
                var recipient = _context.Users.FirstOrDefault(u => u.UserId == exchange.ReciverId);

                if (sender != null && sender.UserId != id) // Exclude the user with the same ID as the parameter
                {
                    users.Add(new UserDto
                    {
                        FirstName = sender.FirstName,
                        LastName = sender.LastName,
                        Username = sender.Username
                    });
                }
                if (recipient != null && recipient.UserId != id) // Exclude the user with the same ID as the parameter
                {
                    users.Add(new UserDto
                    {
                        FirstName = recipient.FirstName,
                        LastName = recipient.LastName,
                        Username = recipient.Username
                    });
                }
            }
            return Ok(users);
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
            var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
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
    }

    internal class NotificationDto
    {
        public long ReciverId { get; set; }
        public string Message { get; set; } = default!;
    }
}