using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DumDum.Models.Entities;

namespace DumDum.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public Player Player { get; set; }
        public DbSet<Kingdom> Kingdoms { get; set; }
        public DbSet<Resource> Resources { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasOne<Kingdom>(q => q.Kingdom)
                .WithOne( a => a.Player)
                .HasForeignKey<Player>(a =>a.KingdomId)
                .IsRequired(true);
            
            modelBuilder.Entity<Kingdom>()
                .HasMany<Resource>(k => k.Resources)
                .WithOne( a => a.Kingdom)
                .HasForeignKey(a =>a.KingdomId)
                .IsRequired(true);
        }
    }
}
