using Microsoft.EntityFrameworkCore;
using Archetypical.Software.SchemaRegistry.Shared.Models;
using Archetypical.Software.Vega.Api.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Archetypical.Software.SchemaRegistry.Shared.Data
{
    public class Context(DbContextOptions<VegaDbContext> options, IConfiguration config)
        :
            VegaDbContext(options, config)
    {
        public DbSet<SchemaGroup> SchemaGroups { get; set; }

        public DbSet<Schema> Schemata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SchemaGroup>(x =>
            {
                x.HasMany(sc => sc.Schemas)
                    .WithOne(x => x.SchemaGroup)
                    .HasForeignKey(x => x.SchemaGroupId);
                x.Ignore(x => x.GroupProperties);
            });

            modelBuilder.Entity<Schema>(x =>
            {
                x.HasKey(s => new { s.Id, s.SchemaGroupId, s.Version });
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}