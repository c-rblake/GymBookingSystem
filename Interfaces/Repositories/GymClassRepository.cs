using GymBookingSystem.Data;
using GymBookingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBookingSystem.Interfaces.Repositories
{
    public class GymClassRepository : IGymClassRepository
    {
        private readonly ApplicationDbContext db;

        public GymClassRepository(ApplicationDbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }
        public void Add(GymClass gymClass)
        {
            throw new NotImplementedException();
        }

        public Task<GymClass> FindAsync(int? id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<GymClass>> GetAsync()
        {
            return await db.GymClasses.ToListAsync();
        }
    }
}
