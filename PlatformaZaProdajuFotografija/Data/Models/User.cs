﻿namespace Data.Models;

public partial class User
{
    public int Id { get; set; }

    public Guid Guid { get; set; } = Guid.NewGuid();

    public string Username { get; set; } = null!;

    public bool Admin { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Download> Downloads { get; set; } = new List<Download>();

    public virtual ICollection<Picture> Pictures { get; set; } = new List<Picture>();
}
