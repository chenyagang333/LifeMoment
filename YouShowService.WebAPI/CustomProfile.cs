using AutoMapper;
using Chen.Commons.FileUtils;
using System.Text.Json;
using YouShowService.Domain.DTO;
using YouShowService.Domain.Entities;
using YouShowService.Domain.RequestObject;

namespace YouShowService.WebAPI;


public class CustomProfile : Profile
{
    /// <summary>
    /// 配置构造函数，用来创建关系映射
    /// </summary>
    public CustomProfile()
    {
        CreateMap<CreateYouShow, YouShow>();
        CreateMap<CreateComment, Comment>();
        CreateMap<CreateReply, Reply>();

        CreateMap<Comment, CommentDTO>();
        CreateMap<Reply, ReplyDTO>();
        CreateMap<YouShow, YouShowDTO>();
            //.ForMember(dest => dest.Files, opt => opt.MapFrom(src => DeserializeMyFileInfo(src.Files)));

    }

    private List<MyFileInfo> DeserializeMyFileInfo(IEnumerable<string>? strings)
    {
        if (strings != null && strings.Any())
        {
            return strings.Select(x => JsonSerializer.Deserialize<MyFileInfo>(x)).ToList();
        }
        return default;
    }
    //public class StringListToMyFileInfoListConverter : ITypeConverter<List<string>, List<MyFileInfo>>
    //{
    //    public List<MyFileInfo> Convert(List<string> source, List<MyFileInfo> destination, ResolutionContext context)
    //    {
    //        if (source.Count > 0)
    //        {
    //            var fileInfoList = source
    //            return fileInfoList;
    //        }
    //        return default;
    //    }
    //}
}
