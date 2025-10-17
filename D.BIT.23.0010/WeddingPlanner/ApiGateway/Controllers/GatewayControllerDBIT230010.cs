using Microsoft.AspNetCore.Mvc;
using ApiGateway.Models;
using System.Net.Http.Json;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("gateway")]
    public class GatewayControllerDBIT230010 : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GatewayControllerDBIT230010(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [HttpGet("summary/{eventId}")]
        public async Task<IActionResult> GetSummaryDBIT230010(int eventId)
        {
            var eventResponse = await _httpClient.GetAsync($"http://localhost:5001/events/{eventId}");
            if (!eventResponse.IsSuccessStatusCode) return NotFound("Event not found");

            var guestsResponse = await _httpClient.GetAsync($"http://localhost:5002/guests?eventId={eventId}");
            var tasksResponse = await _httpClient.GetAsync($"http://localhost:5003/tasks?eventId={eventId}");

            var eventData = await eventResponse.Content.ReadFromJsonAsync<object>();
            var guestsData = await guestsResponse.Content.ReadFromJsonAsync<List<object>>();
            var tasksData = await tasksResponse.Content.ReadFromJsonAsync<List<object>>();

            var summary = new EventSummaryDBIT230010
            {
                Event = eventData,
                Guests = guestsData,
                Tasks = tasksData
            };

            return Ok(summary);
        }
    }
}
