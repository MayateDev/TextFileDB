using DataLibrary.Entities;
using DataLibrary.Interfaces;

namespace DataLibrary.Repositories
{
    public class PrizeRepository : Repository<DbContext, Prize>, IPrizeRepository
    {
    }
}
