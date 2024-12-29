using WhatsApp_Clone.Models;
using WhatsApp_Clone.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using WhatsApp_Clone;
using Music_Aditor.Models;
using Microsoft.AspNetCore.Authorization;

namespace WhatsApp_Clone.Services
{
    [Authorize]
    public class StudentService : IStudentService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _Context;
        private readonly IWebHostEnvironment _hostingEnvironment;



        public StudentService( IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager
            , ApplicationDbContext Context)
        {
            _Context = Context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
            _roleManager = roleManager;
        }




        public async Task<ChatMessageDto> SendMessage(MessageDto message)
        {
            // Retrieve admin's ID based on the email sent from Angular
            var user = _Context.Users.FirstOrDefault(a => a.Id == message.SenderId);

            string id = "";
            if (user is not null)
                id = user.Id;
            string filePath = "";

            if (message.File!=null)

             filePath = "test";

            // Create a new Chat object
            var chat = new Tables.Message
            {
                Content = string.IsNullOrEmpty(filePath) ? message.Content : filePath,
                Time = DateTime.Now, // Last day at 12:00 PM (midday)
                SenderId = id, // Assign admin's ID as sender ID
                ReceiverId = message.ReceiverId, // Set receiver ID (Doctor ID from Angular)
                Status =
                _Context.ConnectionViaSignalRUsers.FirstOrDefault(a => a.Id == message.ReceiverId) is not null ?
                StatusType.Delivered : StatusType.NotDelivered,
                ContentType = message.MessageType
            };

            // Add chat to context and save changes
            _Context.Messages.Add(chat);
            await _Context.SaveChangesAsync();

            if (message.File is not null)
            {
                string directoryPath = Path.Combine(_hostingEnvironment.ContentRootPath, $"MessageFiles/{chat.Id}");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (!File.Exists($"{directoryPath}\\{message.File.FileName}"))
                {
                    var files = Directory.GetFiles(directoryPath);

                    // Delete each file
                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            // Handle any exceptions (e.g., log them)
                            Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        }
                    }
                      chat.Content   = $"MessageFiles/{chat.Id}/{message.File.FileName}";
                    SaveFile($"{directoryPath}\\{message.File.FileName}", message.File);
                     _Context.Messages.Update(chat);
                    await _Context.SaveChangesAsync();

                }
            }
                if (message.IsForward)
                {
                    var isForward = new IsForwardMessage
                    {
                        MessageId = _Context.Messages.OrderByDescending(a => a.Id).FirstOrDefault()?.Id ?? 0
                    };
                    _Context.IsForwardMessages.Add(isForward);
                    await _Context.SaveChangesAsync();
                }
                if(message.ReplayMessageId!=0)
                {
                    var replayMessage = new IsRepliedMessage
                    {
                        RepliedMessageId = message.ReplayMessageId,
                        AnsweredMessageId = chat.Id
                    };
                _Context.IsRepliedMessages.Add(replayMessage);
                await _Context.SaveChangesAsync();
            }



                var result = await _Context.Messages
                            .Where(m => m.Id == chat.Id)
                            .Select(c => new ChatMessageDto
                            {
                                Id = c.Id,
                                Content = c.Content,
                                Time = c.Time,
                                Status = c.Status,
                                Receiver = new Receiver
                                {
                                    Id = _Context.Users.Where(r => r.Id == c.ReceiverId).Select(s => s.Id).FirstOrDefault(),
                                    Name = _Context.Users.Where(r => r.Id == c.ReceiverId).Select(s => s.Name).FirstOrDefault(),
                                    Image = _Context.Users.Where(r => r.Id == c.ReceiverId).Select(s => s.ImageURL).FirstOrDefault()
                                },
                                SenderId = c.SenderId,
                                ContentType = c.ContentType,
                                Reacts = _Context.Reacts.Where(r => r.MessageId == c.Id).Select(r => new ReactDto { Id = r.Id, React = r.ReactValue, MessageId = r.MessageId, UserId = r.UserId }).ToList(),
                                ReplayMessageId = _Context.IsRepliedMessages.Where(r=>r.AnsweredMessageId==c.Id).FirstOrDefault()!=null?
                                (_Context.IsRepliedMessages.Where(r => r.AnsweredMessageId == c.Id).FirstOrDefault()).RepliedMessageId:0
                            })
                            .FirstOrDefaultAsync();
                return result;


            }
            public static async void SaveFile(string path, IFormFile file)
            {
                // Check if the file and path are valid
                if (file == null || file.Length == 0 || string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("Invalid file or path.");
                }

                // Create directory if it doesn't exist

                // Open a stream to write the file content
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    // Copy the file content to the stream asynchronously
                    await file.CopyToAsync(stream);
                }
            }
        public async Task<bool> SendEmailMessageViaGmail(string userName, string userEmail, string subject, string textContent)
            {
                sib_api_v3_sdk.Client.Configuration.Default.ApiKey["api-key"] = "";

                // Create an instance of the TransactionalEmailsApi
                var apiInstance = new TransactionalEmailsApi();

                // Define sender details
                string senderName = "";
                string senderEmail = "mh2156@fayoum.edu.eg";
                SendSmtpEmailSender sender = new SendSmtpEmailSender(senderName, senderEmail);

                // Define recipient details
                string toEmail = userEmail;
                string toName = userName;
                SendSmtpEmailTo recipient = new SendSmtpEmailTo(toEmail, toName);
                List<SendSmtpEmailTo> recipients = new List<SendSmtpEmailTo>();
                recipients.Add(recipient);

                try
                {
                    var sendSmtpEmail = new SendSmtpEmail(sender, recipients, null, null, textContent, "xdfxg", subject); // Pass an empty list for Bcc

                    // Send the email
                    CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                    return true;


                }
                catch (Exception e)
                {
                    return false;
                }
            }

        }
        public class SendMessageResponseDto
        {
            public int Id { get; set; }
            public bool Delivered { get; set; }
        }

       
    
}
