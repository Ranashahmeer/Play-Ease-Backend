namespace Play_ease_Backend.NewFolder
{
    public class PlayEaseModelClass
    {
        public class BookingDto
        {
            public int CourtId { get; set; }
            public int OwnerId { get; set; }
            public int UserId { get; set; }
            public int PaymentMethodId { get; set; }
            public string PaymentProof { get; set; }
            public DateTime BookingDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public decimal Price { get; set; }
        }

    }
}
