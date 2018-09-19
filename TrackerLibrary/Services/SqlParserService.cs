using DataLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Interfaces;

namespace TrackerLibrary.Services
{
    public class SqlParserService : ISqlParserService
    {
        private readonly ISqlParserRepository _sqlParser;

        public SqlParserService(ISqlParserRepository sqlParser)
        {
            _sqlParser = sqlParser;
        }

        public DataSet ParseSql(string sqlString)
        {
            return _sqlParser.ParseSql(sqlString);
        }
    }
}
