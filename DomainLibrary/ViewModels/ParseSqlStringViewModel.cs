using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLibrary.ViewModels
{
    public class ParseSqlStringViewModel
    {
        public DataSet QueryDataSet { get; set; }
        public string SqlString { get; set; }
        public string[] ColumnNames { get; set; }
    }
}
