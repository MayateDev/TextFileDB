using TextDbLibrary.Enums;

namespace TextDbLibrary.Interfaces
{
    public interface IDbColumn
    {
        string ColumnName { get; }
        int ColumnPosition { get; }
        ColumnDataType DataType { get; }
    }
}
