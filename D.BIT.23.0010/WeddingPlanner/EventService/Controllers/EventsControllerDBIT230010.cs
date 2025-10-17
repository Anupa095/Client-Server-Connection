using Microsoft.AspNetCore.Mvc;
using EventService.Data;
using EventService.Models;

namespace EventService.Controllers
{
    [ApiController]
    [Route("events")]
    public class EventsControllerDBIT230010 : ControllerBase
    {
        private readonly EventDbContextDBIT230010 _context;

        public EventsControllerDBIT230010(EventDbContextDBIT230010 context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetEvent(int id)
        {
            var ev = _context.Events.Find(id);
            return ev == null ? NotFound() : Ok(ev);
        }

        [HttpPost]
        public IActionResult CreateEvent(EventDBIT230010 ev)
        {
            _context.Events.Add(ev);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetEvent), new { id = ev.EventId }, ev);
        }
    }
}
