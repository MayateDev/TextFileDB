using DataLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
