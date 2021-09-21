using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBookingSystem.Data;
using GymBookingSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GymBookingSystem.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            db = context;
            this.userManager = userManager;
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {


            return View(await db.ApplicationUserGymClasses.ToListAsync());

        }

        public async Task<IActionResult> Index2()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userGymClasses = db.ApplicationUserGymClasses.ToList();
            var isAttending = db.ApplicationUserGymClasses.Where(agc => agc.ApplicationUserId == userId);

            return View(await db.GymClasses.Include(gc => gc.ApplicationUsers).ToListAsync());

        }

        // GET: GymClasses/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClasses.Include(gc => gc.ApplicationUsers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        [Authorize]
        public async Task<IActionResult> BookingToggle(int? id)
        {
            //bool isAttending;
            if (id == null) return NotFound();
            //var user = await userManager.GetUserAsync(User); GET IDENTITY USER
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // will give the user's userId
            var userName = User.FindFirstValue(ClaimTypes.Name); // will give the user's userName
            var isAttendingRow = db.ApplicationUserGymClasses.Find(userId, id); //null eller rad
            
            // For user Feedback
            GymClass gymClass = db.GymClasses.FirstOrDefault(gc => gc.Id == id);

            if (isAttendingRow is null)
            {
                db.ApplicationUserGymClasses.Add(new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                });
                TempData["BookedStatus"] = $"Successfull booking for {gymClass.Name} at {gymClass.StartTime}";
                //TempData["BookedStatus"] = $"Successfull booking for ...";
                TempData["Toggle"] = true;

            }
            else
            {
                db.ApplicationUserGymClasses.Remove(isAttendingRow);
                TempData["BookedStatus"] = $"Successfull UNbooking for {gymClass.Name} at {gymClass.StartTime}";
                //TempData["BookedStatus"] = $"Successfull UNbooking for ";
                TempData["Toggle"] = null;
            }
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.Add(gymClass);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClasses.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(gymClass);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        [Authorize(Roles = "Admin")]
        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClasses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await db.GymClasses.FindAsync(id);
            db.GymClasses.Remove(gymClass);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return db.GymClasses.Any(e => e.Id == id);
        }
    }
}
