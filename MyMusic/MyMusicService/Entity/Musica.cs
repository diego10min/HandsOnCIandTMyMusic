using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Entity
{
    public class MusicasContext : DbContext
    {
        public MusicasContext(DbContextOptions<MusicasContext> options)
            : base(options)
        { }

        public DbSet<Musicas> Musicas { get; set; }

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

    public class Musicas
    {
        public string Id { get; set; }

        public string Nome { get; set; }

        public string ArtistaId { get; set; }

        public Artistas Artista { get; set; }
    }

}
