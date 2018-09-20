using DataLibrary.Entities;
using DataLibrary.Interfaces;
using DomainLibrary.Models;
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
