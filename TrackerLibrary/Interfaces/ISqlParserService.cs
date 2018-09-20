using System.Data;

namespace TrackerLibrary.Interfaces
{
    public interface ISqlParserService
    {
        DataSet ParseSql(string sqlString);
    }
}
