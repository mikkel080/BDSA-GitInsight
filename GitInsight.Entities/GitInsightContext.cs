namespace GitInsight.Entities;

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata;

using GitInsight.Core;

public partial class GitInsightContext : DbContext {
    public GitInsightContext(DbContextOptions<GitInsightContext> options)
            : base(options)
        {
        }
    public virtual DbSet<Author> Authors { get; set; } = null!;
    public virtual DbSet<Repo> Repos { get; set; } = null!;
    public virtual DbSet<Commit> Commits { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}