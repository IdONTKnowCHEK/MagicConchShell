using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MagicConchShell.Models;

public partial class SpongebobsContext : DbContext
{
    public SpongebobsContext()
    {
    }

    public SpongebobsContext(DbContextOptions<SpongebobsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SpongebobDatum> SpongebobData { get; set; }

   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SpongebobDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("spongebob_data");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
