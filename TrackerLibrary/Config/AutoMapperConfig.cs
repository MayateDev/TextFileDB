using AutoMapper;
using DataLibrary.Entities;
using DomainLibrary.Models;

namespace TrackerLibrary.Config
{
    public class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Person, PersonModel>();
                cfg.CreateMap<PersonModel, Person>();

                cfg.CreateMap<Team, TeamModel>();
                cfg.CreateMap<TeamModel, Team>();

                cfg.CreateMap<Prize, PrizeModel>();
                cfg.CreateMap<PrizeModel, Prize>();
            });
        }
    }
}
