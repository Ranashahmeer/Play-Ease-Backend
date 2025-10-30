using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace Play_ease_Backend.NewFolder1
{
    [Table("applicants")]
    public class Applicant
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("match_id")]
        public int MatchId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        [Column("role")]
        public string Role { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("applied_at")]
        public DateTime AppliedAt { get; set; }
        
        [JsonIgnore]
        public MatchRequest Match { get; set; }
    }

}