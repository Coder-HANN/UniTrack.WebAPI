using AutoMapper;
using UniTrack.Application.Feature.Club.Command;
using UniTrack.Application.Feature.Event.Command;
using UniTrack.Domain.Entities;

namespace UniTrack.Application.Abstraction.Services.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            // CreateMap<Source, Destination>();
            // 1.nin aynısını 2.ye mapler
            CreateMap<CreateEventCommand, Event>().ReverseMap();
            CreateMap<UserJoinToEventCommand, EventUser>().ReverseMap();
            CreateMap<CreateClubCommand, Club>().ReverseMap();


        }
    }
}
