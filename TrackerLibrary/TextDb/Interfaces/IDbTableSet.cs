using System.Collections.Generic;

namespace TrackerLibrary.TextDb.Interfaces
{
    public interface IDbTableSet
    {
        List<IDbColumn> Columns { get; set; }
        string DbTextFile { get; set; }
        string TableName { get; set; }
    }
}
