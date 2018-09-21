using System;

namespace TextDbLibrary.Interfaces
{
    public interface IDbRelationship
    {
        string ToTable { get; } // set;
        Type RelationshipReturnType { get; } // set;
    }
}
