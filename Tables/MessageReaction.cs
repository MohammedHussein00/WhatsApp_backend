namespace WhatsApp_Clone.Tables
{
    public class MessageReaction
    {
        public int Id { get; set; }
        public string Reaction { get; set; }
        public int MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
