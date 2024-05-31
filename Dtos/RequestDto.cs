namespace PROJECTALTERAPI;

public record class RequestDto
{
    public long RequestId { get; set; }

    public long UserId { get; set; }

    public string RequestTitle { get; set; } = null!;

    public string RequestDescription { get; set; } = null!;

    public DateOnly Deadline { get; set; }
}
