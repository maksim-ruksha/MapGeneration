using AutoMapper;
using MapGeneration.BLL.Models;
using MapGeneration.BLL.Models.Users;
using MapGeneration.DAL.Entities;
using MapGeneration.DAL.Entities.Users;

namespace MapGeneration.BLL.Mapping;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<UserModel, UserEntity>().ReverseMap();
        CreateMap<MapModel, MapEntity>().ReverseMap();
        CreateMap<CommentModel, CommentEntity>().ReverseMap();
        CreateMap<UserModel, UserEntity>().ReverseMap();
    }
}