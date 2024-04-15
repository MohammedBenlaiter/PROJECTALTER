using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Models;

namespace PROJECTALTERAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AlterDbContext _db;

        public UserController(AlterDbContext db)
        {
            _db = db;
        }

        [HttpGet("getAllUser")] // Route at method level
        public async Task<IActionResult> Get()
        {
            var users = await _db.Users.ToListAsync(); // Asynchronous retrieval

            return Ok(users); // Return successful response with users
        }
    }
}

