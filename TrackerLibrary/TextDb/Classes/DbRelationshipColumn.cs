using System;
using TrackerLibrary.TextDb.Enums;
using TrackerLibrary.TextDb.Interfaces;

namespace TrackerLibrary.TextDb.Classes
{
    class DbRelationshipColumn : IDbRelationshipColumn
    {
        public string ColumnName { get; set; }

        public int ColumnPosition { get; set; }

        public ColumnDataType DataType { get; set; }

        public Type RelationshipReturnType { get; set; }

        public string ToTable { get; set; }
    }
}
