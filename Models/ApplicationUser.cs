using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public DateTime TimeOfRegistration { get; set; } //Can be added as shadow property.

        //Nav property EF CORE 5. Implicit Infer

        public ICollection<GymClass> GymClasses{ get; set; }

        //NAV PROPERTIES to JOIN TABLE
        public ICollection<ApplicationUserGymClass> ApplicationUserGymClasses { get; set; }

    }
}
