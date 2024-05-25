using System;
using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using System.Security.Claims;
namespace PROJECTALTERAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AlterDbContext _context;

        public RequestController(AlterDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("creatRequest/{id}")]
        public async Task<IActionResult> AddRequest(long id, RequestDto requestDto)
        {
            //var userId = GetCurrentUser();
            var request = new Request
            {
                UserId = id,
                RequestTitle = requestDto.RequestTitle,
                RequestDescription = requestDto.RequestDescription,
                Deadline = requestDto.Deadline,
            };
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return Ok(request);
        }
        [HttpGet("getRequests")]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await _context.Requests.ToListAsync();
            if (requests == null)
            {
                return NotFound();
            }
            return Ok(requests);
        }
        [HttpGet("getRequest/{id}")]
        public async Task<IActionResult> GetRequest(long id)
        {
            //var userId = GetCurrentUser();
            var request = await _context.Requests.FindAsync(id);
            var rq = _context.Requests.Where(s => s.UserId == id);
            if (rq == null)
            {
                return NotFound();
            }
            return Ok(rq);
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
