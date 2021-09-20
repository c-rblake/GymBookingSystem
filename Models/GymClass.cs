using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Models
{
    public class GymClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime EndTime => StartTime + Duration;
        public string Description { get; set; }

        //Nav Properties
        //public ICollection<ApplicationUser> ApplicationUsers { get; set; } = new ICollection<ApplicationUser>(); //This is to set without join table
        //To Debugg I need a non-null object in Razor
        public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();

        //Nav Properties Explicit Join Table
        public ICollection<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; }

        

    }
}
