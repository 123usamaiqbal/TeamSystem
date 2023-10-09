namespace TeamManageSystem.Models.Account
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Sendername { get; set; }
        public string TextMessage { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
