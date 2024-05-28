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
        [HttpGet("GetUsersOffers/{userId}")]
        public IActionResult GetUsersOffers(long userId)
        {
            //var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var requests = _context.Requests.Where(o => o.UserId == userId).ToList();
            var offers = new List<Offer>();
            foreach (var request in requests)
            {
                offers.AddRange(_context.Offers.Where(o => o.RequestId == request.RequestId).ToList());
            }
            return Ok(offers);
        }
    }
}