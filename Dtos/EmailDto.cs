using System.ComponentModel.DataAnnotations;

namespace PROJECTALTERAPI.Dtos;

public record class EmailDto
{
    public string Email { get; set; } = null!;
}
