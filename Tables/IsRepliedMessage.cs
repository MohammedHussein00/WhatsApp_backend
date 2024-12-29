namespace WhatsApp_Clone.Tables
{
    public class IsRepliedMessage
    {
        public int RepliedMessageId { get; set; }
        public Message? RepliedMessage { get; set; }
        public int AnsweredMessageId { get; set; }
        public Message? AnsweredMessage { get; set; }
    }
}
