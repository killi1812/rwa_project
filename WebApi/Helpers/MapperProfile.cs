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
        //     CreateMap<NewUserDto, User>()
        //         .ForMember(dest => dest.Password, opt => opt.Ignore())
        //         .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)));
        //     CreateMap<UserLoginDto, User>();
    }
}