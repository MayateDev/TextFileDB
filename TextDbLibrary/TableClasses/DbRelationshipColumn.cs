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

        public Type RelationshipReturnType { get; private set; } // set;
        public string ToTable { get; private set; } // set;
    }
}
