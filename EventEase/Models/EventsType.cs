using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventsType
    {
        //in correlation with database fields
       public int? eventsTypeID { get; set; }
       public  string? eventsTypeName { get; set; }
    }
}
