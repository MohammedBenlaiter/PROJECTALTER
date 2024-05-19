using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Dtos;

namespace PROJECTALTERAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeController : ControllerBase
    {
        private readonly AlterDbContext _context;

        public ExchangeController(AlterDbContext context)
        {
            _context = context;
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
            return Ok(dto);
        }
    }
}