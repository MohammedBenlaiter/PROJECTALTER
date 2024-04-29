namespace PROJECTALTERAPI.Dtos;

public record class CreateSkillDto
{
    public string SkillName { get; set; } = null!;

    public string SkillDescription { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public string? SkillLevel { get; set; }
}
