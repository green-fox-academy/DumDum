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
        public DbSet<Player> Kingdoms { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        

    }
}
