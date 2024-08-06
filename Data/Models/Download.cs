using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Download
{
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public int PictureId { get; set; }

    public int UserId { get; set; }

    public virtual Picture Picture { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
