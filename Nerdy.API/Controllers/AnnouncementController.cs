using Microsoft.AspNetCore.Mvc;
using Nerdy.Domain.Interfaces;
using Nerdy.Domain.Models;

namespace Nerdy.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _service;

        public AnnouncementController(IAnnouncementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var announcement = await _service.GetByIdAsync(id);
            if (announcement == null) return NotFound();

            var similar = await _service.GetSimilarAsync(announcement);
            return Ok(new
            {
                announcement.Id,
                announcement.Title,
                announcement.Description,
                announcement.DateAdded,
                Similar = similar.Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.DateAdded
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Announcement announcement)
        {
            if (announcement == null || string.IsNullOrWhiteSpace(announcement.Title) || string.IsNullOrWhiteSpace(announcement.Description))
            {
                return BadRequest("Invalid announcement data.");
            }
            await _service.AddAsync(announcement);
            return CreatedAtAction(nameof(GetById), new { id = announcement.Id }, announcement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update (int id, [FromBody] Announcement updated)
        {
            if (updated == null || string.IsNullOrWhiteSpace(updated.Title) || string.IsNullOrWhiteSpace(updated.Description))
            {
                return BadRequest("Invalid announcement data.");
            }
            updated.Id = id;
            var success = await _service.UpdateAsync(updated);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
