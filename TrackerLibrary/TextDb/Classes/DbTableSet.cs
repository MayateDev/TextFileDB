using System.Collections.Generic;
using TrackerLibrary.TextDb.Interfaces;

namespace TrackerLibrary.TextDb.Classes
{
    public class DbTableSet : IDbTableSet
    {
        public List<IDbColumn> Columns { get; set; }
        public string DbTextFile { get; set; }
        public string TableName { get; set; }
    }
}
