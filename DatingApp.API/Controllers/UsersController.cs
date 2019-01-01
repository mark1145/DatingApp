using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
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
    [Route("api/[controller]")] // localhost:5000/api/users
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository, IUserRepository userRepository, ILikeRepository likeRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _userRepository = userRepository;
            _likeRepository = likeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return UnprocessableEntity("Unable to find NameIdentifier on Claim");

            int userId = int.Parse(claim.Value);
            User thisUser = await _userRepository.GetUserAsync(userId);
            userParams.UserId = userId;

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = thisUser.Gender.ToLower() == "male" ? "female" : "male";

            var users = await _userRepository.GetUsersAsync(userParams);

            var ret = _mapper.Map<IEnumerable<UserForListDto>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(ret);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetUserPhotoAsync(id);

            var ret = _mapper.Map<UserForDetailDto>(user);

            return Ok(ret);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return UnprocessableEntity("Unable to find NameIdentifier on Claim");

            //1. Make sure that user trying to update the profile is matching the profile their trying to update
            if (id != int.Parse(claim.Value))
                return Unauthorized();

            User user = await _userRepository.GetUserAsync(id);

            _mapper.Map(userForUpdateDto, user);

            if (await _datingRepository.SaveAllAsync())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save to database");
        }

        [HttpPost("{id}/like/{recepientId}")]
        public async Task<IActionResult> LikeUser(int id, int recepientId)
        {
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return UnprocessableEntity("Unable to find NameIdentifier on Claim");

            //1. Make sure that user trying to update the profile is matching the profile their trying to update
            if (id != int.Parse(claim.Value))
                return Unauthorized();

            if (await _likeRepository.IsUserAlreadyLikedAsync(id, recepientId))
                return BadRequest("You already liked this user");

            if (await _userRepository.GetUserAsync(recepientId) == null)
                return NotFound("Cannot find user");

            Like like = new Like
            {
                LikerId = id,
                LikeeId = recepientId
            };

            _datingRepository.Add<Like>(like);

            if (await _datingRepository.SaveAllAsync())
                return Ok();

            return BadRequest("Failed to like user");
        }
    }
}
