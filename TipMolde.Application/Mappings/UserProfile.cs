using AutoMapper;
using TipMolde.Application.Dtos.UserDto;
using TipMolde.Domain.Entities;

namespace TipMolde.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.Nome.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.User_id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.Nome, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Nome));
                    opt.MapFrom(src => src.Nome!.Trim());
                })
                .ForMember(dest => dest.Email, opt =>
                {
                    opt.Condition(src => !string.IsNullOrWhiteSpace(src.Email));
                    opt.MapFrom(src => src.Email!.Trim());
                })
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.User_id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<ChangeUserRoleDto, User>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.User_id, opt => opt.Ignore())
                .ForMember(dest => dest.Nome, opt => opt.Ignore())
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<User, ResponseUserDto>();

        }
    }
}
