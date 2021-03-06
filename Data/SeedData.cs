using Bogus;
using GymBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();



                if (await db.ApplicationUserGymClasses.AnyAsync()) return;

                if(true)
                {
                    // first we create Admin role  
                    var role = new IdentityRole();
                    role.Name = "Admin";
                    await roleManager.CreateAsync(role);

                    //Make ApplicationUser
                    
                    var user = new ApplicationUser();
                    user.UserName = "edit@e.com"; // USERNAME and Email are the same here
                    user.Email = "edit@e.com";

                    string userPassword = "Passw0rd";

                    //if (!await checkUser) Check for success
                    await userManager.CreateAsync(user, userPassword);

                    await userManager.AddToRoleAsync(user, "Admin"); // Doesnt Work.

                    await db.SaveChangesAsync();
                }

                var admin = await userManager.FindByEmailAsync("edit@e.com");

                //var oneGymClass = MakeOneClass();
                //await db.GymClasses.AddAsync(oneGymClass);
                //await db.SaveChangesAsync();

                var classes = MakeGymClasses();
                await db.GymClasses.AddRangeAsync(classes); // "object" MakeGymClasses() CANNOT CONVERT OBJECT
               

                var applicationUsers = MakeApplicationUsers();
                //await db.ApplicationUser.AddRangeAsync(applicationUsers); // The Db is not called application user
                await db.Users.AddRangeAsync(applicationUsers); // Where AM I adding these...
                await db.SaveChangesAsync();

                var applicationUserGymClasses = MakeApplicationUserGymClasses(applicationUsers, classes, admin);
                await db.ApplicationUserGymClasses.AddRangeAsync(applicationUserGymClasses);
                await db.SaveChangesAsync();

                //MakeClasses(db);

                //if (!await roleManager.RoleExistsAsync("Admin")) // Debugger Passes this check...
            }
        }

        private static List<ApplicationUserGymClass> MakeApplicationUserGymClasses(List<ApplicationUser> applicationUsers, List<GymClass> gymClasses, ApplicationUser admin)
        {
            var applicationUserGymClasses = new List<ApplicationUserGymClass>();
            string adminId = admin.Id;

            //var adminId = db.ApplicationUsers.Where(ac => ac.UserName == "edit@e.com").Select(ac => ac.Id).FirstOrDefault(); Not a good idea nice query though.

            for (int i = 0; i < 20; i++)
            {

                var applicationGymClass = new ApplicationUserGymClass
                {
                    ApplicationUserId = applicationUsers[i].Id,
                    GymClassId = gymClasses[i].Id                    
                    //ApplicationUserId = applicationUsers[i%applicationUsers.Count].Id,
                    //GymClassId = gymClasses[i%gymClasses.Count].Id
                };
                applicationUserGymClasses.Add(applicationGymClass);

                var adminApplicationGymClass = new ApplicationUserGymClass
                {
                    ApplicationUserId = adminId,
                    GymClassId = gymClasses[i].Id
                };
                applicationUserGymClasses.Add(adminApplicationGymClass); // is never saved...

            }
            return applicationUserGymClasses;

        }

        //public IQueryable<ApplicationUser> GetUsersInRole(string roleName)
        //{
        //    return from user in ApplicationUsers
        //           where user.Roles.Any(r => r.Role.Name == roleName)
        //           select user;
        //}

        private static List<ApplicationUser> MakeApplicationUsers()
        {
            var applicationUsers = new List<ApplicationUser>();

            for (int i = 0; i < 20; i++)
            {
                var applicationUser = new ApplicationUser
                {
                    FirstName = fake.Name.FirstName(),
                    LastName = fake.Name.LastName(),
                    UserName = fake.Name.FirstName(),
                    Email = fake.Internet.Email(),
                    TimeOfRegistration = fake.Date.Recent(3)
                    //Id = fake.Lorem.Letter(8).ToUpper() // Works??
                };
                applicationUsers.Add(applicationUser);
            }

            return applicationUsers;
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
            var timeSpanOld = new TimeSpan(5, 4, 0, 0);
            var timeSpan = new TimeSpan(5, 4, 0, 0);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("hello");
                var gymclass = new GymClass
                {
                    //Id = i, // THIS HERE LINE `SHOULD NOT BE HERE RIGHT?? it is set by the dbContext...
                    Name = $"{fake.Name.FirstName()}ball",
                    StartTime = DateTime.Now.AddDays(fake.Random.Int(-5, 5)),
                    Duration = TimeSpan.FromMinutes(20),
                    Description = fake.Hacker.Verb()
                };
                gymClasses.Add(gymclass);
            }
            return gymClasses;
        }

        private static async void MakeClasses(DbContext db)
        {
            var applicationGymClasses = new List<ApplicationUserGymClass>();
            var gymClasses = new List<GymClass>();
            var ApplicationUsers = new List<ApplicationUser>();
            var timeSpanOld = new TimeSpan(5,4, 0, 0);
            var timeSpan = new TimeSpan(5,4,0,0);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("hello");
                var gymclass = new GymClass
                {
                    Name = $"{fake.Name.FirstName()} + ball ",
                    StartTime = fake.Date.Between((DateTime.Now - timeSpanOld), (DateTime.Now + timeSpan)),
                    Duration = TimeSpan.FromMinutes(20),
                    Description = "Random Description Here"
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

            await db.SaveChangesAsync();

        }
    }
}
