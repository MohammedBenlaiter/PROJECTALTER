namespace PROJECTALTERAPI.Dtos;

public record class LoginDto
{
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
}
