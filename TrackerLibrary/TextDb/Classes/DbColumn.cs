using TrackerLibrary.TextDb.Enums;
using TrackerLibrary.TextDb.Interfaces;

namespace TrackerLibrary.TextDb.Classes
{
    public class DbColumn : IDbColumn
    {
        public string ColumnName { get; set; }
        public int ColumnPosition { get; set; }
        public ColumnDataType DataType { get; set; }
    }
}
