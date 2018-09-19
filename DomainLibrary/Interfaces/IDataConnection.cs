using DomainLibrary.Models;

namespace DomainLibrary.Interfaces
{
    public interface IDataConnection
    {
        PersonModel CreatePerson(PersonModel model);
        PrizeModel CreatePrize(PrizeModel model);
        TeamModel CreateTeam(TeamModel model);
    }
}
