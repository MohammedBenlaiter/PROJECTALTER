using System.ComponentModel.DataAnnotations;

namespace PROJECTALTERAPI.Dtos;

public record class EmailDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}