namespace PROJECTALTERAPI.Dtos;

public record class SkillLanguageDto
{
    public long LanguageId { get; set; }
    public long SkillId { get; set; }
    public string LanguageName { get; set; } = null!;
}