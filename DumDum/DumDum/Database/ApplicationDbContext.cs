using Microsoft.EntityFrameworkCore;
using DumDum.Models.Entities;

namespace DumDum.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Kingdom> Kingdoms { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Troop> Troops { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<TroopLevel> TroopLevel { get; set; }
        public DbSet<TroopTypes> TroopTypes { get; set; }

        public DbSet<LastChange> LastChanges { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasOne<Kingdom>(q => q.Kingdom)
                .WithOne(a => a.Player)
                .HasForeignKey<Player>(a => a.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Kingdom>()
                .HasMany<Resource>(k => k.Resources)
                .WithOne(a => a.Kingdom)
                .HasForeignKey(a => a.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Troop>()
                .HasOne<Kingdom>(t => t.Kingdom)
                .WithMany(k => k.Troops)
                .HasForeignKey(t => t.KingdomId)
                .IsRequired(false);

            modelBuilder.Entity<Kingdom>()
                .HasMany<Building>(k => k.Buildings)
                .WithOne(a => a.Kingdom)
                .HasForeignKey(a => a.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Troop>()
                 .HasOne<TroopTypes>(t => t.TroopType)
                 .WithMany(t => t.Troops)
                 .HasForeignKey(t => t.TroopTypeId)
                 .IsRequired(true);

            modelBuilder.Entity<TroopLevel>()
               .HasOne<TroopTypes>(t=>t.TroopType)
               .WithOne(t => t.TroopLevel)
               .HasForeignKey<TroopLevel>(t=>t.TroopTypeId)
               .IsRequired(true);

            modelBuilder.Entity<Player>()
                .HasOne<LastChange>(p => p.LastChange)
                .WithOne(l => l.Player)
                .HasForeignKey<LastChange>(p => p.PlayerId)
                .IsRequired(false);
        }
    }
}