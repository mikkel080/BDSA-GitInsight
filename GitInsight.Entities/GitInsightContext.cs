namespace GitInsight.Entities;

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata;

using GitInsight.Core;

public partial class GitInsightContext : DbContext
{
    public GitInsightContext(DbContextOptions<GitInsightContext> options)
            : base(options)
    {
    }
    public virtual DbSet<Author> Authors { get; set; } = null!;
    public virtual DbSet<Repo> Repos { get; set; } = null!;
    public virtual DbSet<Commit> Commits { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Repo>()
        .Property(x => x.Name)
        .HasMaxLength(100);

        modelBuilder.Entity<Repo>()
        .HasIndex(x => x.Name)
        .IsUnique();

        modelBuilder.Entity<Author>()
        .Property(x => x.Name)
        .HasMaxLength(100);

        modelBuilder.Entity<Author>()
        .HasIndex(x => x.Name)
        .IsUnique();

        modelBuilder.Entity<Commit>()
        .HasOne(c => c.Repo)
        .WithMany(r => r.AllCommits)
        .HasForeignKey(e => e.RepoID);

        modelBuilder.Entity<Commit>()
        .HasOne(c => c.Author)
        .WithMany(a => a.AllCommits)
        .HasForeignKey(e => e.AuthorID);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}