using System;
using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;
using System.Security.Claims;

namespace PROJECTALTERAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly AlterDbContext _context;

        public SkillController(AlterDbContext context, IConfiguration configuration)
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
        [HttpPost("CreateSkillListining")]
        public IActionResult CreateSkillListining(SkillDto skillDto)
        {
            var userId = GetCurrentUser();
            // Check if the provided ID exists in the database
            var user = _context.Users.Find(userId.UserId);
            if (user == null)
            {
                return NotFound();
            }
            // Create a new Skill object based on the provided SkillDto
            var skill = new Skill
            {
                UserId = userId.UserId,
                SkillName = skillDto.SkillName,
                SkillDescription = skillDto.SkillDescription,
                YearsOfExperience = skillDto.YearsOfExperience,
                SkillLevel = skillDto.SkillLevel,
                SkillType = skillDto.SkillType
            };
            // Add the new Skill to the context and save changes
            _context.Skills.Add(skill);
            _context.SaveChanges();
            skillDto.SkillId = skill.SkillId;
            skillDto.UserId = skill.UserId;
            var language = new Language
            {
                SkillId = skillDto.SkillId,
                LanguageName = skillDto?.LanguageName ?? string.Empty
            };
            _context.Languages.Add(language);
            _context.SaveChanges();
            var link = new Link
            {
                SkillId = skillDto.SkillId,
                LinkInformation = skillDto?.LinkInformation ?? string.Empty
            };
            _context.Links.Add(link);
            _context.SaveChanges();
            // Return the created Skill object
            return Ok(skillDto);
        }
        [HttpPost("CreateSkillListining2")]
        public IActionResult CreateSkillListining2(SkillDto skillDto)
        {
            var userId = GetCurrentUser();
            // Check if the provided ID exists in the database
            var user = _context.Users.Find(userId.UserId);
            if (user == null)
            {
                return NotFound();
            }
            // Create a new Skill object based on the provided SkillDto
            var skill = new Skill
            {
                UserId = userId.UserId,
                SkillName = skillDto.SkillName,
                SkillDescription = skillDto.SkillDescription,
                YearsOfExperience = skillDto.YearsOfExperience,
                SkillLevel = skillDto.SkillLevel,
                SkillType = skillDto.SkillType
            };
            // Add the new Skill to the context and save changes
            _context.Skills.Add(skill);
            _context.SaveChanges();
            skillDto.SkillId = skill.SkillId;
            skillDto.UserId = skill.UserId;
            // Return the created Skill object
            return Ok(skillDto);
        }
        [HttpPost("AddSkillLanguage/{id}")]
        public IActionResult AddSkillLanguage(long id, SkillLanguageDto dto)
        {
            var skill = _context.Skills.Find(id);
            if (skill == null)
            {
                return NotFound();
            }
            var language = new Language
            {
                SkillId = id,
                LanguageName = dto.LanguageName
            };
            _context.Languages.Add(language);
            _context.SaveChanges();
            dto.LanguageId = language.LanguageId;
            dto.SkillId = skill.SkillId;
            return Ok(dto);
        }
        [HttpPost("AddSkillLinks/{id}")]
        public IActionResult AddSkillLinks(long id, LinkDto dto)
        {
            var skill = _context.Skills.Find(id);
            if (skill == null)
            {
                return NotFound();
            }
            var link = new Link
            {
                SkillId = id,
                LinkInformation = dto.LinkInformation
            };
            _context.Links.Add(link);
            _context.SaveChanges();
            dto.LinksId = link.LinksId;
            dto.SkillId = skill.SkillId;
            return Ok(dto);
        }
        [HttpGet("SkillSearch/{Id}")]
        public IActionResult SearchSkill(SearchDto query, int Id)
        {
            var user = _context.Users.Find(Id);
            if (user == null)
            {
                return NotFound();
            }
            var userWishlist = _context.Wishlists.Where(k => k.UserId == Id).Select(k => k.WishlistName).ToList();
            var skills = _context.Skills.Where(s => (s.SkillName.Contains(query.Query) || s.SkillDescription.Contains(query.Query) || s.SkillLevel.Contains(query.Query) || s.SkillType.Contains(query.Query)) && userWishlist.Contains(s.SkillName));
            if (!skills.Any())
            {
                return NotFound();
            }
            var dto = skills.Select(s => new SkillSearchDto
            {
                UserId = s.UserId,
                SkillName = s.SkillName,
                SkillDescription = s.SkillDescription,
                SkillLevel = s.SkillLevel,
                SkillType = s.SkillType,
                wishlists = s.User.Wishlists,
            });
            return Ok(dto);
        }
        [HttpGet("SearchSkillByName")]
        public IActionResult SearchSkillByName([FromQuery] string skillName)
        {
            var skills = _context.Skills.Where(s => s.SkillName.Contains(skillName));
            if (!skills.Any())
            {
                return NotFound();
            }
            //var users = _context.Users.Where(u => skills.Select(s => s.UserId).Contains(u.UserId));
            var users = _context.Users;
            var dto = skills.Select(s => new SkillSearchDto
            {
                UserId = s.UserId,
                //Username = users.Where(u => u.UserId == u.UserId).FirstOrDefault().Username,
                Username = users.Where(u => u.UserId == s.UserId).Select(u => u.Username).FirstOrDefault()!,
                SkillName = s.SkillName,
                SkillDescription = s.SkillDescription,
                SkillLevel = s.SkillLevel,
                SkillType = s.SkillType,
                wishlists = s.User.Wishlists,
            });
            return Ok(dto);
        }
        [HttpGet("getSkill")]
        public async Task<ActionResult<Skill>> GetSkill()
        {
            var userId = GetCurrentUser();
            var skill = await _context.Skills.FindAsync(userId.UserId);

            var skills = _context.Skills.Where(s => s.UserId == userId.UserId);

            if (skills == null)
            {
                return NotFound();
            }
            return Ok(skills);
        }
        [HttpGet("getSkillById/{id}")]
        public async Task<ActionResult<Skill>> GetSkill(long id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }
            var SkillDto = new SkillDto
            {
                SkillId = skill.SkillId,
                UserId = skill.UserId,
                SkillName = skill.SkillName,
                SkillDescription = skill.SkillDescription,
                YearsOfExperience = skill.YearsOfExperience,
                SkillLevel = skill.SkillLevel,
                SkillType = skill.SkillType
            };
            return Ok(SkillDto);
        }
        [HttpGet("getSkillsByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkillsByUserId(long userId)
        {
            var skills = await _context.Skills
                .Include(s => s.Languages)
                .Include(s => s.Links)
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var skillDto2 = skills.Select(s => new SkillDto2
            {
                SkillId = s.SkillId,
                UserId = s.UserId,
                SkillName = s.SkillName,
                SkillDescription = s.SkillDescription,
                YearsOfExperience = s.YearsOfExperience,
                SkillLevel = s.SkillLevel,
                SkillType = s.SkillType,
                Languages = s.Languages.Select(l => new Language
                {
                    LanguageId = l.LanguageId,
                    LanguageName = l.LanguageName
                }).ToList(),
                Links = s.Links.Select(l => new Link
                {
                    LinksId = l.LinksId,
                    LinkInformation = l.LinkInformation
                }).ToList()
            });

            return Ok(skillDto2);
        }
        [HttpGet("getSkills1")]
        public async Task<ActionResult<Skill>> GetSkills([FromQuery] string skillType)
        {
            var skills = await _context.Skills.Where(s => s.SkillType == skillType).ToListAsync();
            if (skills == null)
            {
                return NotFound();
            }
            return Ok(skills);
        }
        [HttpGet("getSkills")]
        public async Task<ActionResult<Skill>> GetSkills()
        {
            var skillsWithDetails = await _context.Skills
                        .Include(s => s.Languages)
                        .Include(s => s.Links)
                        .Include(s => s.User.Wishlists)
                        .ToListAsync();

            var skillDetailsDto = skillsWithDetails.Select(s => new SkillDto2
            {
                SkillId = s.SkillId,
                UserId = s.UserId,
                SkillName = s.SkillName,
                SkillDescription = s.SkillDescription,
                YearsOfExperience = s.YearsOfExperience,
                SkillLevel = s.SkillLevel,
                SkillType = s.SkillType,
                Languages = s.Languages.Select(l => new Language
                {
                    LanguageId = l.LanguageId,
                    LanguageName = l.LanguageName
                }).ToList(),
                Links = s.Links.Select(l => new Link
                {
                    LinksId = l.LinksId,
                    LinkInformation = l.LinkInformation
                }).ToList(),
                Wishlists = s.User.Wishlists.Select(w => new Wishlist
                {
                    WishlistId = w.WishlistId,
                    WishlistName = w.WishlistName
                }).ToList()
            });

            return Ok(skillDetailsDto);
        }
        [HttpGet("api/users/{userId}")]
        public async Task<IActionResult> UserExists(long userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}