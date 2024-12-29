using Music_Aditor.Tables;
using WhatsApp_Clone.Tables;

namespace WhatsApp_Clone.Tables
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public ContentType ContentType { get; set; }
        public DateTime Time { get; set; }
        public string SenderId { get; set; }  // Can be PatientId, DoctorId, or AdminId
        public string ReceiverId { get; set; }  // DoctorId or AdminId, depending on sender type
        public StatusType Status { get; set; }
        public ICollection<IsRepliedMessage>? RepliedMessages { get; set; }  // Messages that reply to this message
        public ICollection<IsRepliedMessage>? AnsweredMessages { get; set; }  // Messages that reply to this message
        public ICollection<IsForwardMessage>? IsForwardMessages { get; set; }  // Messages that reply to this message
        public ICollection<MessageReaction>? MessageReactions { get; set; }  // Messages that reply to this message
        public ICollection<React>? Reacts { get; set; }  // Messages that reply to this message
        public ICollection<MessageGroup>? MessageGroups { get; set; }  // Messages that reply to this message


    }

    public enum ContentType
    {
        Text,
        File,
        Audio
    }    
    public enum StatusType
    {
        NotDelivered,
        Delivered,
        Readed
    }

}
