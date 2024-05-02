using System;
using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;

namespace PROJECTALTERAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly AlterDbContext _context;

        public SkillController(AlterDbContext context)
        {
            _context = context;
        }
        [HttpPost("CreateSkillListining/{id}")]
        public IActionResult CreateSkillListining(long id, SkillDto skillDto)
        {
            // Check if the provided ID exists in the database
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            // Create a new Skill object based on the provided SkillDto
            var skill = new Skill
            {
                UserId = id,
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
        [HttpGet("SkillSearch")]
        public IActionResult SearchSkill(SearchDto query)
        {
            var skills = _context.Skills.Where(s => s.SkillName.Contains(query.Query) || s.SkillDescription.Contains(query.Query) || s.SkillLevel.Contains(query.Query) || s.SkillType.Contains(query.Query));
            if (skills == null)
            {
                return NotFound();
            }
            var dto = skills.Select(s => new SkillDto
            {
                SkillId = s.SkillId,
                UserId = s.UserId,
                SkillName = s.SkillName,
                SkillDescription = s.SkillDescription,
                YearsOfExperience = s.YearsOfExperience,
                SkillLevel = s.SkillLevel,
                SkillType = s.SkillType
            });
            return Ok(dto);
        }
        [HttpGet("getSkill/{id}")]
        public async Task<ActionResult<Skill>> GetSkill(long id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            return skill;
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