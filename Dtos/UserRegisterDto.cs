namespace PROJECTALTERAPI.Dtos;

public record class UserRegisterDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? Picture { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
}
