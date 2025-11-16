using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Play_ease_Backend.NewFolder
{
    [Table("matches")]
    public class PlayerRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
        public string Roles { get; set; }

        public int NumPlayers { get; set; }
        public decimal Price { get; set; }
        public string Organizer { get; set; }
        public int OrganizerId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
