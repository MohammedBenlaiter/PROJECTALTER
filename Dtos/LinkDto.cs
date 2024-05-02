namespace PROJECTALTERAPI.Dtos;

public record class LinkDto
{
    public long LinksId { get; set; }

    public long SkillId { get; set; }

    public string LinkInformation { get; set; } = null!;
}
