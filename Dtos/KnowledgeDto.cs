namespace PROJECTALTERAPI.Dtos;

public record class KnowledgeDto
{
    public int Id { get; set; }
    public string KnowledgeName { get; set; } = null!;
}
