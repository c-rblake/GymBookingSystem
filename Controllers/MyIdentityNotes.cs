using GymBookingSystem.Data;
using GymBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymBookingSystem.Controllers
{
    public class MyIdentityNotes : Controller
    {
        //Declare User manager
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public MyIdentityNotes(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
        {
            db = context;
            this.userManager = userManager;
        }

        public static async void GetUser()
        {
            //var user = await userManager.GetUserAsync(User); //GET IDENTITY USER
        }



    }
}
