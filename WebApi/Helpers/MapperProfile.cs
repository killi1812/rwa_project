using AutoMapper;
using Data.Dto;
using Data.Models;

namespace WebApi.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<NewUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Log, LogDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToString("yyyy-MM-dd HH:mm:ss")));

        CreateMap<Picture, PictureDto>()
            .ForMember(dest => dest.guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.user, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.tags,
                opt => opt.MapFrom(src => src.PictureTags.Select(pt => pt.Tag.Name).ToList()));

        //     CreateMap<NewUserDto, User>()
        //         .ForMember(dest => dest.Password, opt => opt.Ignore())
        //         .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)));
        //     CreateMap<UserLoginDto, User>();
    }
}