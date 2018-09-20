using System.Data;

namespace DomainLibrary.ViewModels
{
    public class ParseSqlStringViewModel
    {
        public DataSet QueryDataSet { get; set; }
        public string SqlString { get; set; }
        public string[] ColumnNames { get; set; }
    }
}
