using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Hubs;
using System.Threading.Tasks; // Add this line
namespace PROJECTALTERAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly AlterDbContext _context;

        public FeedbackController(AlterDbContext context)
        {
            _context = context;
        }
        [HttpPost("sendFeedback/{userId}")]
        public async Task<IActionResult> SendFeedback(FeedbackDto feedback, long userId)
        {
            var fb = new Feedback
            {
                UserId = userId,
                Description = feedback.Description,
            };
            _context.Feedbacks.Add(fb);
            await _context.SaveChangesAsync();
            return Ok(feedback);
        }
        [HttpGet("getUserFeedbacks/{userId}")]
        public async Task<IActionResult> GetFeedbacks(long userId)
        {
            var feedbacks = await _context.Feedbacks.Where(f => f.UserId == userId).ToListAsync();
            return Ok(feedbacks);
        }
        [HttpGet("getOwnFeedbacks")]
        public async Task<IActionResult> GetOwnFeedbacks()
        {
            var user = GetCurrentUser();
            var feedbacks = await _context.Feedbacks.Where(f => f.UserId == user.UserId).ToListAsync();
            return Ok(feedbacks);
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
}