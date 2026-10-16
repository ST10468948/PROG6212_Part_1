using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Filters;
using RaceDay.Api.Models;
using RaceDay.Contracts;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/events/{eventId}/categories
        [HttpGet("events/{eventId}/categories")]
        public async Task<IActionResult> GetCategoriesForEvent(int eventId)
        {
            List<Category> categories = await _context.Categories
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            List<CategoryDto> categoryDtos = new List<CategoryDto>();
            foreach (Category item in categories)
            {
                categoryDtos.Add(new CategoryDto
                {
                    CategoryID = item.CategoryID,
                    EventID = item.EventID,
                    CategoryName = item.CategoryName,
                    Description = item.Description
                });
            }

            return Ok(categoryDtos);
        }

        // POST: api/events/{eventId}/categories (Organiser only)
        [HttpPost("events/{eventId}/categories")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> CreateCategory(int eventId, [FromBody] CreateCategoryRequest request)
        {
            int organiserId = HttpContext.Session.GetInt32("UserId").Value;

            Event existingEvent = await _context.Events.FindAsync(eventId);
            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (existingEvent.OrganiserID != organiserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not own this event." });
            }

            Category newCategory = new Category
            {
                EventID = eventId,
                CategoryName = request.CategoryName,
                Description = request.Description
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            CategoryDto dto = new CategoryDto
            {
                CategoryID = newCategory.CategoryID,
                EventID = newCategory.EventID,
                CategoryName = newCategory.CategoryName,
                Description = newCategory.Description
            };

            return StatusCode(StatusCodes.Status201Created, dto);
        }
    }
}