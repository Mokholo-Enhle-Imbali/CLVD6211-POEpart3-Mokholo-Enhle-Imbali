using AspNetCoreGeneratedDocument;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDBContext _context; //readonly field

        public BookingsController(ApplicationDBContext context) //parameterised contructor 
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string searchtype, int? venueId, DateTime? startDate, DateTime? endDate) //asynchronous task method
        {
            
            //filters
            var bookings = _context.Bookings.Include(b => b.Events).Include(b => b.Venue).AsQueryable();

            if (!string.IsNullOrEmpty(searchtype))
            {
                bookings = bookings.Where(b => b.EventsType.eventsTypeName == searchtype);
            }

            if (venueId.HasValue)
            {
                bookings = bookings.Where(b=> b.venueID==venueId);
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                bookings = bookings.Where(b=>b.bookingDate>=startDate && b.bookingDate<=endDate);
            }

            //data for dropdown menu
            ViewData["EventsType"] = _context.EventsType.ToList();
            ViewData["Venue"] = _context.Venue.ToList();

           

            if (!string.IsNullOrEmpty(searchString))
            {
                bookings = bookings.Where(b => 
                b.Venue.venueName.Contains(searchString) || 
                b.Events.eventName.Contains(searchString));
            }
            return View(await bookings.ToListAsync());
        }

        public IActionResult CreateBookings()
        {
            ViewBag.Events = _context.Events.ToList();
            ViewBag.Venue = _context.Venue.ToList();
            ViewData["EventsType"] = _context.EventsType.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBookings(Bookings booking)
        {


            var selectedEvent= await _context.Events.FirstOrDefaultAsync(e=>e.eventsID==booking.eventsID);

            if (selectedEvent==null)
            {
                ModelState.AddModelError("", "Error: selected event not found");
                ViewBag.Events = _context.Events.ToList();
                ViewBag.Venue = _context.Venue.ToList();
                ViewBag.EventsType= _context.EventsType.ToList();

                return View(booking);
            }

            var conflict = await _context.Bookings
                .Include(b => b.Events)
                .AnyAsync(b => b.venueID == booking.venueID &&
                               b.Events.eventDate.Date == selectedEvent.eventDate.Date);

            if (conflict)
            {
                ModelState.AddModelError("", "Error: This venue is already booked for that date.");
                ViewBag.Events = _context.Events.ToList();
                ViewBag.Venue = _context.Venue.ToList();
                return View(booking);
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

           
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var bookings = await _context.Bookings.FirstOrDefaultAsync(m => m.bookingsID == id);

            if (bookings==null)
            {
                return NotFound();
            }
            
            return View(bookings);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            var bookings = await _context.Bookings.FirstOrDefaultAsync(m=>m.bookingsID==id);

            if (bookings==null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(m => m.bookingsID == id);
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingsExists(int id)
        {
            return _context.Bookings.Any(e => e.bookingsID == id);

        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings.FindAsync(id);
            if (id==null)
            {
                return NotFound();
            }

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,Bookings booking)
        {
            if (id!=booking.bookingsID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!BookingsExists(booking.bookingsID))
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

            return View(booking);
        }



    }
}
