using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        [HttpPost("send")]
        public async Task<IActionResult> PostMessage(MessageDto message)
        {
            try
            {
                var newMessage = new Message
                {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
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
    }
}

