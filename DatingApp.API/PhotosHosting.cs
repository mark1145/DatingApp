using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.DataStructures;
using DatingApp.API.Dtos;
using DatingApp.API.Interfaces;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API
{
    public class PhotosHosting : IPhotosHosting
    {
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public PhotosHosting(IOptions<CloudinarySettings> cloudinarySettings, IMapper mapper)
        {
            _cloudinarySettings = cloudinarySettings;
            _mapper = mapper;

            Account acc = new Account(
                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret
                );

            _cloudinary = new Cloudinary(acc);
        }

        public async Task<bool> DeletePhoto(Photo photo)
        {
            //Not a cloudinary photo or delete succeeded from cloudinary but failed in database
            if (photo.PublicId == null)
                return true;

            DeletionParams deletionParams = new DeletionParams(photo.PublicId);

            DeletionResult result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result == "ok")
                return true;

            return false;
        }

        public async Task<Photo> StorePhoto(PhotoForCreationDto photoForCreationDto)
        {
            IFormFile file = photoForCreationDto.File;

            //Results we get back from cloudinary
            ImageUploadResult uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    ImageUploadParams uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            Photo photo = _mapper.Map<Photo>(photoForCreationDto);

            return photo;
        }

        public bool SwapPhoto(int photoId, PhotoForCreationDto photoForCreationDto)
        {
            throw new NotImplementedException();
        }
    }
}
