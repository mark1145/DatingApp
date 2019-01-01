using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Repository
{
    public interface IMessageRepository 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Message> GetMessage(int messageid);

        /// <summary>
        /// For receiving messages in batches
        /// </summary>
        /// <returns></returns>
        Task<PageList<MessageToReturnDto>> GetMessagesForUser(MessageParams messageParams);

        /// <summary>
        /// Conversation between two users
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recipientId"></param>
        /// <returns></returns>
        Task<List<MessageToReturnDto>> GetMessageThread(int senderId, int recipientid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageForCreationDto"></param>
        /// <returns></returns>
        Task<MessageForCreationDto> GetCreatedMessageAsync(MessageForCreationDto messageForCreationDto);

        /// <summary>
        /// Updates messages as read
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateMessagesAsRead(int userId, IEnumerable<int> messageIds);
    }
}
