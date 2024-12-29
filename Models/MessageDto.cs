using WhatsApp_Clone.Tables;

namespace Music_Aditor.Models
{
    public class MessageDto
    {

        public string? Content { get; set; }
        public ContentType MessageType { get; set; }
        public IFormFile? File { get; set; }

        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public bool IsForward { get; set; }
        public int ReplayMessageId { get; set; }


    }

}
