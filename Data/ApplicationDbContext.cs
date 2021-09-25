using GymBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using GymBookingSystem.ViewModels;

namespace GymBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        //4.
        public DbSet<GymClass> GymClasses { get; set; }
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<GymClass>().Property<DateTime>("Edited"); // ShadowProperty. messes with seed
            
            //Explicit fluent API connection of Join Table 
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(au => au.GymClasses)
                .WithMany(gc => gc.ApplicationUsers)
                .UsingEntity<ApplicationUserGymClass>(
                    a => a.HasOne(a => a.GymClass).WithMany(gc => gc.ApplicationUserGymClasses),
                    a => a.HasOne(a => a.ApplicationUser).WithMany(au => au.ApplicationUserGymClasses)
                );

            //Composite Key
            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(a => new { a.ApplicationUserId, a.GymClassId });

            modelBuilder.Entity<GymClass>().HasQueryFilter(g => g.StartTime > DateTime.Now);
            ////REVERESE SIMPLE TABLE
            //modelBuilder.Entity<GymClass>()
            //    .HasMany(gc => gc.ApplicationUsers)
            //    .WithMany(au => au.GymClasses);
            //Composite key

        }


        //public DbSet<GymBookingSystem.ViewModels.GymClassAttendingViewModel> GymClassAttendingViewModel { get; set; }

    }
}
