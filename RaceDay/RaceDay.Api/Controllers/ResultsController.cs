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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // POST: api/results (Organiser only)
        [HttpPost]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> CaptureResult([FromBody] CreateResultRequest request)
        {
            int organiserId = HttpContext.Session.GetInt32("UserId").Value;

            // Locates the enrolment record
            Enrolment enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentID == request.EnrolmentID);

            if (enrolment == null)
            {
                return NotFound(new { message = "Enrolment record not found." });
            }

            // Verifies that this organiser created the event
            if (enrolment.Event.OrganiserID != organiserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "You can only capture results for events you organised." });
            }

            // Validates positions
            if (request.Position > request.TotalFinishers)
            {
                return BadRequest(new { message = "Finishing position cannot exceed total finishers." });
            }

            // Ensures a result isn't captured twice for the same enrolment
            bool alreadyExists = await _context.Results.AnyAsync(r => r.EnrolmentID == request.EnrolmentID);
            if (alreadyExists)
            {
                return Conflict(new { message = "A result has already been recorded for this enrolment." });
            }

            Result newResult = new Result
            {
                EnrolmentID = request.EnrolmentID,
                ParticipantID = request.ParticipantID,
                FinishTime = request.FinishTime,
                Position = request.Position,
                TotalFinishers = request.TotalFinishers,
                RecordedDate = DateTime.UtcNow
            };

            _context.Results.Add(newResult);
            await _context.SaveChangesAsync();

            ResultDto dto = new ResultDto
            {
                ResultID = newResult.ResultID,
                EnrolmentID = newResult.EnrolmentID,
                ParticipantID = newResult.ParticipantID,
                FinishTime = newResult.FinishTime,
                Position = newResult.Position,
                TotalFinishers = newResult.TotalFinishers,
                RecordedDate = newResult.RecordedDate
            };

            return StatusCode(StatusCodes.Status201Created, dto);
        }

        // GET: api/results/mine (Participant only)
        [HttpGet("mine")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            int participantId = HttpContext.Session.GetInt32("UserId").Value;

            List<Result> results = await _context.Results
                .Include(r => r.Enrolment).ThenInclude(e => e.Event)
                .Include(r => r.Enrolment).ThenInclude(e => e.Category)
                .Include(r => r.Participant)
                .Where(r => r.ParticipantID == participantId)
                .ToListAsync();

            List<ResultDto> dtos = new List<ResultDto>();
            foreach (Result r in results)
            {
                dtos.Add(new ResultDto
                {
                    ResultID = r.ResultID,
                    EnrolmentID = r.EnrolmentID,
                    ParticipantID = r.ParticipantID,
                    ParticipantName = r.Participant.FullName,
                    EventName = r.Enrolment.Event.EventName,
                    CategoryName = r.Enrolment.Category.CategoryName,
                    FinishTime = r.FinishTime,
                    Position = r.Position,
                    TotalFinishers = r.TotalFinishers,
                    RecordedDate = r.RecordedDate
                });
            }

            return Ok(dtos);
        }
    }
}