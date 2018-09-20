using DataLibrary.Interfaces;
using System.Data;
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
