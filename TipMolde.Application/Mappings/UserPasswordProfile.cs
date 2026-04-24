using AutoMapper;
using TipMolde.Application.Dtos.UserDto;
using TipMolde.Domain.Entities;

namespace TipMolde.Application.Mappings
{
    public class UserPasswordProfile : Profile
    {
        public UserPasswordProfile()
        {
            ConfigureChangePasswordMap();
            ConfigureResetPasswordMap();
        }

        private void ConfigureChangePasswordMap()
        {
            CreateMap<ChangeUserPasswordDto, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword))
                .ForMember(dest => dest.User_id, opt => opt.Ignore())
                .ForMember(dest => dest.Nome, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }

        private void ConfigureResetPasswordMap()
        {
            CreateMap<ResetUserPasswordDto, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword))
                .ForMember(dest => dest.User_id, opt => opt.Ignore())
                .ForMember(dest => dest.Nome, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}
