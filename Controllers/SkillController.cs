using System;
using Microsoft.AspNetCore.Mvc;
using PROJECTALTERAPI.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PROJECTALTERAPI.Dtos;

namespace PROJECTALTERAPI.Controllers
{ // yoooooooooooooo
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