namespace PROJECTALTERAPI;

public record class MessageDto
{
    public long SenderId { get; set; }

    public long ReceiverId { get; set; }

    public string Content { get; set; } = null!;
}
