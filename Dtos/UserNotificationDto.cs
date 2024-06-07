using PROJECTALTERAPI.Dtos;

namespace PROJECTALTERAPI;

public record class UserNotificationDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public byte[]? Picture { get; set; }
    public List<SkillDto> Skills { get; set; } = null!;// Add the 'Skills' property

}
