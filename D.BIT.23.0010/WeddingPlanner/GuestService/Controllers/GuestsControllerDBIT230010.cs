using Microsoft.AspNetCore.Mvc;
using GuestService.Data;
using GuestService.Model;

namespace GuestService.Controllers
{
    [ApiController]
    [Route("guests")]
    public class GuestsControllerDBIT230010 : ControllerBase
    {
        private readonly GuestDbContextDBIT230010 _context;

        public GuestsControllerDBIT230010(GuestDbContextDBIT230010 context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddGuestDBIT230010(GuestDBIT230010 guest)
        {
            _context.Guests.Add(guest);
            _context.SaveChanges();
            return Ok(guest);
        }

        [HttpGet]
        public IActionResult GetGuestsDBIT230010([FromQuery] int eventId)
        {
            var guests = _context.Guests.Where(g => g.EventId == eventId).ToList();
            return Ok(guests);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGuestDBIT230010(int id, GuestDBIT230010 updatedGuest)
        {
            var guest = _context.Guests.Find(id);
            if (guest == null) return NotFound();

            guest.Name = updatedGuest.Name;
            guest.Email = updatedGuest.Email;
            guest.Rsvp = updatedGuest.Rsvp;

            _context.SaveChanges();
            return Ok(guest);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteGuestDBIT230010(int id)
        {
            var guest = _context.Guests.Find(id);
            if (guest == null) return NotFound();

            _context.Guests.Remove(guest);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
