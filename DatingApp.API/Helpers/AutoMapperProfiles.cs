using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDTO>()
                .ForMember(dest => dest.Age,
                       opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl,
                        opt => opt.MapFrom(src => src.Photos.FirstOrDefault(i => i.IsMain).Url));
            
            CreateMap<User, UserForDetailDTO>()
                .ForMember(dest => dest.Age,
                       opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl,
                           opt => opt.MapFrom(src => src.Photos.FirstOrDefault(i => i.IsMain).Url));

            CreateMap<User, UserForAddDTO>()
                .ForMember(dest => dest.Age,
                       opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl,
                           opt => opt.MapFrom(src => src.Photos.FirstOrDefault(i => i.IsMain).Url));

            CreateMap<User, UserForUseDTO>()
                .ForMember(dest => dest.Age,
                       opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                .ForMember(dest => dest.PhotoUrl,
                        opt => opt.MapFrom(src => src.Photos.FirstOrDefault(i => i.IsMain).Url));
            
            CreateMap<Photo, PhotoForDetailDTO>();

            CreateMap<UserForRegistrationDTO, User>();

            CreateMap<UserForUpdateDTO, User>();

            CreateMap<PhotoForAddDTO, Photo>();

            CreateMap<Photo, PhotoForReturnDTO>();

            CreateMap<MessageForAddDTO, Message>();

            CreateMap<Message, MessageForReturnDTO>()
                .ForMember(dest => dest.SenderPhotoUrl,
                       opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(i => i.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl,
                       opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(i => i.IsMain).Url));
        }
    }
}