using AuthService.DTO;
using AuthService.Models;
using AutoMapper;

namespace AuthService
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
