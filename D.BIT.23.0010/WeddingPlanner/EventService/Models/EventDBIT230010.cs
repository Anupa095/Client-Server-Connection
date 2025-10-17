using System.ComponentModel.DataAnnotations;

namespace EventService.Models
{
    public class EventDBIT230010
    {
        [Key]  // <-- add this
        public int EventId { get; set; }
        public string? Title { get; set; }
        public DateTime Date { get; set; }
        public string? Venue { get; set; }
    }
}
