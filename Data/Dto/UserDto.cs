namespace Data.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Admin { get; set; }
    // public List<DownloadDto> Downloads { get; set; } = new List<DownloadDto>();
    // public List<PictureDto> Pictures { get; set; } = new List<PictureDto>();
}