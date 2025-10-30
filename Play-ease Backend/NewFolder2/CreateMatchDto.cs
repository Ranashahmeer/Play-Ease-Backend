using System;
using System.ComponentModel.DataAnnotations;

namespace Play_ease_Backend.NewFolder2
{
    public class CreateMatchDto
    {
        [Required]
        public int UserId { get; set; }  // Temporary - will be removed when you implement auth

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string StartTime { get; set; }

        [Required]
        public string EndTime { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Roles { get; set; }

        [Required]
        public int NumPlayers { get; set; }

        [Required]
        public decimal Price { get; set; }

        // Note: No Organizer or OrganizerId - these will be auto-filled!
    }
}