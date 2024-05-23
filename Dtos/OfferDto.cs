namespace PROJECTALTERAPI;

public record class OfferDto
{
    public long OfferId { get; set; }

    public long UserId { get; set; }

    public long RequestId { get; set; }

    public string OfferInfo { get; set; } = null!;

    public DateOnly Deadline { get; set; }

    public long Price { get; set; }
}