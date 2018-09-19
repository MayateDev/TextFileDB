using System;
using System.Collections.Generic;

namespace TextDbLibrary.Interfaces
{
    public interface IDbTableSet
    {
        IReadOnlyList<IDbColumn> Columns { get; }
        string DbTextFile { get; }
        string TableName { get; }
        Type EntityType { get; }
    }
}
