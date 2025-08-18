using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Play_ease_Backend.NewFolder;

namespace Play_ease_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveBookingsController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public SaveBookingsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("create")]
        public IActionResult CreateBooking([FromBody] PlayEaseModelClass.BookingDto dto)
        {
            try
            {
                using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    conn.Open();

                    var sql = @"INSERT INTO Bookings
                               (CourtID, CourtPitchID, UserID, OwnerID, PaymentMethodID, PaymentProof, BookingDate, StartTime, EndTime, Price, PaymentStatus, CreatedAt)
                               VALUES (@CourtId, @CourtPitchID, @UserId, @OwnerId, @PaymentMethodId, @PaymentProof, @BookingDate, @StartTime, @EndTime, @Price, @PaymentStatus, @CreatedAt)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourtID", dto.CourtId);
                        cmd.Parameters.AddWithValue("@CourtPitchID", dto.CourtPitchId);
                        cmd.Parameters.AddWithValue("@UserId", dto.UserId); // ✅ hardcoded user
                        cmd.Parameters.AddWithValue("@OwnerId", dto.OwnerId);
                        cmd.Parameters.AddWithValue("@PaymentMethodId", dto.PaymentMethodId);
                        cmd.Parameters.AddWithValue("@PaymentProof", dto.PaymentProof ?? "dummy-proof.jpg");
                        cmd.Parameters.AddWithValue("@BookingDate", dto.BookingDate);
                        cmd.Parameters.AddWithValue("@StartTime", dto.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", dto.EndTime);
                        cmd.Parameters.AddWithValue("@Price", dto.Price);
                        cmd.Parameters.AddWithValue("@PaymentStatus", "Pending");
                        cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                return Ok(new { message = "Booking created successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
