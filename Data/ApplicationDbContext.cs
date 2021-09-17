using GymBookingSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymBookingSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        //4.
        public DbSet<GymClass> GymClasses { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }




    }
}
