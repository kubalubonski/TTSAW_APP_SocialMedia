using AutoMapper;
using IdentityApi.Models;
using IdentityApi.Models.Dtos;

namespace IdentityApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}