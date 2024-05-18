namespace PROJECTALTERAPI.Dtos;

public record class ExchangeDto
{
    public long ExchangeId { get; set; }

    public long ReciverId { get; set; }

    public long SenderId { get; set; }

    public long SkillSendId { get; set; }

    public long SkillReceiveId { get; set; }

    public string Statues { get; set; } = null!;
}
