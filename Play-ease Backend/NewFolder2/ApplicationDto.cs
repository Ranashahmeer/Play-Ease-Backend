using System.ComponentModel.DataAnnotations;
namespace Play_ease_Backend.NewFolder2
{
    public class ApplicationDto
    {
            [Required]
            public int MatchId { get; set; }

            [Required]
            public int UserId { get; set; }

            [Required]
            public string UserName { get; set; }

            [Required]
            public string Role { get; set; }
    }
}
