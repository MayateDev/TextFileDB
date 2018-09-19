using AutoMapper;
using DataLibrary.Entities;
using DomainLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
