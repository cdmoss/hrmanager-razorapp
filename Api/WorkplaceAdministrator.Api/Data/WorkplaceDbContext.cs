using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WorkplaceAdministrator.Common;

namespace WorkplaceAdministrator.Api.Data
{
    public class WorkplaceDbContext : IdentityDbContext<WorkplaceAccount, IdentityRole<Guid>, Guid>
    {
        public DbSet<WorkplaceAccount> Accounts { get; set; }
        public DbSet<Reference> References { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<AccountPosition> AccountPositions { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<ApplicationAlert> ApplicationAlerts { get; set; }
        public DbSet<ShiftRequestAlert> ShiftRequests { get; set; }

        public WorkplaceDbContext(DbContextOptions<WorkplaceDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid> 
                { 
                    Name = "Admin", 
                    NormalizedName = "ADMIN", 
                    Id = Guid.NewGuid() 
                });

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Name = "SuperAdmin",
                    NormalizedName = "SUPERADMIN",
                    Id = Guid.NewGuid()
                });

            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Id = Guid.NewGuid()
                });

            modelBuilder.Entity<Position>().HasData(new Position { Name = "Front Stock", Id = 6 });
            modelBuilder.Entity<Position>().HasData(new Position { Name = "Warehouse", Id = 1 });
            modelBuilder.Entity<Position>().HasData(new Position { Name = "Janitorial", Id = 2 });
            modelBuilder.Entity<Position>().HasData(new Position { Name = "General Maintenance", Id = 3 });
            modelBuilder.Entity<Position>().HasData(new Position { Name = "Special Events", Id = 4 });
            modelBuilder.Entity<Position>().HasData(new Position { Name = "Community Relations", Id = 5 });

            /* modelBuilder.Entity<WorkplaceUser>()
                 .HasMany(p => p.Positions)
                 .WithOne(p => p.User);
            */
        }
    }
}

            