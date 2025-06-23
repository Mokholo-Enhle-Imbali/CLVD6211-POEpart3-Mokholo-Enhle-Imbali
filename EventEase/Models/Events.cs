namespace EventEase.Models
{
    public class Events
    {
        //in correlation with database fields
        public int eventsID { get; set; }
        public string eventName { get; set; }
        public DateTime eventDate { get; set; }
        public string description { get; set; }
        public int venueID { get; set; }
        public Venue? Venue { get; set; }
        public int? eventsTypeID { get; set; }
        public EventsType? EventsType { get; set; }
    }
}
