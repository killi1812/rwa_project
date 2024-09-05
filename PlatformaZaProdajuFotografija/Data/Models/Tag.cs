namespace Data.Models;

public partial class Tag
{
    public int Id { get; set; }

    public Guid Guid { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public virtual ICollection<PictureTag> PictureTags { get; set; } = new List<PictureTag>();
}
