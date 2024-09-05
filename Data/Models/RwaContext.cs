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

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Picture> Pictures { get; set; }

    public virtual DbSet<PictureByte> PictureBytes { get; set; }

    public virtual DbSet<PictureTag> PictureTags { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(
        // "Server=localhost,1433;Database=rwa;User=sa;Password=password123!;Encrypt=False;TrustServerCertificate=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Download>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__download__3213E83F61E18DFF");

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
                .HasConstraintName("FK__downloads__pictu__44FF419A");

            entity.HasOne(d => d.User).WithMany(p => p.Downloads)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__downloads__userI__45F365D3");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__logs__3213E83FF5CACDEC");

            entity.ToTable("logs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Message)
                .IsUnicode(false)
                .HasColumnName("message");
        });

        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pictures__3213E83F08C74D10");

            entity.ToTable("pictures");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Guid).HasColumnName("guid");
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

        modelBuilder.Entity<PictureByte>(entity =>
        {
            entity.HasKey(e => e.PictureId).HasName("PK__pictureB__769A271AF8F2C009");

            entity.ToTable("pictureBytes");

            entity.Property(e => e.PictureId)
                .ValueGeneratedNever()
                .HasColumnName("pictureId");
            entity.Property(e => e.Data).HasColumnName("data");

            entity.HasOne(d => d.Picture).WithOne(p => p.PictureByte)
                .HasForeignKey<PictureByte>(d => d.PictureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pictureBy__pictu__3C69FB99");
        });

        modelBuilder.Entity<PictureTag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pictureT__3213E83F0B8F4659");

            entity.ToTable("pictureTags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PictureId).HasColumnName("pictureId");
            entity.Property(e => e.TagId).HasColumnName("tagId");

            entity.HasOne(d => d.Picture).WithMany(p => p.PictureTags)
                .HasForeignKey(d => d.PictureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pictureTa__pictu__412EB0B6");

            entity.HasOne(d => d.Tag).WithMany(p => p.PictureTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__pictureTa__tagId__4222D4EF");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tags__3213E83FFC7C8F26");

            entity.ToTable("tags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Guid).HasColumnName("guid");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F523538F3");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Admin).HasColumnName("admin");
            entity.Property(e => e.Guid).HasColumnName("guid");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}