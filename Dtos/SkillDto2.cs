namespace PROJECTALTERAPI.Dtos;

public record class SkillDto2
{
    public long SkillId { get; set; }

    public long UserId { get; set; }

    public string SkillName { get; set; } = null!;

    public string SkillDescription { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public string SkillLevel { get; set; } = null!;

    public string SkillType { get; set; } = null!;

    public virtual List<Language> Languages { get; set; } = null!;

    public virtual List<Link> Links { get; set; } = null!;

    public virtual List<Wishlist> Wishlists { get; set; } = null!;
}
