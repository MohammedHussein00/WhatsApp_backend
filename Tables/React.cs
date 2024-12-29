namespace WhatsApp_Clone.Tables
{
    public class React
    {
        public int Id { get; set; }
        public string ReactValue { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public int MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
