using AutoMapper;
using Data.Models;
using WebApp.Models;
using WebApp.ViewModels;

namespace WebApp.Helpers;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Picture, PictureVM>()
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PictureTags.Select(x => x.Tag.Name).ToList()))
            .ForMember(dest => dest.UserGuid, opt => opt.MapFrom(src => src.User.Guid))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));
        CreateMap<Paginated<PictureVM>, SearchVM<PictureVM>>()
            .ForMember(dest => dest.Query, opt => opt.Ignore());

        CreateMap<Picture, PictureDetailsVM>()
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PictureTags.Select(x => x.Tag.Name).ToList()))
            .ForMember(dest => dest.UserGuid, opt => opt.MapFrom(src => src.User.Guid))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.DownloadsCount, opt => opt.MapFrom(src => src.Downloads.Count))
            .ForMember(dest => dest.Downloads,
                opt => opt.MapFrom(src =>
                    src.Downloads.Select(x => $"{x.User.Username} downloaded {x.Date:yy-MM-dd HH:mm}").ToList()));
    }
}