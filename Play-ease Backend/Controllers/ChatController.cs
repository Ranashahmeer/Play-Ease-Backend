using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Play_ease_Backend.NewFolder;
using System;

namespace Play_ease_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Chat/send
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.Message))
                {
                    return BadRequest(new { message = "Message is required." });
                }

                if (dto.MatchId <= 0 || dto.SenderId <= 0)
                {
                    return BadRequest(new { message = "MatchId and SenderId are required." });
                }

                // Verify that the match exists
                var match = await _context.PlayerRequests.FindAsync(dto.MatchId);
                if (match == null)
                {
                    return NotFound(new { message = "Match not found." });
                }

                // Validate that sender and receiver are different
                if (dto.ReceiverId.HasValue && dto.ReceiverId.Value > 0)
                {
                    if (dto.SenderId == dto.ReceiverId.Value)
                    {
                        return BadRequest(new { message = "Sender and receiver cannot be the same person." });
                    }
                }

                // Derive receiver information if not provided
                int receiverId;
                string receiverName;

                if (dto.ReceiverId.HasValue && dto.ReceiverId.Value > 0)
                {
                    // Use provided receiver info
                    receiverId = dto.ReceiverId.Value;
                    
                    // Get receiver name if not provided
                    if (!string.IsNullOrEmpty(dto.ReceiverName))
                    {
                        receiverName = dto.ReceiverName;
                    }
                    else
                    {
                        // Look up receiver name from database
                        if (receiverId == match.OrganizerId)
                        {
                            receiverName = match.Organizer;
                        }
                        else
                        {
                            var receiverApplicant = await _context.MatchApplicants
                                .FirstOrDefaultAsync(a => a.MatchId == dto.MatchId && a.UserId == receiverId);
                            receiverName = receiverApplicant?.UserName ?? "Unknown";
                        }
                    }
                }
                else
                {
                    // Derive receiver from match context
                    // If sender is the organizer, we need receiverId (can't determine which applicant)
                    // If sender is an applicant, receiver is always the organizer
                    if (dto.SenderId == match.OrganizerId)
                    {
                        // Sender is organizer - receiverId is required (could be multiple applicants)
                        return BadRequest(new { message = "ReceiverId is required when organizer sends a message. Please specify which applicant you are messaging." });
                    }
                    else
                    {
                        // Sender is an applicant - receiver is always the organizer
                        receiverId = match.OrganizerId;
                        receiverName = match.Organizer;
                    }
                }

                // Get sender name if not provided
                string senderName = dto.SenderName;
                if (string.IsNullOrEmpty(senderName))
                {
                    if (dto.SenderId == match.OrganizerId)
                    {
                        senderName = match.Organizer;
                    }
                    else
                    {
                        var senderApplicant = await _context.MatchApplicants
                            .FirstOrDefaultAsync(a => a.MatchId == dto.MatchId && a.UserId == dto.SenderId);
                        senderName = senderApplicant?.UserName ?? "Unknown";
                    }
                }

                // Validate that all required fields are set
                if (string.IsNullOrEmpty(senderName))
                {
                    return BadRequest(new { message = "Sender name could not be determined." });
                }

                if (string.IsNullOrEmpty(receiverName))
                {
                    return BadRequest(new { message = "Receiver name could not be determined." });
                }

                if (string.IsNullOrEmpty(dto.Message))
                {
                    return BadRequest(new { message = "Message cannot be empty." });
                }

                var chatMessage = new ChatMessage
                {
                    MatchId = dto.MatchId,
                    SenderId = dto.SenderId,
                    SenderName = senderName ?? "Unknown",
                    ReceiverId = receiverId,
                    ReceiverName = receiverName ?? "Unknown",
                    Message = dto.Message,
                    Timestamp = !string.IsNullOrEmpty(dto.Timestamp) && DateTime.TryParse(dto.Timestamp, out var parsedTime)
                        ? parsedTime.ToUniversalTime()
                        : DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    _context.ChatMessages.Add(chatMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"Database error in SendMessage: {dbEx.Message}");
                    Console.WriteLine($"Inner exception: {dbEx.InnerException?.Message}");
                    return StatusCode(500, new { message = "Database error occurred while saving the message.", error = dbEx.InnerException?.Message ?? dbEx.Message });
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"Save error in SendMessage: {saveEx.Message}");
                    Console.WriteLine($"Stack trace: {saveEx.StackTrace}");
                    return StatusCode(500, new { message = "Error saving message to database.", error = saveEx.Message });
                }

                return Ok(new
                {
                    id = chatMessage.Id,
                    matchId = chatMessage.MatchId,
                    senderId = chatMessage.SenderId,
                    senderName = chatMessage.SenderName,
                    receiverId = chatMessage.ReceiverId,
                    receiverName = chatMessage.ReceiverName,
                    message = chatMessage.Message,
                    timestamp = chatMessage.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    createdAt = chatMessage.CreatedAt?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use ILogger here if needed)
                Console.WriteLine($"Error in SendMessage: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "An error occurred while sending the message.", error = ex.Message });
            }
        }

        // GET: api/Chat/messages/{matchId}/{userId1}/{userId2} - Route parameters
        [HttpGet("messages/{matchId}/{userId1}/{userId2}")]
        public async Task<IActionResult> GetMessagesByRoute(int matchId, int userId1, int userId2)
        {
            if (matchId <= 0 || userId1 <= 0 || userId2 <= 0)
            {
                return BadRequest(new { message = "MatchId, UserId1, and UserId2 are required." });
            }

            return await GetMessagesInternal(matchId, userId1, userId2);
        }

        // GET: api/Chat/messages/{matchId}?userId1={id}&userId2={id} - Query parameters (for frontend compatibility)
        [HttpGet("messages/{matchId}")]
        public async Task<IActionResult> GetMessagesByQuery(int matchId, [FromQuery] int? userId1, [FromQuery] int? userId2)
        {
            if (matchId <= 0)
            {
                return BadRequest(new { message = "MatchId is required." });
            }

            if (!userId1.HasValue || userId1.Value <= 0)
            {
                return BadRequest(new { message = "userId1 query parameter is required and must be greater than 0." });
            }

            if (!userId2.HasValue || userId2.Value <= 0)
            {
                return BadRequest(new { message = "userId2 query parameter is required and must be greater than 0." });
            }

            return await GetMessagesInternal(matchId, userId1.Value, userId2.Value);
        }

        // Internal method to get messages (shared by both endpoints)
        private async Task<IActionResult> GetMessagesInternal(int matchId, int userId1, int userId2)
        {
            // Get messages where sender and receiver are either userId1 or userId2
            var messages = await _context.ChatMessages
                .Where(m => m.MatchId == matchId &&
                           ((m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1)))
                .OrderBy(m => m.Timestamp)
                .Select(m => new
                {
                    id = m.Id,
                    matchId = m.MatchId,
                    senderId = m.SenderId,
                    senderName = m.SenderName,
                    receiverId = m.ReceiverId,
                    receiverName = m.ReceiverName,
                    message = m.Message,
                    timestamp = m.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    createdAt = m.CreatedAt.HasValue ? m.CreatedAt.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") : null
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}

