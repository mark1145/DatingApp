using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using DatingApp.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}/[controller]")] // localhost:5000/api/users/1/Messages
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        private readonly IJwtClaimValidator _jwtClaimValidator;
        private readonly IUserRepository _userRepository;


        public MessagesController(IDatingRepository datingRepository,
            IMapper mapper,
            IMessageRepository messageRepository,
            IJwtClaimValidator jwtClaimValidator,
            IUserRepository userRepository)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
            _messageRepository = messageRepository;
            _jwtClaimValidator = jwtClaimValidator;
            _userRepository = userRepository;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!_jwtClaimValidator.UserIdConfirmed(userId, claim))
                return Unauthorized();

            Message message = await _messageRepository.GetMessage(id);

            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpGet("thread/{recipientId}", Name = "GetMessageThread")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!_jwtClaimValidator.UserIdConfirmed(userId, claim))
                return Unauthorized();

            List<MessageToReturnDto> ret = await _messageRepository.GetMessageThread(userId, recipientId);

            return Ok(ret);
        }

        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            //1. Is user who they say they are
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!_jwtClaimValidator.UserIdConfirmed(userId, claim))
                return Unauthorized();

            messageParams.UserId = userId;

            PageList<MessageToReturnDto> messagesFromRepot = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPagination(messagesFromRepot.CurrentPage, messagesFromRepot.PageSize, messagesFromRepot.TotalCount, messagesFromRepot.TotalPages);

            return Ok(messagesFromRepot);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            //1. Is user who they say they are
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!_jwtClaimValidator.UserIdConfirmed(userId, claim))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            //2. Does recipient exist?
            var ret = await _messageRepository.GetCreatedMessageAsync(messageForCreationDto);

            if (ret == null)
                return BadRequest("User does not exist");

            //3. Store the message
            Message message = _mapper.Map<Message>(messageForCreationDto);

            _datingRepository.Add(message);


            if (await _datingRepository.SaveAllAsync())
                return CreatedAtRoute("GetMessage", new { id = message.Id }, ret);


            throw new System.Exception("Failed to save message to database");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!_jwtClaimValidator.UserIdConfirmed(userId, claim))
                return Unauthorized();

            Message messageFromRepo = await _messageRepository.GetMessage(id);

            if (messageFromRepo != null)
            {
                if (messageFromRepo.SenderId == userId)
                {
                    messageFromRepo.SenderDeleted = true;

                    if (!messageFromRepo.IsRead)
                        messageFromRepo.RecipientDeleted = true;
                }
                else if (messageFromRepo.RecipientId == userId)
                    messageFromRepo.RecipientDeleted = true;

                if (await _datingRepository.SaveAllAsync())
                    return NoContent();
            }

            return NotFound();
        }

        [HttpPost("read")]
        public async Task<IActionResult> MarkMessagesAsRead(int userId, MessageReadDto messageIds)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (!_jwtClaimValidator.UserIdConfirmed(userId, claim))
                return Unauthorized();


            await _messageRepository.UpdateMessagesAsRead(userId, messageIds.MessageIds);

            await _datingRepository.SaveAllAsync();

            return NoContent();
        }
    }
}
