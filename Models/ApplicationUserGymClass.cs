using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Models
{
    public class ApplicationUserGymClass
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public int GymClassId { get; set; }

        //NAV/FLUENT PROPRETIES
        public ApplicationUser ApplicationUser { get; set; }
        public GymClass GymClass { get; set; }


    }
}
