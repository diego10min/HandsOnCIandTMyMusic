using EFSecondLevelCache.Core;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Entity
{
    public class UsuariosContext : DbContext
    {
        public UsuariosContext(DbContextOptions<UsuariosContext> options)
            : base(options)
        { }

        public DbSet<Usuarios> Usuarios { get; set; }

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

    public class Usuarios
    {
        public string Id { get; set; }

        public string Nome { get; set; }

        public string PlaylistId { get; set; }
    }
}
