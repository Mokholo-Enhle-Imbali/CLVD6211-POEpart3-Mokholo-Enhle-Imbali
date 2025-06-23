using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net;

namespace EventEase.Controllers
{
    public class VenueController : Controller
    {
        private readonly ApplicationDBContext _context; //readonly field
        
        public VenueController(ApplicationDBContext context) //parameterised contructor 
        {
            _context = context;
        }

        public async Task<IActionResult> Index() //asynchronous task method
        {
            var venues = await _context.Venue.ToListAsync();
            return View(venues);
        }

        

        public IActionResult CreateVenue()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVenue(Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile!=null)
                {
                    var blobUrl =await UploadImageToBlobAsync(venue.ImageFile);
                    venue.Imageurl = blobUrl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Venue Created Successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

      

        public async Task<IActionResult> VenueDetails(int id)
        {
            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.venueId == id);

            if (venue==null)
            {
                return NotFound();
            }

            return View(venue);
        }

        public async Task<IActionResult> DeleteVenue(int id)
        {
            var venue = await _context.Venue.FirstOrDefaultAsync(m => m.venueId == id);

            if (venue==null)
            {
                return NotFound();
            }

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id)
        {

            var venue = await _context.Venue.FindAsync(id);
            if (venue==null)
            {
                return NotFound();
            }

            var hasBookings = await _context.Bookings.AnyAsync(b => b.venueID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Error: Cannot delete venue because it is associated with existing bookings";
                return RedirectToAction(nameof(Index));
            }
            _context.Venue.Remove(venue);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue has been deleted Successfully!";
            return RedirectToAction(nameof(Index));

        }

        

        public async Task<IActionResult> EditVenue(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var venue = await _context.Venue.FindAsync(id);

            if (id==null)
            {
                return NotFound();
            }

            return View(venue);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> EditVenue(int id, Venue venue)
        {
            if (id!=venue.venueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);
                        venue.Imageurl = blobUrl;
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.venueId))
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




        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=eventblob123;AccountKey=JmadNeIznfk5mrQkjtyTm48cp1scZVwfwZCkmSvTnnRRaDuwiRc+eySy9kte36QBR62fqgCfEU8F+AStB94rVA==;EndpointSuffix=core.windows.net";
            var containerName = "evetease123";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }


        private bool VenueExists(int id)
        {
            return _context.Venue.Any(e => e.venueId == id);
        }
    }
}
