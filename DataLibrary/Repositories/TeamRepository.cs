using DataLibrary.Entities;
using DataLibrary.Interfaces;

namespace DataLibrary.Repositories
{
    public class TeamRepository : Repository<DbContext, Team>, ITeamRepository
    {
    }
}
