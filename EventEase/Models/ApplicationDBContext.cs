using Microsoft.EntityFrameworkCore;

namespace EventEase.Models
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Events> Events { get; set; }
        public DbSet<EventsType> EventsType { get; set; }
        public DbSet<Venue> Venue { get; set; }
        public DbSet<Bookings> Bookings { get; set; }
        


    }
}
