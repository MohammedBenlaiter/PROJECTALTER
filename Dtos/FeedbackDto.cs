namespace PROJECTALTERAPI;

public record class FeedbackDto
{
    public long FeedbackId { get; set; }

    public long UserId { get; set; }

    public string Description { get; set; } = null!;
}
