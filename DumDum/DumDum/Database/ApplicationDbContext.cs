using Microsoft.EntityFrameworkCore;
using DumDum.Models.Entities;
using DumDum.Models.JsonEntities.Battles;

namespace DumDum.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Kingdom> Kingdoms { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Troop> Troops { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<BuildingLevel> BuildingLevels { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<TroopLevel> TroopLevel { get; set; }
        public DbSet<TroopTypes> TroopTypes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<TroopsLost> TroopsLost { get; set; }

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
                .IsRequired(true);

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
            
            modelBuilder.Entity<BuildingType>()
                .HasMany<BuildingLevel>(b =>b.BuildingLevels)
                .WithOne(a => a.BuildingType)
                .HasForeignKey(a => a.BuildingLevelId)
                .IsRequired(true);
            
            modelBuilder.Entity<Building>()
                .HasMany<BuildingType>(b =>b.BuildingTypes)
                .WithOne(a => a.Building)
                .HasForeignKey(a => a.BuildingTypeId)
                .IsRequired(true);
        }
    }
}