using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Picture
{
    public int Id { get; set; }

    public Guid Guid { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string Photographer { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<Download> Downloads { get; set; } = new List<Download>();

    public virtual PictureByte? PictureByte { get; set; }

    public virtual ICollection<PictureTag> PictureTags { get; set; } = new List<PictureTag>();

    public virtual User User { get; set; } = null!;
}
