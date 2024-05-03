namespace PROJECTALTERAPI;

public record class SearchDto
{
    public string Query { get; set; } = null!;
    public List<Knowledge> Knowledges { get; set; } = null!;

}
