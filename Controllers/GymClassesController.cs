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
using GymBookingSystem.ViewModels;

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
            var model = db.GymClasses.IgnoreQueryFilters(); // Query filter for only future bookings.

            return View("Index", await model.ToListAsync());
        }

        public async Task<IActionResult> MoreSophisticatedIndex()
        {

            bool isTrue;

            isTrue = true;

            //isTrue = db.ApplicationUserGymClasses.Find(userId, 1) is null; 
            //=> tempInCelsius < 20.0 ? "Cold." : "Perfect!";

            bool isAttending(int id, string userId)
            {
                return !(db.ApplicationUserGymClasses.Find(userId, id) == null);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Not null.
            
            var Attending = db.ApplicationUserGymClasses.Where(agc => agc.ApplicationUserId == userId); // Why is this NULL for Logged in ADmin?? .Any doesnt work so well here. Do Where and Check.

            var model2 = db.ApplicationUserGymClasses.Include(agc => agc.GymClass)
                .Where(agc => agc.ApplicationUserId == userId); //Attending classes

            var model = db.GymClasses.Select(g => new GymClassAttendingViewModel {
                Id = g.Id,
                Name = g.Name,
                StartTime = g.StartTime,
                Duration = g.Duration,
                Attending = db.ApplicationUserGymClasses.Any(agc => (agc.GymClassId == g.Id) && (agc.ApplicationUserId == userId)) // THis should then be true with that logic. But it is false. So agc.ApplicationUserId == userId is false.

                //Attending = !(g.ApplicationUserGymClasses.FirstOrDefault(a => a.ApplicationUserId == userId && a.GymClassId == g.Id) == null) // Not null => True/Attending
                //Transform to T-SQL but a stored procedure would be better since FIND works for this problem.
                //Attending = isAttending(g.Id, userId) //With anonymous function there is no such confusion, so they are allowed.
                //Functions cannot be packed into Linq so easily since they need to be read out.
                //https://stackoverflow.com/questions/44228502/an-expression-tree-may-not-contain-a-reference-to-a-local-function
                //Find  cannot be called with instance of type 'System.Linq.IQueryable
                //Attending = !(db.ApplicationUserGymClasses.FirstOrDefault(agc => (agc.GymClassId == g.Id) && (agc.ApplicationUserId == userId)) == null) // Still too Loopy.
                //Attending = db.ApplicationUserGymClasses.Where(agc => agc.GymClassId == 1) // Still too Loopy.
                //Attending =  db.ApplicationUserGymClasses.Any(agc => agc.ApplicationUserId == userId)  // Evaluates to FALSE. WHY? Not Logged in? nope.
                //Attending = db.ApplicationUserGymClasses.Any(agc => agc.GymClassId == g.Id) // Evaluates to True...
                // BUGGY SCOPE OF LINQ maybe. Linq => T-SQL if no connection to the Query then it cannot evaluate it. So if i dont use g. then it wont help.
                //These Bools are reversed.
                //Attending = g.Id is int

            });

            return View("MoreSophisticatedIndex", await model.ToListAsync());
        }


        [Authorize]
        public async Task<IActionResult> MyHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Not functioning as intended. Perhaps turn it around.
            var model = db.GymClasses.IgnoreQueryFilters()
                .Include(gc=> gc.ApplicationUsers.Where(au => au.Id == userId))
                .Where(gc => gc.StartTime < DateTime.Now);

            // userId, id
            //GET BOOL FOR ATTENDING. USED LATER IN LAMBDA filteredMyHistory
            var myHistoryIds = db.ApplicationUserGymClasses 
                .IgnoreQueryFilters()
                .Where(agc => agc.ApplicationUserId == userId)
                .Select(agc => agc.GymClassId); // Not needed if we want all. This is not a bool series though...

            //var filtered = listOfAllVenues
            //       .Where(x => !listOfBlockedVenues.Any(y => y.VenueId == x.Id));

            var filtered = db.GymClasses.Where(gc => db.ApplicationUserGymClasses.Any(agc => agc.GymClassId == gc.Id)); // Same as ALL classes with Query Filter
            //var filtered2 = db.GymClasses.Where(gc => db.ApplicationUserGymClasses.Any(agc => agc.ApplicationUserId == userId)); // MY BOOKINGS QUERYFILTER + "MY BOOKINGS"
            var filteredMyHistory = db.GymClasses
                .IgnoreQueryFilters()
                .Where(gc => db.ApplicationUserGymClasses.Any(agc => (agc.GymClassId == gc.Id) && (agc.ApplicationUserId == userId) && (gc.StartTime < DateTime.Now)));
            // The seed is not Testable for this. Added more classes to Admin to test this.

            return View(nameof(Index),await filteredMyHistory.ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var futureBookings = db.ApplicationUserGymClasses.Include(agc => agc.GymClass)
                .Where(agc => agc.ApplicationUserId == userId)
                .Select(agc => agc.GymClass);


            var myFutureBookings = db.GymClasses.Include(gc =>  gc.ApplicationUserGymClasses.Where(agc => agc.ApplicationUserId == userId))
                .Where(gc => db.ApplicationUserGymClasses.Any(agc => agc.ApplicationUserId == userId)); // MY BOOKINGS QUERYFILTER is only for future bookings.
            //Also False...

            return View(nameof(Index), await futureBookings.ToListAsync());
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

        public async Task<IActionResult> BookingToggleTwo(int? id)
        {
            if (id is null)
            {
                //throw new ArgumentNullException(nameof(id));
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAttendingRow = db.ApplicationUserGymClasses.Find(userId, id);

            if (isAttendingRow is null)
            {
                db.ApplicationUserGymClasses.Add( new ApplicationUserGymClass 
                {
                    ApplicationUserId = userId,
                    GymClassId = (int)id
                });
            }
            else
            {
                db.ApplicationUserGymClasses.Remove(isAttendingRow);
            }
            await db.SaveChangesAsync();

            return RedirectToAction("MoreSophisticatedIndex");
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
                //TempData["BookedStatus"] = $"Successfull booking for {gymClass.Name} at {gymClass.StartTime}";
                //TempData["BookedStatus"] = $"Successfull booking for ...";
                TempData["Toggle"] = true;

            }
            else
            {
                db.ApplicationUserGymClasses.Remove(isAttendingRow);
                //TempData["BookedStatus"] = $"Successfull UNbooking for {gymClass.Name} at {gymClass.StartTime}";
                //TempData["BookedStatus"] = $"Successfull UNbooking for ";
                TempData["Toggle"] = null;
            }
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(MoreSophisticatedIndex));
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
