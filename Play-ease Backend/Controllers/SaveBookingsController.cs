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
                               (CourtID, OwnerID, UserID, PaymentMethodID, PaymentProof, BookingDate, StartTime, EndTime, Price, Status)
                               VALUES (@CourtId, @OwnerId, @UserId, @PaymentMethodId, @PaymentProof, @BookingDate, @StartTime, @EndTime, @Price, 'Pending')";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CourtId", dto.CourtId);
                        cmd.Parameters.AddWithValue("@OwnerId", dto.OwnerId);
                        cmd.Parameters.AddWithValue("@UserId", dto.UserId);
                        cmd.Parameters.AddWithValue("@PaymentMethodId", dto.PaymentMethodId);
                        cmd.Parameters.AddWithValue("@PaymentProof", dto.PaymentProof ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BookingDate", dto.BookingDate);
                        cmd.Parameters.AddWithValue("@StartTime", dto.StartTime);
                        cmd.Parameters.AddWithValue("@EndTime", dto.EndTime);
                        cmd.Parameters.AddWithValue("@Price", dto.Price);

                        cmd.ExecuteNonQuery(); // ✅ Correct method
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
