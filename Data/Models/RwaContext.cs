using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.Models;

public partial class RwaContext : DbContext
{
    public RwaContext()
    {
    }

    public RwaContext(DbContextOptions<RwaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Download> Downloads { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<PictureTag> PictureTags { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=rwa;User=sa;Password=password123!;Encrypt=False;TrustServerCertificate=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Download>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__download__3213E83F2A4EBE42");

            entity.ToTable("downloads");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.PictureId).HasColumnName("pictureId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Picture).WithMany(p => p.Downloads)
                .HasForeignKey(d => d.PictureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__downloads__pictu__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.Downloads)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__downloads__userI__4222D4EF");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pictures__3213E83F50C24011");

            entity.ToTable("pictures");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Photographer)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("photographer");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Pictures)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pictures__userId__398D8EEE");
        });

        modelBuilder.Entity<PictureTag>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("pictureTags");

            entity.Property(e => e.PictureId).HasColumnName("pictureId");
            entity.Property(e => e.TagId).HasColumnName("tagId");

            entity.HasOne(d => d.Picture).WithMany()
                .HasForeignKey(d => d.PictureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pictureTa__pictu__3D5E1FD2");

            entity.HasOne(d => d.Tag).WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pictureTa__tagId__3E52440B");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tags__3213E83FC58894C7");

            entity.ToTable("tags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F8E2E2DF9");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Admin).HasColumnName("admin");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
