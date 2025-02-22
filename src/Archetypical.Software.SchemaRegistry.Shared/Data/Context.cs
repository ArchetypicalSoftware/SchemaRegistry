using Microsoft.EntityFrameworkCore;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Archetypical.Software.Vega.Api.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Archetypical.Software.SchemaRegistry.Shared.Data
{
    public class Context(DbContextOptions<VegaDbContext> options, IConfiguration config) : VegaDbContext(options, config)
    {
        public DbSet<SchemaCollection> Collections { get; set; }
        public DbSet<Schema> Schemas { get; set; }

        public DbSet<SchemaVersion> Versions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schema>(x =>
            {
                x.HasMany(sc => sc.Schemas)
                    .WithOne(x => x.SchemaGroup)
                    .HasForeignKey(x => x.SchemaGroupId);
                x.Ignore(x => x.SchemaProperties);
                x.HasOne(y => y.SchemaCollection).WithMany(y => y.Schemas).HasForeignKey(y => y.SchemaCollectionId);
            });

            modelBuilder.Entity<SchemaVersion>(x =>
            {
                x.HasKey(s => new { s.Id, s.SchemaGroupId, s.Version });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}