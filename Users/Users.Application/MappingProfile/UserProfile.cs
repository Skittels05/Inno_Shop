using AutoMapper;
using Users.Application.CQRS.Commands;
using Users.Application.DTOs;
using Users.Domain.Entities;
using Users.Domain.Enums;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<CreateUserDto, CreateUserCommand>();

        CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(_ => Role.User));

        CreateMap<UpdateUserDto, User>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
