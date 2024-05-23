using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PROJECTALTERAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificationController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AlterDbContext _context;

        public CertificationController(AlterDbContext context, IConfiguration configuration)
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
        [HttpPost("addCertification")]
        public async Task<IActionResult> AddCertification(CertificationDto certificateDto)
        {
            var userId = GetCurrentUser();
            Certification certificate =  new Certification {
                CertificationPicture = certificateDto.CertificationPicture,
                UserId = userId.UserId,
            };
            _context.Certifications.Add(certificate);
            await _context.SaveChangesAsync();
            return Ok(certificate);
        }
    }
}