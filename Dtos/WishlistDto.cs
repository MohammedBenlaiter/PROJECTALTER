namespace PROJECTALTERAPI.Dtos;

public record class WishlistDto
{
    public long WishlistId { get; set; }

    public string WishlistName { get; set; } = null!;

    public long UserId { get; set; }
}
