namespace PROJECTALTERAPI;

public class AuthResponse
{
    public long UserId { get; set; }

    public string Password { get; set; } = null!;

    public string Username { get; set; } = null!;
}
