using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Venue
    {
        //in correlation with database fields
        public int venueId { get; set; }
        public string venueName { get; set; }
        public string location { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
        public string capacity { get; set; }
        public string? Imageurl { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
