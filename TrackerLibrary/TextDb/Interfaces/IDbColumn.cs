using TrackerLibrary.TextDb.Enums;

namespace TrackerLibrary.TextDb.Interfaces
{
    public interface IDbColumn
    {
        string ColumnName { get; set; }
        int ColumnPosition { get; set; }
        ColumnDataType DataType { get; set; }
    }
}
