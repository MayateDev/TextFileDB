using DataLibrary.Entities;
using DataLibrary.Interfaces;

namespace DataLibrary.Repositories
{
    public class PersonRepository : Repository<DbContext, Person>, IPersonRepository
    {
    }
}
