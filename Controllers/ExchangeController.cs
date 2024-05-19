using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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


        public ExchangeController(AlterDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("CreateExchange/{id_rec}/{id_send}/{id_s_rec}/{id_s_send}")]
        public IActionResult CreateExchange(long id_rec, long id_s_rec, long id_send, long id_s_send)
        {
            var dto = new ExchangeDto
            {
                ReciverId = id_rec,
                SenderId = id_send,
                SkillSendId = id_s_send,
                SkillReceiveId = id_s_rec,
                Statues = "sended"
            };
            var exchange = new Exchange
            {
                ReciverId = dto.ReciverId,
                SenderId = dto.SenderId,
                SkillSendId = dto.SkillSendId,
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
    }

    internal class NotificationDto
    {
        public long ReciverId { get; set; }
        public string Message { get; set; } = default!;
    }
}