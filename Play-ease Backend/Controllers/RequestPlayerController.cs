using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder;   // DTO

namespace Play_ease_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestPlayerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestPlayerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<PlayerRequest>> CreateMatch([FromBody] PlayerRequest matchDto)
        {
            //var user = await _context.Users.FindAsync(matchDto.UserId);

            //if (user == null)
            //    return BadRequest(new { message = "User not found" });

            var matchRequest = new PlayerRequest
            {
                Title = matchDto.Title,
                Date = matchDto.Date,
                StartTime = matchDto.StartTime,
                EndTime = matchDto.EndTime,
                Location = matchDto.Location,
                Roles = matchDto.Roles,
                NumPlayers = matchDto.NumPlayers,
                Price = matchDto.Price,
                Organizer = matchDto.Organizer,
                OrganizerId = matchDto.OrganizerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.PlayerRequests.Add(matchRequest);
            await _context.SaveChangesAsync();

            //return CreatedAtAction(nameof(GetMatchById), new { id = matchRequest.Id }, matchRequest);
            return Ok(new
            {
                message = "Match request created successfully!",
                matchId = matchRequest.Id 
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerRequest>> GetMatchById(int id)
        {
            var match = await _context.PlayerRequests.FindAsync(id);

            if (match == null)
                return NotFound();

            return match;
        }
    }
}
