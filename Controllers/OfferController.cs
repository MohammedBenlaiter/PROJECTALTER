using System;
using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace PROJECTALTERAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AlterDbContext _context;

        public OfferController(AlterDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
        [HttpGet("GetUsersWhoOffering/{requestId}")]
        public async Task<IActionResult> GetUsersWhoOffering(long requestId)
        {
            var usersOffering = await _context.Offers
                .Where(o => o.RequestId == requestId)
                .Select(o => new UserDto
                {
                    UserId = o.UserId,
                    Username = o.User.Username
                })
                .ToListAsync();
            return Ok(usersOffering);
        }
        [HttpPost("CreateOffer")]
        public IActionResult CreateSkillListining(OfferDto offerDto)
        {
            var userId = GetCurrentUser();
            var offer = new Offer
            {
                UserId = userId.UserId,
                RequestId = offerDto.RequestId,
                OfferInfo = offerDto.OfferInfo,
                Deadline = offerDto.Deadline,
                Price = offerDto.Price,
                Status = "Sended"
            };
            // Add the new Skill to the context and save changes
            _context.Offers.Add(offer);
            _context.SaveChanges();
            offerDto.OfferId = offer.OfferId;
            offerDto.UserId = offer.UserId;
            // Return the created Skill object
            return Ok(offerDto);
        }
        [HttpGet("GetOfferByUser/{userId}")]
        public async Task<IActionResult> GetOfferByUser(long userId)
        {
            var offers = await _context.Offers.Where(o => o.UserId == userId).ToListAsync();
            if (offers == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(offers);
            }
        }
        [HttpGet("GetOffers/{receiver_id}")]
        public async Task<IActionResult> GetOffers(long receiver_id)
        {
            var offers = await _context.Offers.Where(o => o.RequestId == receiver_id).ToListAsync();
            if (offers == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(offers);
            }
        }
        [HttpPost("AcceptOffer/{id}")]
        public async Task<IActionResult> AcceptOffer(long id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }
            offer.Status = "Accepted";
            _context.SaveChanges();
            return Ok(offer);
        }
        [HttpPost("RejectOffer/{id}")]
        public async Task<IActionResult> RejectOffer(long id)
        {
            var offer = await _context.Offers.FindAsync(id);
            if (offer == null)
            {
                return NotFound();
            }
            offer.Status = "Rejected";
            _context.SaveChanges();
            return Ok(offer);
        }
        /*         [HttpGet("GetUsersOffers/{userId}")]
                public IActionResult GetUsersOffers(long userId)
                {
                    var userOffers = _context.Users
                        .Where(u => u.UserId == userId)
                        .Include(u => u.Requests)
                            .ThenInclude(r => r.Offers)
                        .Where(u => u.Requests.Any(r => r.Offers.Any(o => o.Status == "Accepted")))
                        .Select(u => new
                        {
                            UserId = u.UserId,
                            Username = u.Username,
                            Offers = u.Requests.SelectMany(r => r.Offers.Where(o => o.Status == "Accepted"))
                        })
                        .ToList();

                    return Ok(userOffers);
                } */
        [HttpGet("GetUsersOffers/{userId}")]
        public IActionResult GetUsersOffers(long userId)
        {
            var userOffers = _context.Users
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
            var userDtoList = userOffers.Select(o => new UserDto
            {
                UserId = o.UserId,
                Username = o.Username
            }).ToList();

            return Ok(userDtoList);
        }
        [HttpGet("GetUsersOffers2/{userId}")]
        public IActionResult GetUsersOffers2(long userId)
        {
            var userOffers = _context.Users
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
                Price = o.Price,
                Status = o.Status
            })
            .ToList();
            /*             var userDtoList = userOffers.Select(o => new UserDto
                        {
                            UserId = o.UserId,
                            Username = o.Username
                        }).ToList(); */
            var requestIdList = userOffers.Select(o => o.RequestId).ToList();
            var requests = _context.Requests.Where(r => requestIdList.Contains(r.RequestId)).ToList();

            var userIds = requests.Select(r => r.UserId).ToList();
            var users = _context.Users
                .Where(u => userIds.Contains(u.UserId))
                .Select(u => new
                {
                    UserId = u.UserId,
                    Username = u.Username
                })
                .ToList();
            return Ok(users);
        }
    }
}