namespace Play_ease_Backend.NewFolder
{
    public class MatchApplicant
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; } = "pending";
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }
    }

}
