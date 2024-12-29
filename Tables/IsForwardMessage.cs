namespace WhatsApp_Clone.Tables
{
    public class IsForwardMessage
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public Message? Message { get; set; }
    }
}
