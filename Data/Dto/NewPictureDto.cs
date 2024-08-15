using Microsoft.AspNetCore.Http;

namespace Data.Dto;

public class NewPictureDto
{
    public string Name { get; set; }
    public string Photographer { get; set; }
    public string Description { get; set; }
    public IFormFile Data { get; set; }
    public List<string> Tags { get; set; }
}