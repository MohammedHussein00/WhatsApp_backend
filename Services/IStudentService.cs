using WhatsApp_Clone.Tables;
using WhatsApp_Clone;
using Music_Aditor.Models;

namespace WhatsApp_Clone.Services
{
    public interface IStudentService
    {
        Task<ChatMessageDto> SendMessage(MessageDto message);
        Task<bool> SendEmailMessageViaGmail(string userName, string userEmail, string subject, string textContent);
    }
}
