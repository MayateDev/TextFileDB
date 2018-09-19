using DataLibrary.Entities;
using DataLibrary.Interfaces;
using DomainLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Interfaces;

namespace TrackerLibrary.Services
{
    public class TeamService : Service<ITeamRepository, Team, TeamModel>, ITeamService
    {
        private readonly ITeamRepository _teamRepo;

        public TeamService(ITeamRepository teamRepo)
            : base(teamRepo)
        {
            _teamRepo = teamRepo;
        }
    }
}
