using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDBContext _context; //readonly field

        public EventsController(ApplicationDBContext context) //parameterised contructor 
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchtype, int? venueId, DateTime? startDate,DateTime? endDate) //asynchronous task method
        {
            //filters
            var events = _context.Events.Include(e => e.Venue).Include(e => e.EventsType).AsQueryable();

            if (!string.IsNullOrEmpty(searchtype))
            {
                events = events.Where(e => e.EventsType.eventsTypeName == searchtype);
            }

            if (venueId.HasValue)
            {
                events = events.Where(e=> e.venueID==venueId);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                events = events.Where(e=>e.eventDate>=startDate && e.eventDate<=endDate);
            }

            //data for dropdown menu
            ViewData["EventsType"] = _context.EventsType.ToList();
            ViewData["Venue"]= _context.Venue.ToList();

            return View(await events.ToListAsync());
        }


        public IActionResult CreateEvents()
        {
            ViewData["EventsType"] = _context.EventsType.ToList();
            ViewData["Venue"]= _context.Venue.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvents(Events events)
        {
            
            
            
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Venue"] = _context.Venue.ToList();
            ViewData["EventsType"] = _context.EventsType.ToList();
            return View(events);
        }

        public async Task<IActionResult> EventsDetails(int id)
        {
            var events = await _context.Events.Include(e=>e.Venue).FirstOrDefaultAsync(m => m.eventsID == id);

            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        public async Task<IActionResult> DeleteEvents(int id)
        {

            if (id == null) return NotFound();

            var events = await _context.Events.Include(e=>e.Venue).FirstOrDefaultAsync(m => m.eventsID == id);

            if (events == null)
            {
                return NotFound();
            }

            return View(events);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            var events = await _context.Events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }

            var isBooked = await _context.Bookings.AnyAsync(b => b.eventsID == id); //check to see if events id is being used in bookings 

            if (isBooked) //if true display error message
            {
                TempData["ErrorMessage"] = "Error: Cannot delete due to event due to existing booking(s)"; //error message
                return RedirectToAction(nameof(Index)); //redirect to index page
            }

            //else delete
            _context.Events.Remove(events);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event Deleted Successfully";
            return RedirectToAction(nameof(Index));

        }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.eventsID == id);
        }

        public async Task<IActionResult> EditEvents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var events = await _context.Events.FindAsync(id);

            if (id == null)
            {
                return NotFound();
            }

            ViewData["Venue"] = _context.Venue.ToList();
            ViewData["EventsType"] = _context.EventsType.ToList();

            return View(events);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvents(int id, Events events)
        {
            if (id != events.eventsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(events);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event Updated Successfully";
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!EventsExists(events.eventsID))
                    {
                        return NotFound();
                    }

                    else
                    {
                        throw;
                    }
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
