using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder;

namespace Play_ease_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApplicantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> ApplyToMatch([FromBody] ApplyDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request.");

            var alreadyApplied = await _context.MatchApplicants
                .AnyAsync(a => a.MatchId == dto.MatchId && a.UserId == dto.UserId);

            if (alreadyApplied)
                return BadRequest(new { message = "User already applied to this match." });

            var applicant = new MatchApplicant
            {
                MatchId = dto.MatchId,
                UserId = dto.UserId,
                UserName = dto.UserName,
                Role = dto.Role,
                Status = "pending",
                AppliedAt = DateTime.UtcNow
            };

            _context.MatchApplicants.Add(applicant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Application submitted successfully!" });
        }



        // ✅ UPDATE STATUS (accept / reject)
        [HttpPut("{matchId}/applicants/{applicantId}")]
        public async Task<IActionResult> UpdateStatus(int matchId, int applicantId, [FromBody] dynamic obj)
        {
            string status = obj.status;

            var applicant = await _context.MatchApplicants
                .FirstOrDefaultAsync(a => a.Id == applicantId && a.MatchId == matchId);

            if (applicant == null)
                return NotFound(new { message = "Applicant not found." });

            applicant.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Applicant status updated to {status}"
            });
        }
    }
}
