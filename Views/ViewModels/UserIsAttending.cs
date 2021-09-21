using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Views.ViewModels
{
    public class UserIsAttending
    {
        public string GymClassName { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsAttending { get; set; }
    }
}
