using Microsoft.EntityFrameworkCore;
using Archetypical.Software.SchemaRegistry.Shared.Models;

namespace Archetypical.Software.SchemaRegistry.Shared.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

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