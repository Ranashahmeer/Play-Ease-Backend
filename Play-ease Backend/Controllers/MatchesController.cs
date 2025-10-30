using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder1;
using Play_ease_Backend.NewFolder2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play_ease_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MatchesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/matches
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchRequest>>> GetAllMatches()
        {
            try
            {
                var matches = await _context.MatchRequests
                    .AsNoTracking()  // Add this
                    .Where(m => m.Date >= DateTime.Today)
                    .OrderBy(m => m.Date)
                    .ToListAsync();

                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving matches", error = ex.Message });
            }
        }
        // GET: api/matches/available/{userId}
        [HttpGet("available/{userId}")]
        public async Task<ActionResult<IEnumerable<MatchRequest>>> GetAvailableMatches(int userId)
        {
            try
            {
                var matches = await _context.MatchRequests
                    .Where(m => m.OrganizerId != userId && m.Date >= DateTime.Today)
                    .OrderBy(m => m.Date)
                    .ToListAsync();

                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving available matches", error = ex.Message });
            }
        }

        [HttpGet("my/{userId}")]
        public async Task<ActionResult<IEnumerable<MatchRequest>>> GetMyMatches(int userId)
        {
            try
            {
                var matches = await _context.MatchRequests
                    .Where(m => m.OrganizerId == userId && m.Date >= DateTime.Today)
                    .OrderBy(m => m.Date)
                    .ToListAsync();

                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user matches", error = ex.Message });
            }
        }

    
        // POST: api/matches
        [HttpPost]
        public async Task<ActionResult<MatchRequest>> CreateMatch([FromBody] CreateMatchDto matchDto)
        {
            try
            {
                // TODO: Get userId from authentication token
                // For now, you need to pass it in the request or get it from claims
                int userId = matchDto.UserId; // Temporary - you'll get this from auth token

                // Fetch user details from database
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return BadRequest(new { message = "User not found" });
                }

                // Create match request with automatic organizer info
                var matchRequest = new MatchRequest
                {
                    Title = matchDto.Title,
                    Date = matchDto.Date,
                    StartTime = matchDto.StartTime,
                    EndTime = matchDto.EndTime,
                    Location = matchDto.Location,
                    Roles = matchDto.Roles,
                    NumPlayers = matchDto.NumPlayers,
                    Price = matchDto.Price,
                    Organizer = user.FullName,      // ✅ Auto-filled from logged-in user
                    OrganizerId = user.UserID,      // ✅ Auto-filled from logged-in user
                    CreatedAt = DateTime.Now
                };

                Console.WriteLine($"Creating match for organizer: {matchRequest.Organizer} (ID: {matchRequest.OrganizerId})");

                _context.MatchRequests.Add(matchRequest);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Match created with ID: {matchRequest.Id}");

                return CreatedAtAction(nameof(GetMatchById), new { id = matchRequest.Id }, matchRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return StatusCode(500, new { message = "Error creating match", error = ex.Message });
            }
        }
        // GET: api/matches/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<MatchRequest>> GetMatchById(int id)
        {
            try
            {
                var match = await _context.MatchRequests.FindAsync(id);

                if (match == null)
                {
                    return NotFound(new { message = "Match not found" });
                }

                return Ok(match);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving match", error = ex.Message });
            }
        }

        // GET: api/matches/{matchId}/applicants
        [HttpGet("{matchId}/applicants")]
        public async Task<ActionResult<IEnumerable<Applicant>>> GetApplicants(int matchId)
        {
            try
            {
                var applicants = await _context.Applicants
                    .Where(a => a.MatchId == matchId)
                    .OrderBy(a => a.AppliedAt)
                    .ToListAsync();

                return Ok(applicants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving applicants", error = ex.Message });
            }
        }
        
        // POST: api/matches/apply
        [HttpPost("apply")]
        public async Task<ActionResult> ApplyToMatch([FromBody] ApplicationDto application)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if match exists
                var match = await _context.MatchRequests.FindAsync(application.MatchId);
                if (match == null)
                {
                    return NotFound(new { message = "Match not found" });
                }

                // Check if user already applied
                var existingApplication = await _context.Applicants
                    .FirstOrDefaultAsync(a => a.MatchId == application.MatchId && a.UserId == application.UserId);

                if (existingApplication != null)
                {
                    return BadRequest(new { message = "You have already applied to this match" });
                }

                // Create new applicant
                var applicant = new Applicant
                {
                    MatchId = application.MatchId,
                    UserId = application.UserId,
                    UserName = application.UserName,
                    Role = application.Role,
                    Status = "pending",
                    AppliedAt = DateTime.Now
                };

                _context.Applicants.Add(applicant);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Application submitted successfully", applicantId = applicant.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error submitting application", error = ex.Message });
            }
        }

        // PUT: api/matches/{matchId}/applicants/{applicantId}
        [HttpPut("{matchId}/applicants/{applicantId}")]
        public async Task<ActionResult> UpdateApplicantStatus(
            int matchId,
            int applicantId,
            [FromBody] UpdateApplicantStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var applicant = await _context.Applicants
                    .FirstOrDefaultAsync(a => a.Id == applicantId && a.MatchId == matchId);

                if (applicant == null)
                {
                    return NotFound(new { message = "Applicant not found" });
                }

                applicant.Status = statusDto.Status;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Applicant status updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating applicant status", error = ex.Message });
            }
        }

        // DELETE: api/matches/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMatch(int id)
        {
            try
            {
                var match = await _context.MatchRequests.FindAsync(id);

                if (match == null)
                {
                    return NotFound(new { message = "Match not found" });
                }

                _context.MatchRequests.Remove(match);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Match deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting match", error = ex.Message });
            }
        }
    }
}