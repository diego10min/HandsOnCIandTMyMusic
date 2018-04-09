using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Entity
{
    public class PlaylistMusicasContext : DbContext
    {
        public PlaylistMusicasContext(DbContextOptions<PlaylistMusicasContext> options)
            : base(options)
        { }

        public DbSet<PlaylistMusicas> PlaylistMusicas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(new Util().ConnectionStringDB());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlaylistMusicas>()
                .HasKey(c => new { c.PlaylistId, c.MusicaId });
        }

        public override int SaveChanges()
        {
            this.ChangeTracker.DetectChanges();
            var changedEntityNames = this.GetChangedEntityNames();

            var result = base.SaveChanges();
            this.GetService<IEFCacheServiceProvider>().InvalidateCacheDependencies(changedEntityNames);

            return result;
        }
    }

    public class PlaylistMusicas
    {
        [Key]
        public string PlaylistId { get; set; }

        public string MusicaId { get; set; }

        public Musicas Musica { get; set; }
    }
}
