using System.ComponentModel.DataAnnotations;

namespace PROJECTALTERAPI.Dtos;

public record class UsernameDto
{
    public string Username { get; set; } = null!;
}
