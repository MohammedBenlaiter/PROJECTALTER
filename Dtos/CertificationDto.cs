namespace PROJECTALTERAPI;

public record class CertificationDto
{
    public long UserId { get; set; } 
    public byte[] CertificationPicture { get; set; } = null!;
}
