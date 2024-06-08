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
using System.Linq;

namespace PROJECTALTERAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AlterDbContext _db;
        public UserController(AlterDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        [HttpPost("AddPicture")]
        public IActionResult AddPicture([FromBody] byte[] dto)
        {
            var id = GetCurrentUser();
            var user = _db.Users.SingleOrDefault(g => g.UserId == id.UserId);
            if (user == null)
            {
                return NotFound($"User does not exist");
            }
            user.Picture = dto;
            _db.SaveChanges();
            return Ok(user);
        }
        [HttpGet("getAllUser")] // Route at method level
        public IActionResult Get()
        {
            var users = _db.Users.ToList(); //  final

            return Ok(users); // Return successful response with users d 
        }
        [HttpGet("searchUser")]
        public IActionResult SearchUser([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var users = _db.Users.Where(u => u.FirstName.Contains(query) || u.LastName.Contains(query) || u.Username.Contains(query)).ToList();
            if (users.Count == 0)
            {
                return NotFound("No users found");
            }
            /* var userDtos = users.Select(u => new UserDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Username = u.Username,
                Password = u.Password
            }).ToList(); */
            return Ok(users);
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
                Picture = user.Picture
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
            if (_db.Users.Any(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists");
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
            if (dto != null)
            {
                var token = Generate(user);
                return Ok(new { token });
            }

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
                    Picture = user.Picture
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
                Picture = user.Picture,
                Skills = user.Skills.Select(s => new SkillDto
                {
                    SkillId = s.SkillId,
                    SkillName = s.SkillName
                }).ToList()
            };
            return Ok(User);
        }
        [HttpGet("GetUsersOffersAndExchanges")]
        public IActionResult GetUsersOffersAndExchanges()
        {
            var user = GetCurrentUser();
            var userOffers1 = _db.Users
            .Where(u => u.UserId == user.UserId)
            .Include(u => u.Requests)
            .ThenInclude(r => r.Offers)
            .Where(u => u.Requests.Any(r => r.Offers.Any(o => o.Status == "Accepted")))
            .SelectMany(u => u.Requests.SelectMany(r => r.Offers.Where(o => o.Status == "Accepted")))
            .Include(o => o.User)
            .Select(o => new
            {
                UserId = o.User.UserId,
                FirstName = o.User.FirstName,
                LastName = o.User.LastName,
                Password = o.User.Password,
                Username = o.User.Username
            })
            .ToList();

            var userOffers2 = _db.Users
            .Where(u => u.UserId == user.UserId)
            .Include(u => u.Offers)
            .ThenInclude(o => o.Request)
            .ThenInclude(r => r.User)
            .Where(u => u.Offers.Any(o => o.Status == "Accepted"))
            .SelectMany(u => u.Offers.Where(o => o.Status == "Accepted"))
            .Include(o => o.User)
            .Select(o => new
            {
                UserId = o.User.UserId,
                FirstName = o.User.FirstName,
                LastName = o.User.LastName,
                Password = o.User.Password,
                Username = o.User.Username
            })
            .ToList();

            var userDtoList1 = userOffers1.Select(o => new UserDto
            {
                UserId = o.UserId,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Password = o.Password,
                Username = o.Username
            }).ToList();

            var requestIdList = userOffers2.Select(o => o.UserId).ToList();
            var requests = _db.Requests.Where(r => requestIdList.Contains(r.RequestId)).ToList();

            var userIds = requests.Select(r => r.UserId).ToList();
            var users = _db.Users
            .Where(u => userIds.Contains(u.UserId))
            .Select(u => new UserDto
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Password = u.Password,
                Username = u.Username
            })
            .ToList();

            var exchanges = _db.Exchanges
            .Where(e => (e.ReciverId == user.UserId || e.SenderId == user.UserId) && e.Statues == "accepted")
            .ToList();

            var exchangeUsers = new List<UserDto>();
            var exchangeUserIds = new HashSet<long>(); // To keep track of unique user IDs

            foreach (var exchange in exchanges)
            {
                var sender = _db.Users.FirstOrDefault(u => u.UserId == exchange.SenderId);
                var recipient = _db.Users.FirstOrDefault(u => u.UserId == exchange.ReciverId);

                if (sender != null && sender.UserId != user.UserId && !exchangeUserIds.Contains(sender.UserId))
                {
                    exchangeUsers.Add(new UserDto
                    {
                        UserId = sender.UserId,
                        FirstName = sender.FirstName,
                        LastName = sender.LastName,
                        Password = sender.Password,
                        Username = sender.Username
                    });
                    exchangeUserIds.Add(sender.UserId);
                }
                if (recipient != null && recipient.UserId != user.UserId && !exchangeUserIds.Contains(recipient.UserId))
                {
                    exchangeUsers.Add(new UserDto
                    {
                        UserId = recipient.UserId,
                        FirstName = recipient.FirstName,
                        LastName = recipient.LastName,
                        Password = recipient.Password,
                        Username = recipient.Username
                    });
                    exchangeUserIds.Add(recipient.UserId);
                }
            }

            var combinedResult = userDtoList1.Concat(users).Concat(exchangeUsers).Distinct().ToList();

            return Ok(combinedResult);
        }
        /*         [HttpGet("GetUsersOffersAndExchanges/{userId}")]
                public IActionResult GetUsersOffersAndExchanges(long userId)
                {
                    var userOffers1 = _db.Users
                        .Where(u => u.UserId == userId)
                        .Include(u => u.Requests)
                        .ThenInclude(r => r.Offers)
                        .Where(u => u.Requests.Any(r => r.Offers.Any(o => o.Status == "Accepted")))
                        .SelectMany(u => u.Requests.SelectMany(r => r.Offers.Where(o => o.Status == "Accepted")))
                        .Include(o => o.User)
                        .Select(o => new
                        {
                            UserId = o.User.UserId,
                            FirstName = o.User.FirstName,
                            LastName = o.User.LastName,
                            Password = o.User.Password,
                            Username = o.User.Username
                        })
                        .ToList();

                    var userOffers2 = _db.Users
                        .Where(u => u.UserId == userId)
                        .Include(u => u.Offers)
                        .ThenInclude(o => o.Request)
                        .ThenInclude(r => r.User)
                        .Where(u => u.Offers.Any(o => o.Status == "Accepted"))
                        .SelectMany(u => u.Offers.Where(o => o.Status == "Accepted"))
                        .Include(o => o.User)
                        .Select(o => new
                        {
                            UserId = o.User.UserId,
                            FirstName = o.User.FirstName,
                            LastName = o.User.LastName,
                            Password = o.User.Password,
                            Username = o.User.Username
                        })
                        .ToList();

                    var userDtoList1 = userOffers1.Select(o => new UserDto
                    {
                        UserId = o.UserId,
                        FirstName = o.FirstName,
                        LastName = o.LastName,
                        Password = o.Password,
                        Username = o.Username
                    }).ToList();

                    var requestIdList = userOffers2.Select(o => o.UserId).ToList();
                    var requests = _db.Requests.Where(r => requestIdList.Contains(r.RequestId)).ToList();

                    var userIds = requests.Select(r => r.UserId).ToList();
                    var users = _db.Users
                        .Where(u => userIds.Contains(u.UserId))
                        .Select(u => new UserDto
                        {
                            UserId = u.UserId,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            Password = u.Password,
                            Username = u.Username
                        })
                        .ToList();

                    var exchanges = _db.Exchanges
                        .Where(e => (e.ReciverId == userId || e.SenderId == userId) && e.Statues == "accepted")
                        .ToList();

                    var exchangeUsers = new List<UserDto>();
                    var exchangeUserIds = new HashSet<long>(); // To keep track of unique user IDs

                    foreach (var exchange in exchanges)
                    {
                        var sender = _db.Users.FirstOrDefault(u => u.UserId == exchange.SenderId);
                        var recipient = _db.Users.FirstOrDefault(u => u.UserId == exchange.ReciverId);

                        if (sender != null && sender.UserId != userId && !exchangeUserIds.Contains(sender.UserId))
                        {
                            exchangeUsers.Add(new UserDto
                            {
                                UserId = sender.UserId,
                                FirstName = sender.FirstName,
                                LastName = sender.LastName,
                                Password = sender.Password,
                                Username = sender.Username
                            });
                            exchangeUserIds.Add(sender.UserId);
                        }
                        if (recipient != null && recipient.UserId != userId && !exchangeUserIds.Contains(recipient.UserId))
                        {
                            exchangeUsers.Add(new UserDto
                            {
                                UserId = recipient.UserId,
                                FirstName = recipient.FirstName,
                                LastName = recipient.LastName,
                                Password = recipient.Password,
                                Username = recipient.Username
                            });
                            exchangeUserIds.Add(recipient.UserId);
                        }
                    }

                    var combinedResult = userDtoList1.Concat(users).Concat(exchangeUsers).ToList();

                    return Ok(combinedResult);
                } */
        /*         [HttpGet("GetUsersOffersAndExchanges/{userId}")]
                public IActionResult GetUsersOffersAndExchanges(long userId)
                {
                    var userOffers1 = _db.Users
                        .Where(u => u.UserId == userId)
                        .Include(u => u.Requests)
                            .ThenInclude(r => r.Offers)
                        .Where(u => u.Requests.Any(r => r.Offers.Any(o => o.Status == "Accepted")))
                        .SelectMany(u => u.Requests.SelectMany(r => r.Offers.Where(o => o.Status == "Accepted")))
                        .Include(o => o.User)
                        .Select(o => new
                        {
                            UserId = o.User.UserId,
                            Username = o.User.Username,
                            OfferId = o.OfferId,
                            RequestId = o.RequestId,
                            OfferInfo = o.OfferInfo,
                            Deadline = o.Deadline,
                            Price = o.Price,
                            Status = o.Status
                        })
                        .ToList();

                    var userOffers2 = _db.Users
                        .Where(u => u.UserId == userId)
                        .Include(u => u.Offers)
                            .ThenInclude(o => o.Request)
                            .ThenInclude(r => r.User)
                        .Where(u => u.Offers.Any(o => o.Status == "Accepted"))
                        .SelectMany(u => u.Offers.Where(o => o.Status == "Accepted"))
                        .Include(o => o.User)
                        .Select(o => new
                        {
                            UserId = o.User.UserId,
                            Username = o.User.Username,
                            OfferId = o.OfferId,
                            RequestId = o.Request.RequestId,
                            OfferInfo = o.OfferInfo,
                            Deadline = o.Deadline,
                            Price = o.Price,[HttpGet("GetUsersOffersAndExchanges/{userId}")]
        public IActionResult GetUsersOffersAndExchanges(long userId)
        {
            var userOffers1 = _db.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Requests)
                .ThenInclude(r => r.Offers)
                .Where(u => u.Requests.Any(r => r.Offers.Any(o => o.Status == "Accepted")))
                .SelectMany(u => u.Requests.SelectMany(r => r.Offers.Where(o => o.Status == "Accepted")))
                .Include(o => o.User)
                .Select(o => new
                {
                    UserId = o.User.UserId,
                    FirstName = o.User.FirstName,
                    LastName = o.User.LastName,
                    Password = o.User.Password,
                    Username = o.User.Username
                })
                .ToList();

            var userOffers2 = _db.Users
                .Where(u => u.UserId == userId)
                .Include(u => u.Offers)
                .ThenInclude(o => o.Request)
                .ThenInclude(r => r.User)
                .Where(u => u.Offers.Any(o => o.Status == "Accepted"))
                .SelectMany(u => u.Offers.Where(o => o.Status == "Accepted"))
                .Include(o => o.User)
                .Select(o => new
                {
                    UserId = o.User.UserId,
                    FirstName = o.User.FirstName,
                    LastName = o.User.LastName,
                    Password = o.User.Password,
                    Username = o.User.Username
                })
                .ToList();

            var userDtoList1 = userOffers1.Select(o => new UserDto
            {
                UserId = o.UserId,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Password = o.Password,
                Username = o.Username
            }).ToList();

            var requestIdList = userOffers2.Select(o => o.UserId).ToList();
            var requests = _db.Requests.Where(r => requestIdList.Contains(r.RequestId)).ToList();

            var userIds = requests.Select(r => r.UserId).ToList();
            var users = _db.Users
                .Where(u => userIds.Contains(u.UserId))
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Password = u.Password,
                    Username = u.Username
                })
                .ToList();

            var exchanges = _db.Exchanges
                .Where(e => (e.ReciverId == userId || e.SenderId == userId) && e.Statues == "accepted")
                .ToList();

            var exchangeUsers = new List<UserDto>();
            var exchangeUserIds = new HashSet<long>(); // To keep track of unique user IDs

            foreach (var exchange in exchanges)
            {
                var sender = _db.Users.FirstOrDefault(u => u.UserId == exchange.SenderId);
                var recipient = _db.Users.FirstOrDefault(u => u.UserId == exchange.ReciverId);

                if (sender != null && sender.UserId != userId && !exchangeUserIds.Contains(sender.UserId))
                {
                    exchangeUsers.Add(new UserDto
                    {
                        UserId = sender.UserId,
                        FirstName = sender.FirstName,
                        LastName = sender.LastName,
                        Password = sender.Password,
                        Username = sender.Username
                    });
                    exchangeUserIds.Add(sender.UserId);
                }
                if (recipient != null && recipient.UserId != userId && !exchangeUserIds.Contains(recipient.UserId))
                {
                    exchangeUsers.Add(new UserDto
                    {
                        UserId = recipient.UserId,
                        FirstName = recipient.FirstName,
                        LastName = recipient.LastName,
                        Password = recipient.Password,
                        Username = recipient.Username
                    });
                    exchangeUserIds.Add(recipient.UserId);
                }
            }

            var combinedResult = userDtoList1.Concat(users).Concat(exchangeUsers).ToList();

            return Ok(combinedResult);
        }
                            Status = o.Status
                        })
                        .ToList();

                    var userDtoList1 = userOffers1.Select(o => new UserDto
                    {
                        UserId = o.UserId,
                        Username = o.Username
                    }).ToList();

                    var requestIdList = userOffers2.Select(o => o.RequestId).ToList();
                    var requests = _db.Requests.Where(r => requestIdList.Contains(r.RequestId)).ToList();

                    var userIds = requests.Select(r => r.UserId).ToList();
                    var users = _db.Users
                        .Where(u => userIds.Contains(u.UserId))
                        .Select(u => new
                        {
                            UserId = u.UserId,
                            Username = u.Username
                        })
                        .ToList();

                    var exchanges = _db.Exchanges
                        .Where(e => (e.ReciverId == userId || e.SenderId == userId) && e.Statues == "accepted")
                        .ToList();

                    var exchangeUsers = new List<UserDto>();
                    var exchangeUserIds = new HashSet<long>(); // To keep track of unique user IDs

                    foreach (var exchange in exchanges)
                    {
                        var sender = _db.Users.FirstOrDefault(u => u.UserId == exchange.SenderId);
                        var recipient = _db.Users.FirstOrDefault(u => u.UserId == exchange.ReciverId);

                        if (sender != null && sender.UserId != userId && !exchangeUserIds.Contains(sender.UserId))
                        {
                            exchangeUsers.Add(new UserDto
                            {
                                UserId = sender.UserId,
                                FirstName = sender.FirstName,
                                LastName = sender.LastName,
                                Username = sender.Username
                            });
                            exchangeUserIds.Add(sender.UserId);
                        }
                        if (recipient != null && recipient.UserId != userId && !exchangeUserIds.Contains(recipient.UserId))
                        {
                            exchangeUsers.Add(new UserDto
                            {
                                UserId = recipient.UserId,
                                FirstName = recipient.FirstName,
                                LastName = recipient.LastName,
                                Username = recipient.Username
                            });
                            exchangeUserIds.Add(recipient.UserId);
                        }
                    }

                    var combinedResult = new
                    {
                        UserOffers = userDtoList1,
                        //Users = users,
                        ExchangeUsers = exchangeUsers
                    };

                    return Ok(combinedResult);
                } */
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