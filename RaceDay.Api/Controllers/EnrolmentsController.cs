using System;
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
    [Route("api/[controller]")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/enrolments (Participant only)
        [HttpPost]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> EnrolInEvent([FromBody] CreateEnrolmentRequest request)
        {
            int participantId = HttpContext.Session.GetInt32("UserId").Value;

            // Verifies that the event exists
            Event evt = await _context.Events.FindAsync(request.EventID);
            if (evt == null)
            {
                return NotFound(new { message = "Event does not exist." });
            }

            // Verifies that the category belongs to this event
            Category cat = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryID == request.CategoryID && c.EventID == request.EventID);
            if (cat == null)
            {
                return BadRequest(new { message = "Category does not belong to this event." });
            }

            // Prevents duplicate enrolments
            bool alreadyEnrolled = await _context.Enrolments.AnyAsync(e => e.ParticipantID == participantId && e.EventID == request.EventID);
            if (alreadyEnrolled)
            {
                return Conflict(new { message = "You are already enrolled in this event." });
            }

            Enrolment newEnrolment = new Enrolment
            {
                ParticipantID = participantId,
                EventID = request.EventID,
                CategoryID = request.CategoryID,
                EnrolmentDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            _context.Enrolments.Add(newEnrolment);
            await _context.SaveChangesAsync();

            AppUser participant = await _context.Users.FindAsync(participantId);

            EnrolmentDto dto = new EnrolmentDto
            {
                EnrolmentID = newEnrolment.EnrolmentID,
                ParticipantID = participantId,
                ParticipantName = participant.FullName,
                EventID = evt.EventID,
                EventName = evt.EventName,
                CategoryID = cat.CategoryID,
                CategoryName = cat.CategoryName,
                EnrolmentDate = newEnrolment.EnrolmentDate,
                Status = newEnrolment.Status
            };

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        // GET: api/enrolments/mine (Participant only)
        [HttpGet("mine")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            int participantId = HttpContext.Session.GetInt32("UserId").Value;

            List<Enrolment> enrolments = await _context.Enrolments
                .Include(e => e.Event)
                .Include(e => e.Category)
                .Include(e => e.Participant)
                .Where(e => e.ParticipantID == participantId)
                .ToListAsync();

            List<EnrolmentDto> dtos = new List<EnrolmentDto>();
            foreach (Enrolment item in enrolments)
            {
                dtos.Add(new EnrolmentDto
                {
                    EnrolmentID = item.EnrolmentID,
                    ParticipantID = item.ParticipantID,
                    ParticipantName = item.Participant.FullName,
                    EventID = item.EventID,
                    EventName = item.Event.EventName,
                    CategoryID = item.CategoryID,
                    CategoryName = item.Category.CategoryName,
                    EnrolmentDate = item.EnrolmentDate,
                    Status = item.Status
                });
            }

            return Ok(dtos);
        }

        // GET: api/enrolments/event/{eventId} (Organiser only)
        [HttpGet("event/{eventId}")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> GetEventEnrolments(int eventId)
        {
            int organiserId = HttpContext.Session.GetInt32("UserId").Value;

            Event evt = await _context.Events.FindAsync(eventId);
            if (evt == null)
            {
                return NotFound(new { message = "Event not found." });
            }

            if (evt.OrganiserID != organiserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only view enrolments for events you created." });
            }

            List<Enrolment> enrolments = await _context.Enrolments
                .Include(e => e.Participant)
                .Include(e => e.Category)
                .Where(e => e.EventID == eventId)
                .ToListAsync();

            List<EnrolmentDto> dtos = new List<EnrolmentDto>();
            foreach (Enrolment item in enrolments)
            {
                dtos.Add(new EnrolmentDto
                {
                    EnrolmentID = item.EnrolmentID,
                    ParticipantID = item.ParticipantID,
                    ParticipantName = item.Participant.FullName,
                    EventID = evt.EventID,
                    EventName = evt.EventName,
                    CategoryID = item.CategoryID,
                    CategoryName = item.Category.CategoryName,
                    EnrolmentDate = item.EnrolmentDate,
                    Status = item.Status
                });
            }

            return Ok(dtos);
        }
    }
}