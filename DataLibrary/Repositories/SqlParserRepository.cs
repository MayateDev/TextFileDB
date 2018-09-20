using DataLibrary.Interfaces;
using TextDbLibrary.Classes;

namespace DataLibrary.Repositories
{
    public class SqlParserRepository : SqlParser, ISqlParserRepository
    {
        private readonly DbContext db;

        public SqlParserRepository()
        {
            db = new DbContext();
        }
    }
}
