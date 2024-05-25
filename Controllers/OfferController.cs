using System;
using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using System.Security.Claims;

namespace PROJECTALTERAPI;
[Route("api/[controller]")]
[ApiController]
public class OfferController : ControllerBase
{
    /*  private IConfiguration _configuration;
     private readonly AlterDbContext _context;

     public OfferController(AlterDbContext context , IConfiguration configuration)
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
     [HttpPost("CreateOffer/{id}")]
     public IActionResult CreateSkillListining(OfferDto offerDto , long id)
     {
         var userId = GetCurrentUser();
         var offer = new Offer
         {
             UserId = userId.UserId,
             RequestId = offerDto.RequestId,
             OfferInfo = offerDto.OfferInfo,
             Deadline = offerDto.Deadline,
             Price = offerDto.Price
         };
         // Add the new Skill to the context and save changes
         _context.Offers.Add(offer);
         _context.SaveChanges();
         offerDto.OfferId = offer.OfferId;
         offerDto.UserId = offer.UserId;
         // Return the created Skill object
         return Ok(offerDto);
     }
     [HttpGet("GetOffers")]
     public async Task<IActionResult> GetOffer()
     {
         var offer = await _context.Offers.ToListAsync();
         if (offer == null)
         {
             return NotFound();
         }
         return Ok(offer);
     } */
}