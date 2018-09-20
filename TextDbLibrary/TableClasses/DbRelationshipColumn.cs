using System;
using TextDbLibrary.Enums;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.TableClasses
{
    public class DbRelationshipColumn : DbColumn, IDbRelationshipColumn
    {
        public DbRelationshipColumn(string columnName, int columnPosition, ColumnDataType dataType, Type relationshipReturnType, string toTable)
            : base(columnName, columnPosition, dataType)
        {
            RelationshipReturnType = relationshipReturnType;
            ToTable = toTable;
        }
        //public string ColumnName { get; set; }

        //public int ColumnPosition { get; set; }

        //public ColumnDataType DataType { get; set; }

        public Type RelationshipReturnType { get; set; }

        public string ToTable { get; set; }
    }
}
