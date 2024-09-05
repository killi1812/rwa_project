namespace Data.Dto;

public class PictureDto
{
   public string guid { get; set; } = null!;
   public string name { get; set; } = null!;
   public string photographer { get; set; } = null!;
   public UserDto user { get; set; } = null!; 
   public List<string> tags { get; set; } = null!;
   // public List<DownloadDto> downloads { get; set; } = null!;
}