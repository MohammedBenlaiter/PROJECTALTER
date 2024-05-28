
namespace PROJECTALTERAPI.Dtos;

public record class UserDto
{
    public long UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Picture { get; set; }

    public string Username { get; set; } = null!;
}
