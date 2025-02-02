using AutoMapper;
using PeopleApi.Models;
using PeopleApi.Models.Dtos;

namespace PeopleApi.Mapping;

public class PeopleProfile : Profile
{
    public PeopleProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<Friendship, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Friend.Id))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Friend.Username));

        CreateMap<FriendRequestDto, Friendship>()
            .ForMember(dest => dest.FriendId, opt => opt.MapFrom(src => src.FriendId));
    }
}
