using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Entity
{
    public class PlaylistsContext : DbContext
    {
        public PlaylistsContext(DbContextOptions<PlaylistsContext> options)
            : base(options)
        { }

        public DbSet<Playlists> Playlist { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(new Util().ConnectionStringDB());
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

    public class Playlists
    {
        public string Id { get; set; }

        public Usuarios Usuario { get; set; }

        public List<PlaylistMusicas> PlaylistMusicas { get; set; }
    }
}
