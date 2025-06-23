namespace EventEase.Models
{
    public class Bookings
    {
        //in correlation with database fields
        public int bookingsID { get; set; }
        public int eventsID { get; set; }
        public Events? Events { get; set; }
        public int venueID { get; set; }
        public Venue? Venue { get; set; }
        public DateTime bookingDate { get; set; }
        public int? eventsTypeID { get; set; }
        public EventsType? EventsType { get; set; }
    }
}
