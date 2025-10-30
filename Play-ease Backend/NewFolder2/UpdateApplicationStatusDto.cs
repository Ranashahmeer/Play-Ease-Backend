using System.ComponentModel.DataAnnotations;

namespace Play_ease_Backend.NewFolder2
{
    public class UpdateApplicantStatusDto
    {
        [Required]
        public string Status { get; set; } // "accepted" or "rejected"
    }
}