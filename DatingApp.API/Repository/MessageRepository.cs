using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Enums;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;

        public MessageRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Message> GetMessage(int messageid)
        {
            return await _dataContext.Messages.FirstOrDefaultAsync(x => x.Id == messageid);
        }

        public async Task<PageList<MessageToReturnDto>> GetMessagesForUser(MessageParams messageParams)
        {
            IQueryable<Message> messages;
            IQueryable<MessageToReturnDto> ret;

            switch (messageParams.MessageContainer)
            {
                case MessageContainerEnum.Inbox:
                    messages = _dataContext.Messages
                        .Where(x => x.RecipientId == messageParams.UserId
                                    && x.RecipientDeleted == false);
                    break;
                case MessageContainerEnum.Outbox:
                    messages = _dataContext.Messages
                        .Where(x => x.SenderId == messageParams.UserId
                                && x.SenderDeleted == false);
                    break;
                default:
                    messages = _dataContext.Messages
                        .Where(x => x.RecipientId == messageParams.UserId && x.IsRead == false && x.RecipientDeleted == false);
                    break;
            }
            if (messages == null)
                throw new Exception("Messages is null!");

            messages = messages.OrderByDescending(x => x.MessageSent);
            ret = messages.Select(x => new MessageToReturnDto
            {
                Id = x.Id,
                Content = x.Content,
                DateRead = x.DateRead,
                IsRead = x.IsRead,
                MessageSent = x.MessageSent,
                RecipientId = x.RecipientId,
                RecipientKnownAs = x.Recipient.KnownAs,
                RecipientPhotoUrl = x.Recipient.Photos.FirstOrDefault(y => y.IsMain).Url,
                SenderId = x.SenderId,
                SenderKnownAs = x.Sender.KnownAs,
                SenderPhotoUrl = x.Sender.Photos.FirstOrDefault(y => y.IsMain).Url
            });

            return await PageList<MessageToReturnDto>.CreateAsync(ret, messageParams.PageNumber, messageParams.PageSize);

        }

        public async Task<List<MessageToReturnDto>> GetMessageThread(int userId, int recipientid)
        {
            List<MessageToReturnDto> messages = await _dataContext.Messages
                .Where(x =>
                    (x.RecipientId == recipientid && x.SenderId == userId        //Messages sent to other users
                        && x.SenderDeleted == false)                             //sender deleted before recipient could read
                    || (x.SenderId == recipientid && x.RecipientId == userId       //Other users sending messages to us
                        && x.RecipientDeleted == false))                         //Recipient deletes a message but it's already been read, only delete for them && Sender deleted a message before recipient could read it
                .Select(x => new MessageToReturnDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    DateRead = x.DateRead,
                    IsRead = x.IsRead,
                    MessageSent = x.MessageSent, 
                    RecipientId = x.RecipientId,
                    RecipientKnownAs = x.Recipient.KnownAs,
                    RecipientPhotoUrl = x.Recipient.Photos.FirstOrDefault(y => y.IsMain).Url,
                    SenderId = x.SenderId,
                    SenderKnownAs = x.Sender.KnownAs,
                    SenderPhotoUrl = x.Sender.Photos.FirstOrDefault(y => y.IsMain).Url
                })
                .OrderByDescending(x => x.MessageSent)
                .ToListAsync();

            return messages;
        }

        public async Task<MessageForCreationDto> GetCreatedMessageAsync(MessageForCreationDto messageForCreationDto)
        {
            var user = await (from usr1 in _dataContext.Users
                              join usr2 in _dataContext.Users on messageForCreationDto.RecipientId equals usr2.Id
                              where usr1.Id == messageForCreationDto.SenderId
                              select new
                              {
                                  RecipientId = usr2.Id,
                                  RecipientPhotoUrl = usr2.Photos.FirstOrDefault(x => x.IsMain).Url,
                                  RecipientKnownAs = usr2.KnownAs,
                                  SenderId = usr1.Id,
                                  SenderPhotoUrl = usr1.Photos.FirstOrDefault(x => x.IsMain).Url,
                                  SenderKnownAs = usr1.KnownAs
                              })
                        .ToListAsync();


            if (user.Any())
            {
                foreach (var thing in user)
                {
                    messageForCreationDto.RecipientPhotoUrl = thing.RecipientPhotoUrl;
                    messageForCreationDto.RecipientKnownAs = thing.RecipientKnownAs;
                    messageForCreationDto.SenderPhotoUrl = thing.SenderPhotoUrl;
                    messageForCreationDto.SenderKnownAs = thing.SenderKnownAs;
                }

                return messageForCreationDto;
            }

            return null;
        }

        public async Task<bool> UpdateMessagesAsRead(int userId, IEnumerable<int> messageIds)
        {
            var conn = _dataContext.Database.GetDbConnection();
            await conn.OpenAsync();
            var command = conn.CreateCommand();
            string numbers = string.Join(',', messageIds);
            string query = $"UPDATE Messages SET IsRead = 1, DateRead = '{DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss")}' WHERE RecipientId = {userId} AND id IN ({numbers})";
            command.CommandText = query;
            var reader = await command.ExecuteReaderAsync();

            return true;
        }
    }
} 
