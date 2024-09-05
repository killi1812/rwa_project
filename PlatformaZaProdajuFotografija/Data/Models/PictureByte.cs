namespace Data.Models;

public partial class PictureByte
{
    public int PictureId { get; set; }

    public byte[] Data { get; set; } = null!;

    public virtual Picture Picture { get; set; } = null!;
}
