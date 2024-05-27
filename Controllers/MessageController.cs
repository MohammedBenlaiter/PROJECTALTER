using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Hubs;
namespace PROJECTALTERAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AlterDbContext _db;
        public MessageController(AlterDbContext db, IHubContext<ChatHub> hubContext)
        {
            _db = db;

            _hubContext = hubContext;
        }
        [HttpPost("send/{id_sender}/{id_receiver}")]
        public async Task<IActionResult> PostMessage(MessageDto message, long id_receiver)
        {
            var user = GetCurrentUser();
            try
            {
                var newMessage = new Message
                {
                    SenderId = user.UserId,
                    ReceiverId = id_receiver,
                    Content = message.Content
                };

                _db.Messages.Add(newMessage);
                await _db.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("ReceiveMessage", newMessage.SenderId, newMessage.ReceiverId, newMessage.Content);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetAllMessages/{id_sender}/{id_receiver}")]
        public async Task<IActionResult> GetAllMessages(long id_receiver)
        {
            var user = GetCurrentUser();
            try
            {
                var messages = await _db.Messages.Where(m => (m.SenderId == user.UserId && m.ReceiverId == id_receiver) || (m.SenderId == id_receiver && m.ReceiverId == user.UserId)).ToListAsync();

                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

