using AutoMapper;
using CommandService.Models;
using CommandService.Dtos;

namespace CommandService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            CreateMap<PlatformPublishedDto, Platform>()
                .ForMember(d => d.ExternalID, opt => opt.MapFrom(s => s.Id));
        }
    }
}