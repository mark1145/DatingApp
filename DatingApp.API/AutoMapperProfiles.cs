using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API
{
    public class AutoMapperProfiles : Profile
    {
        /// <summary>
        /// In ctor we specify the source and destination mappings for AutoMapper
        /// </summary>
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl,
                           opt => { opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url); })
                .ForMember(dest => dest.Age,
                           opt => { opt.MapFrom(src => CalculateAge(src.DateOfBirth)); });
            CreateMap<User, UserForDetailDto>()
                .ForMember(dest => dest.PhotoUrl,
                           opt => { opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url); })
                .ForMember(dest => dest.Age,
                           opt => { opt.MapFrom(src => CalculateAge(src.DateOfBirth)); });
            CreateMap<Photo, PhotosForDetailDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap(); //map both ways
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(x => x.SenderPhotoUrl, opt => opt.MapFrom(y => y.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(x => x.RecipientPhotoUrl, opt => opt.MapFrom(y => y.Sender.Photos.FirstOrDefault(p => p.IsMain).Url));
        }

        // TODO : Move this to helper/utility class
        private int CalculateAge(DateTime dob)
        {
            if (DateTime.UtcNow < dob)
                return 0; //someone can't be born tomorrow

            return (int)Math.Floor((DateTime.UtcNow - dob).TotalDays / 365);
        }
    }
}
