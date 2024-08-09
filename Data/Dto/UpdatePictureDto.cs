namespace Data.Dto;

public class UpdatePictureDto
{
    public string Guid { get; set; }
    public string? Name { get; set; }
    public string? Photographer { get; set; }
    public List<string> Tags { get; set; }
}