using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WhatsApp_Clone.Models;
using WhatsApp_Clone.Tables;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WhatsApp_Clone
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _Context;

        public ChatHub(ApplicationDbContext context)
        {
            _Context = context;
        }

        public async Task<ChatMessageDto> SendMessage(int messageId,string receiverId)
        {
            try
            {
                var user = await _Context.Users.FirstOrDefaultAsync(a => a.Id == receiverId);
                var m = await _Context.Messages.FirstOrDefaultAsync(a => a.Id == messageId);


                if (user != null&& m!=null)
                {
                    var message = await _Context.Messages
                        .Where(c => c.ReceiverId == receiverId && c.Id == messageId)
                        .Select(c => new ChatMessageDto
                        {
                            Id = c.Id,
                            Content = c.Content,
                            Time = c.Time,
                            Status = c.Status,
                            Receiver =  new Receiver
                                {
                                    Id = _Context.Users.Where(r => (r.Id == c.ReceiverId&& r.Id != user.Id)|| (r.Id == c.SenderId && r.Id != user.Id)).Select(s => s.Id).FirstOrDefault(),
                                    Name = _Context.Users.Where(r => (r.Id == c.ReceiverId && r.Id != user.Id) || (r.Id == c.SenderId && r.Id != user.Id)).Select(s => s.Name).FirstOrDefault(),
                                    Image = _Context.Users.Where(r => (r.Id == c.ReceiverId && r.Id != user.Id) || (r.Id == c.SenderId && r.Id != user.Id)).Select(s => s.ImageURL).FirstOrDefault()
                                },
                            SenderId = c.SenderId,
                            ContentType = c.ContentType,
                            Reacts = _Context.Reacts.Where(r => r.MessageId == c.Id).Select(r=>new ReactDto {Id=r.Id,React=r.ReactValue,MessageId=r.MessageId,UserId=r.UserId }).ToList(),
                            ReplayMessageId = _Context.IsRepliedMessages.Where(r => r.AnsweredMessageId == c.Id).FirstOrDefault() != null ?
                                (_Context.IsRepliedMessages.Where(r => r.AnsweredMessageId == c.Id).FirstOrDefault()).RepliedMessageId : 0
                        })
                        .FirstOrDefaultAsync();

                    // Notify clients via SignalR (if needed)
                    var userInConnectionTable = await _Context.ConnectionViaSignalRUsers.FirstOrDefaultAsync(a => a.Id == user.Id);
                    if (userInConnectionTable != null)
                        await Clients.Client(userInConnectionTable.ConnectionId).SendAsync("ReceiveMessage", message);

                    return message;
                }

                return new ChatMessageDto();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in SendMessage: {ex.Message}");
                return new ChatMessageDto(); // Return an empty list on error
            }
        }

        public async Task<List<int>> ReadMessages(string UserId, string SelectedUserId)
        {
            try
            {
                var user = await _Context.Users.FirstOrDefaultAsync(a => a.Id == UserId);
                var SelectedUser = await _Context.Users.FirstOrDefaultAsync(a => a.Id == SelectedUserId);


                if (user != null && null != SelectedUser)
                {
                    var messages = await _Context.Messages
                        .Where(c => c.ReceiverId == UserId && c.SenderId == SelectedUserId&&(c.Status== StatusType.Delivered|| c.Status== StatusType.NotDelivered)).ToListAsync();
                    foreach (var message in messages)
                    {
                        message.Status = StatusType.Readed;
                        _Context.Messages.Update(message);
                    }
                    await _Context.SaveChangesAsync();
                    // Notify clients via SignalR (if needed)
                    var result = messages.Select(a => a.Id).ToList();
                    var userInConnectionTable = await _Context.ConnectionViaSignalRUsers.FirstOrDefaultAsync(a => a.Id == SelectedUserId);
                    if (userInConnectionTable != null)
                        await Clients.Client(userInConnectionTable.ConnectionId).SendAsync("ReadMessages", result);

                    return result;
                }

                return new List<int>();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in SendMessage: {ex.Message}");
                return new List<int>(); // Return an empty list on error
            }
        }


        public async Task<ReactDto> AddReact(string UserId, string SelectedUserId, ReactDto reactDto)
        {
            try
            {
                // Retrieve the user and message from the database
                var user = await _Context.Users.Where(u => u.Id == reactDto.UserId).FirstOrDefaultAsync();
                var message = await _Context.Messages.Where(u => u.Id == reactDto.MessageId).FirstOrDefaultAsync();

                if (user != null && message != null)
                {
                    var existReact = await _Context.Reacts.Where(r => r.UserId == reactDto.UserId && r.MessageId == reactDto.MessageId).FirstOrDefaultAsync();
                    React react;

                    if (existReact == null)
                    {
                        // Create a new react if it doesn't exist
                        react = new React
                        {
                            UserId = reactDto.UserId,
                            MessageId = reactDto.MessageId,
                            ReactValue = reactDto.React
                        };
                        _Context.Reacts.Add(react); // Adding the new react
                    }
                    else
                    {
                        // Update the existing react
                        react = existReact;
                        react.ReactValue = reactDto.React; // Update only the ReactValue
                        _Context.Reacts.Update(react); // Update the existing react in the database
                    }

                    // Save changes
                    await _Context.SaveChangesAsync();
                    reactDto.Id = react.Id;  // Ensure the Id is populated after saving

                    // Return the updated ReactDto
                    var updatedReactDto = new ReactDto
                    {
                        Id = react.Id,
                        React = react.ReactValue,
                        UserId = user.Id,
                        MessageId = message.Id
                    };

                    // Notify the client via SignalR (if needed)
                    var userInConnectionTable = await _Context.ConnectionViaSignalRUsers.FirstOrDefaultAsync(a => a.Id == SelectedUserId);
                    if (userInConnectionTable != null)
                    {
                        await Clients.Client(userInConnectionTable.ConnectionId).SendAsync("listenerReact", updatedReactDto);
                    }

                    return updatedReactDto; // Return the updated react DTO to the caller
                }

                // If user or message not found, return an empty ReactDto (or handle differently based on your requirements)
                return new ReactDto
                {
                    Id = 0,
                    React = reactDto.React,
                    UserId = reactDto.UserId,
                    MessageId = reactDto.MessageId
                };
            }
            catch (Exception ex)
            {
                // Log the error and return an empty ReactDto (or handle differently based on your needs)
                Console.Error.WriteLine($"Error in AddReact: {ex.Message}");
                return new ReactDto
                {
                    Id = 0,
                    React = "Error",
                    UserId = reactDto.UserId,
                    MessageId = reactDto.MessageId
                };
            }
        }


        public async Task LoadUser(string phone)
        {
            try
            {
                var user = await _Context.Users.Where(a => a.PhoneNumber == phone).Select(u => new
                {
                    Id=u.Id,
                    Name=u.Name,
                    ImgURL=u.ImageURL,
                    Phone=u.PhoneNumber
                }).FirstOrDefaultAsync();

                if (user != null)
                {
                   

                    await Clients.All.SendAsync("LoadUsers", user);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in LoadUsers: {ex.Message}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            // Get the userId from the query string
            var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(userId))
            {
                var connectionId = Context.ConnectionId;

                // Check if the user is already in the connection table
                var existingConnection = await _Context.ConnectionViaSignalRUsers.FirstOrDefaultAsync(c => c.Id == userId);

                if (existingConnection == null)
                {
                    // Add the user to the ConnectionViaSignalRUser table if not already present
                    var newConnection = new ConnectionViaSignalRUser
                    {
                        Id = userId,
                        ConnectionId = connectionId
                    };

                    _Context.ConnectionViaSignalRUsers.Add(newConnection);
                    await _Context.SaveChangesAsync();
                }
                else
                {
                    // Update the user's connection ID if they reconnect
                    existingConnection.ConnectionId = connectionId;
                    _Context.ConnectionViaSignalRUsers.Update(existingConnection);
                    await _Context.SaveChangesAsync();
                }
                var messages = await _Context.Messages.Where(m => m.ReceiverId == userId).ToListAsync();
                foreach (var message in messages)
                {
                    if(message.Status!=StatusType.Readed)
                    message.Status = StatusType.Delivered;
                    _Context.Messages.Update(message);
                }
                await _Context.SaveChangesAsync();

                await Clients.AllExcept(connectionId).SendAsync("UpdateUsersMessagesToBeDeliveredExpectNewConnectedUser", userId);

            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            // Find the connection record for this connection ID
            var connectionRecord = await _Context.ConnectionViaSignalRUsers.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);

            if (connectionRecord != null)
            {
                // Remove the connection record or set the ConnectionId to null
                _Context.ConnectionViaSignalRUsers.Remove(connectionRecord);
                // Alternatively, you can just nullify the ConnectionId if you want to keep the record
                // connectionRecord.ConnectionId = null;
                // _Context.ConnectionViaSignalRUsers.Update(connectionRecord);
            }

            await _Context.SaveChangesAsync();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendSignal(string userId, object signal)
        {
            await Clients.User(userId).SendAsync("ReceiveSignal", signal);
        }

        public async Task JoinCall(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }

        public async Task LeaveCall(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
    }

    public class ChatMessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public List<ReactDto> Reacts { get; set; }
        public int ReplayMessageId { get; set; }
        public ContentType ContentType { get; set; }
        public DateTime Time { get; set; }
        public StatusType Status { get; set; }
        public Receiver? Receiver { get; set; }

    }
    public class ReactDto{
        public int Id { get; set; }
        public string React { get; set; }
        public int MessageId { get; set; }
        public string UserId { get; set; }
    }   
    public class Receiver {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }
    
}
