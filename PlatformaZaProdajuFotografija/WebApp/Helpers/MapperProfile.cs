using AutoMapper;
using Data.Dto;
using Data.Models;
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
        CreateMap<List<Picture>, SearchVM<PictureVM>>()
            .ForMember(dest => dest.Query, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src))
            .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
            .ForMember(dest => dest.Page, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Count()))
            .ForMember(dest => dest.FromPage, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.ToPage, opt => opt.MapFrom(src => 1))
            .ForMember(dest => dest.LastPage, opt => opt.MapFrom(src => 1));

        CreateMap<Picture, PictureDetailsVM>()
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.PictureTags.Select(x => x.Tag.Name).ToList()))
            .ForMember(dest => dest.UserGuid, opt => opt.MapFrom(src => src.User.Guid))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.DownloadsCount, opt => opt.MapFrom(src => src.Downloads.Count))
            .ForMember(dest => dest.Downloads,
                opt => opt.MapFrom(src =>
                    src.Downloads.OrderByDescending(d => d.Date).Take(10)
                        .Select(x => $"{x.User.Username} downloaded {x.Date:yyyy-MM-dd HH:mm}").ToList()));
        CreateMap<Picture, UpdatePictureVM>()
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => string.Join("\n", src.PictureTags.Select(x => x.Tag.Name).ToList())));
        CreateMap<UpdatePictureVM, UpdatePictureDto>()
            .ForMember(dest => dest.Guid, opt => opt.Ignore())
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src =>
                    new List<string>(src.Tags.Split('\n', StringSplitOptions.TrimEntries))));
        CreateMap<User, UserVM>()
            .ForMember(dest => dest.Guid, opt => opt.MapFrom(src => src.Guid.ToString()))
            .ForMember(dest => dest.Downloads,
                opt => opt.MapFrom(src =>
                    src.Downloads.OrderByDescending(d => d.Date)
                        .Select(x => $"{x.User.Username} downloaded {x.Date:yyyy-MM-dd HH:mm}").ToList()))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));

        CreateMap<List<Tag>, TagsVM>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src));
    }
}