using Bogus;
using GymBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Data
{
    public class SeedData
    {
        private static Faker fake;

        internal static async Task InitAsync(IServiceProvider services)
        {
            using (var db = services.GetRequiredService<ApplicationDbContext>())
            {
                //TODO
                fake = new Faker("sv");

                //if (await db.ApplicationUserGymClasses.AnyAsync()) return;

                var oneGymClass = MakeOneClass();
                await db.GymClasses.AddAsync(oneGymClass);
                await db.SaveChangesAsync();

                var classes = MakeGymClasses();
                await db.GymClasses.AddRangeAsync(classes); // "object" MakeGymClasses() CANNOT CONVERT OBJECT
                                                            // .. An issue with AddRange <-- any type not GymClasses.AddRange the right type. Still nothing though.
                await db.SaveChangesAsync();

            }
        }

        private static GymClass MakeOneClass()
        {
            var oneGymClass = new GymClass
            {
                Name = "Jumping Jack",
                Description = "Jumping with arms and legs",
                StartTime = DateTime.Now,
                Duration = TimeSpan.FromMinutes(20)
            };

            return oneGymClass;
        }

        private static List<GymClass> MakeGymClasses()
        {
            var gymClasses = new List<GymClass>();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("hello");
                var gymclass = new GymClass
                {
                    //Id = i, // THIS HERE LINE `SHOULD NOT BE HERE RIGHT?? it is set by the dbContext...
                    Name = $"{fake.Name.FirstName()} + ball ",
                    StartTime = fake.Date.Recent(7),
                    Duration = TimeSpan.FromMinutes(20),
                    Description = "Pancake"
                    
                };
                gymClasses.Add(gymclass);
            }
            return gymClasses;
        }

        private static List<ApplicationUserGymClass> MakeClasses()
        {
            var applicationGymClasses = new List<ApplicationUserGymClass>();
            var gymClasses = new List<GymClass>();
            var ApplicationUsers = new List<ApplicationUser>();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("hello");
                var gymclass = new GymClass
                {
                    Id = i,
                    Name = $"{fake.Name.FirstName()} + ball ",
                    StartTime = fake.Date.Recent(7),
                    Duration = TimeSpan.FromMinutes(20)
                };
                gymClasses.Add(gymclass);

                var applicationUser = new ApplicationUser
                {
                    UserName = fake.Name.FirstName(),
                    Email = fake.Internet.Email(),
                    Id = fake.Lorem.Letter(8).ToUpper()
                };
                ApplicationUsers.Add(applicationUser);

                var applicationGymClasse = new ApplicationUserGymClass
                {
                    ApplicationUserId = applicationUser.Id,
                    GymClassId = gymclass.Id
                };
                applicationGymClasses.Add(applicationGymClasse);


            }
            return applicationGymClasses;
        }
    }
}
