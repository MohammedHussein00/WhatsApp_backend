using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Music_Aditor.Models;
using WhatsApp_Clone.Models;
using WhatsApp_Clone.Services;
using WhatsApp_Clone.Tables;

namespace WhatsApp_Clone.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class UserController : Controller
    {

        private readonly IStudentService _studentService;
        private readonly ApplicationDbContext _Context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;


        private readonly IAuthService _authService;
        public UserController(IStudentService studentService, IWebHostEnvironment env, IAuthService authService, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _authService = authService;
            _Context = context;
            _userManager = userManager;
            _env = env;
            _studentService = studentService;


        }

        [HttpGet("laod-chat")]
        public async Task<IActionResult> LoadChat(string id)
        {
            try
            {
                var user = await _Context.Users.FirstOrDefaultAsync(a => a.Id == id);

                if (user != null)
                {
                    var messages = await _Context.Messages
                        .Where(c => c.ReceiverId == user.Id || c.SenderId == user.Id)
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
                            ReplayMessageId = _Context.IsRepliedMessages.Where(r => r.AnsweredMessageId == c.Id).FirstOrDefault() != null ?
                                (_Context.IsRepliedMessages.Where(r => r.AnsweredMessageId == c.Id).FirstOrDefault()).RepliedMessageId : 0
                        })
                        .ToListAsync();

                    return Ok(messages);
                }

                return Ok(new List<ChatMessageDto>());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in LoadChat: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromForm] MessageDto message)
        {
            try
            {
                var x = await _studentService.SendMessage(message);
                return Ok(x);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request. Please try again later.");
            }
        }
        [Authorize(Roles = "Student")]

        [HttpGet("read-messages")]
        public async Task<IActionResult> ReadMessages(string userId, string senderId)
        {
            try
            {

                if (await _Context.Users.AnyAsync(a => a.Id == userId) && await _Context.Users.AnyAsync(a => a.Id == senderId))
                {
                    var messages = await _Context.Messages.Where(c => c.ReceiverId == userId && c.SenderId == senderId).ToListAsync();
                    foreach (var message in messages)
                    {
                        message.Status = StatusType.Readed;
                        _Context.Update(message);

                    }
                    await _Context.SaveChangesAsync();
                    return Ok(new { readed = true });

                }
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while sending message. Please try again later.");
            }
        }

        [HttpGet("get-user-to-chat")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                var user = await _Context.Users.Where(a => a.UserName == email).Select(u => new
                {
                    Id = u.Id,
                    Name = u.Name,
                    ImgURL = u.ImageURL,
                    Phone = u.PhoneNumber
                }).FirstOrDefaultAsync();

                if (user is not null)
                    return Ok(user);

                return Ok("Student not found");

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while sending message. Please try again later.");
            }
        }


        [HttpGet("laod-selected-user-info")]
        public async Task<IActionResult> LoadUserInfo(string id)
        {
            var user = await _Context.Users.Where(a => a.Id == id).Select(a => new { a.About, a.UserName }).FirstOrDefaultAsync();
            if (user is not null)
            {
                return Ok(user);
            }
            return Ok();
        }


        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(createGroupDto groupDto)
        {
            var group = await _Context.Groups.Where(a => a.Id == groupDto.Id).FirstOrDefaultAsync();
            if (group is not null)
            {
                var newGroup = new Group
                {
                    AdminId = groupDto.AdminId,
                    Name = groupDto.GroupName,
                    Caption=groupDto.GroupCaption,
                    Icon=groupDto.GroupCaption
                };

                
                _Context.Groups.Add(newGroup);

                await _Context.SaveChangesAsync();

                foreach(var memberId in groupDto.GroupMembers)
                {
                    var newMember = new GroupUser
                    {
                        UserId = memberId,
                        GroupId = newGroup.Id
                    };
                    _Context.GroupUsers.Add(newMember);
                    await _Context.SaveChangesAsync();
                }
                groupDto.Id = newGroup.Id;
                return Ok(groupDto);
            }
            return Ok();
        }

    }
}
