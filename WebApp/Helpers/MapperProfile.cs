using AutoMapper;
using Data.Models;
using WebApp.ViewModels;

namespace WebApp.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Picture, PictureVM>()
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PictureTags.Select(x => x.Tag.Name).ToList()));
    }
}