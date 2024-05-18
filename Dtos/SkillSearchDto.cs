namespace PROJECTALTERAPI;

public record class SkillSearchDto
{
    public long UserId { get; set; }

    public string Username { get; set; } = null!;

    public string SkillName { get; set; } = null!;

    public string SkillDescription { get; set; } = null!;

    public string SkillLevel { get; set; } = null!;

    public string SkillType { get; set; } = null!;

    public ICollection<Wishlist> wishlists { get; set; } = new List<Wishlist>();
}
