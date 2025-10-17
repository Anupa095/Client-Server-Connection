using Microsoft.AspNetCore.Mvc;
using TaskService.Data;
using TaskService.Models;
using System.Net.Http.Json;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TasksControllerDBIT230010 : ControllerBase
    {
        private readonly TaskDbContextDBIT230010 _context;
        private readonly HttpClient _http;

        public TasksControllerDBIT230010(TaskDbContextDBIT230010 context, IHttpClientFactory factory)
        {
            _context = context;
            _http = factory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> AddTaskDBIT230010(TaskDBIT230010 task)
        {
            var ev = await _http.GetAsync($"http://localhost:5001/events/{task.EventId}");
            if (!ev.IsSuccessStatusCode)
                return BadRequest("Invalid EventId");

            _context.Tasks.Add(task);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetTaskDBIT230010), new { id = task.TaskId }, task);
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskDBIT230010(int id)
        {
            var t = _context.Tasks.Find(id);
            return t == null ? NotFound() : Ok(t);
        }

        [HttpGet]
        public IActionResult GetTasksByEventDBIT230010([FromQuery] int eventId)
        {
            var list = _context.Tasks.Where(t => t.EventId == eventId).ToList();
            return Ok(list);
        }
    }
}
