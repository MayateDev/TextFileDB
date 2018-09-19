using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Interfaces
{
    public interface ISqlParserRepository
    {
        DataSet ParseSql(string sqlString);
    }
}
