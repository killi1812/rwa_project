using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class PictureTag
{
    public int Id { get; set; }

    public int PictureId { get; set; }

    public int TagId { get; set; }

    public virtual Picture Picture { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
