using System.ComponentModel.DataAnnotations.Schema;

namespace Play_ease_Backend.NewFolder
{
    [Table("chat_messages")]
    public class ChatMessage
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public DateTime? CreatedAt { get; set; }
    }

    public class SendMessageDto
    {
        public int MatchId { get; set; }
        public int SenderId { get; set; }
        public string? SenderName { get; set; }
        public int? ReceiverId { get; set; }
        public string? ReceiverName { get; set; }
        public string Message { get; set; }
        public string? Timestamp { get; set; }
    }
}

