using System.Data;

namespace DataLibrary.Interfaces
{
    public interface ISqlParserRepository
    {
        DataSet ParseSql(string sqlString);
    }
}
