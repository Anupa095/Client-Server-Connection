using System.ComponentModel.DataAnnotations;

namespace GuestService.Model
{
    public class GuestDBIT230010
    {
        [Key]  // 👈 This tells EF Core this is the primary key
        public int GuestId { get; set; }

        public int EventId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Rsvp { get; set; } = "Pending";
    }
}
