using TextDbLibrary.Enums;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.Classes
{
    public class DbColumn : IDbColumn
    {
        public DbColumn(string columnName, int columnPosition, ColumnDataType dataType)
        {
            ColumnName = columnName;
            ColumnPosition = columnPosition;
            DataType = dataType;
        }

        public string ColumnName { get; private set; }
        public int ColumnPosition { get; private set; }
        public ColumnDataType DataType { get; private set; }
    }
}
