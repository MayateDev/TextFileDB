using System;
using System.Collections.Generic;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Extensions;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.Classes
{
    public class DbTableSet<T> : IDbTableSet
    {
        public DbTableSet(IReadOnlyList<IDbColumn> columns, string dbTextFile, string tableName)
        {
            Columns = columns;
            DbTextFile = dbTextFile;
            TableName = tableName;
            EntityType = typeof(T);
        }

        public IReadOnlyList<IDbColumn> Columns { get; private set; }
        public string DbTextFile { get; private set; }
        public string TableName { get; private set; }
        public Type EntityType { get; private set; }
    }
}
