using AutoMapper;

namespace WebApp.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        //     CreateMap<NewUserDto, User>()
        //         .ForMember(dest => dest.Password, opt => opt.Ignore())
        //         .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)));
        //     CreateMap<UserLoginDto, User>();
    }
}