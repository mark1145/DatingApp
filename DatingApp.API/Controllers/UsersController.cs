using AutoMapper;
using DatingApp.API.Dtos;
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
    [Authorize]
    [Route("api/[controller]")] // localhost:5000/api/users
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository datingRepository, IMapper mapper)
        {
            _datingRepository = datingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepository.GetUsers();

            var ret = _mapper.Map<IEnumerable<UserForListDto>>(users); //_mapper.Map<typeof source>(source);

            return Ok(ret);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepository.GetUser(id);

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

            User user = await _datingRepository.GetUser(id);

            _mapper.Map(userForUpdateDto, user);

            if (await _datingRepository.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save to database");
        }
    }
}
