using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Play_ease_Backend.NewFolder1
{
    [Table("matches")]
    public class MatchRequest
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("start_time")]
        public string StartTime { get; set; }

        [Column("end_time")]
        public string EndTime { get; set; }

        [Column("location")]
        public string Location { get; set; }

        [Column("roles")]
        public string Roles { get; set; }

        [Column("num_players")]
        public int NumPlayers { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("organizer")]
        public string Organizer { get; set; }

        [Column("organizer_id")]
        [JsonPropertyName("organizerId")]
        public int OrganizerId { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();  // Initialize with empty list

    }

}
