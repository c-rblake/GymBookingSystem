using GymBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Interfaces
{
    public interface IGymClassRepository
    {
        void Add(GymClass gymClass);
        Task<IEnumerable<GymClass>> GetAsync();
        Task<GymClass> FindAsync(int? id);

    }
}
