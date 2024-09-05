namespace Data.Models;

public partial class PictureTag : IComparable<Tag>
{
    public int Id { get; set; }

    public int PictureId { get; set; }

    public int TagId { get; set; }

    public virtual Picture Picture { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;

    public int CompareTo(Tag? other)
    {
        if (other == null) return 1;
        return Tag.Guid.CompareTo(other.Guid);
    }
}