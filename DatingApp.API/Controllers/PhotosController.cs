using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.DataStructures;
using DatingApp.API.Dtos;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/[controller]")] // localhost:5000/api/photos
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _datingRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IMapper _mapper;
        private readonly IPhotosHosting _photosHosting;

        public PhotosController(IDatingRepository datingRepository, IPhotoRepository photoRepository, IMapper mapper, IPhotosHosting photosHosting)
        {
            _datingRepository = datingRepository;
            _photoRepository = photoRepository;
            _mapper = mapper;
            _photosHosting = photosHosting;
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            Photo photo = await _photoRepository.GetPhotoAsync(id);

            PhotoForReturnDto ret = _mapper.Map<PhotoForReturnDto>(photo);

            return Ok(ret);
        }
        

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto)
        {
            //1. Make sure that user trying to update the profile is matching the profile their trying to update
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return UnprocessableEntity("Unable to find NameIdentifier on Claim");

            if (userId != int.Parse(claim.Value))
                return Unauthorized();

            if (photoForCreationDto.File == null)
                return BadRequest("No picture uploaded");

            Photo photo = await _photosHosting.StorePhoto(photoForCreationDto);

            User user = await _datingRepository.GetUser(userId);
                       
            if (!user.Photos.Any())
                photo.IsMain = true;

            user.Photos.Add(photo);

            
            if (await _datingRepository.SaveAll())
            {
                PhotoForReturnDto ret = _mapper.Map<PhotoForReturnDto>(photo);

                //Because HTTPPost; supposed to return CreatedAtRoute() so we return:
                //a) route to location of object we created
                //b) the id of the photo we return
                //c) the actual photo object
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, ret);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{photoId}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            //1. Make sure that user trying to update the profile is matching the profile their trying to update
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return UnprocessableEntity("Unable to find NameIdentifier on Claim");

            if (userId != int.Parse(claim.Value))
                return Unauthorized();

            User user = await _datingRepository.GetUser(userId);

            // Need to make sure that user is updating THEIR photo
            if (!user.Photos.Any(x => x.Id == photoId))
                return Unauthorized();

            Photo photo = await _photoRepository.GetPhotoAsync(photoId);

            if (photo.IsMain)
                return BadRequest("Already profile photo");

            Photo currentMainPhoto = await _photoRepository.GetMainPhotoForUserAsync(userId);
            currentMainPhoto.IsMain = false;

            photo.IsMain = true;

            if (await _photoRepository.SaveAll())
                return NoContent();

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoId)
        {
            //1. Make sure that user trying to update the profile is matching the profile their trying to update
            Claim claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                return UnprocessableEntity("Unable to find NameIdentifier on Claim");

            if (userId != int.Parse(claim.Value))
                return Unauthorized();

            User user = await _datingRepository.GetUser(userId);

            // Need to make sure that user is updating THEIR photo
            if (!user.Photos.Any(x => x.Id == photoId))
                return Unauthorized();

            Photo photo = await _photoRepository.GetPhotoAsync(photoId);

            if (photo.IsMain)
                return BadRequest("You cannot delete your main photo");

            bool res = await _photosHosting.DeletePhoto(photo);

            if (res)
            {
                _photoRepository.DeletePhoto(photo);

                if (await _photoRepository.SaveAll())
                    return Ok();
            }

            return UnprocessableEntity("Failed to delete the photo");
        }
    }
}
