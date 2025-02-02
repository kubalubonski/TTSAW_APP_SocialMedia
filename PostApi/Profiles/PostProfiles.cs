using AutoMapper;
using PostApi.Models;

namespace PostApi.Dtos;

public class PostMapperProfile : Profile
{
    public PostMapperProfile()
    {
        CreateMap<Post, PostDto>();
        CreateMap<PostCreateDto, Post>(); 
        CreateMap<PostUpdateDto, Post>(); 
    }
}