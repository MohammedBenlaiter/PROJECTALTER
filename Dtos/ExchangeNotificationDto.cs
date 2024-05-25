namespace PROJECTALTERAPI;

public record class ExchangeNotificationDto
{
    public long ExchangeId { get; set; }

    public long ReciverId { get; set; }

    public long SenderId { get; set; }

    public long SkillReceiveId { get; set; }

    public string Statues { get; set; } = null!;

    public string senderFirstName { get; set; } = null!;

    public string senderLastName { get; set; } = null!;

    public string senderUserName { get; set; } = null!;
}
