using WhatsApp_Clone.Tables;

namespace Music_Aditor.Tables
{
    public class MessageGroup
    {
        public int MessageId { get; set; }
        public Message? Message { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
