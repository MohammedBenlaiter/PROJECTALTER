using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
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
        public IActionResult Get()
        {
            var users = _db.Users.ToList(); //  final

            return Ok(users); // Return successful response with users d 
        }
        [HttpPost("createUser")]
        public IActionResult Create(CreateUserDto dto)
        {
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = dto.Username,
                Password = dto.Password,
            };
            _db.Add(user);
            _db.SaveChanges();
            return Ok(dto);
        }
    }
}

