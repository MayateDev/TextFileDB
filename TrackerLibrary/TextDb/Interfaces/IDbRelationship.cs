using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.TextDb.Interfaces
{
    interface IDbRelationship
    {
        string ToTable { get; set; }
        Type RelationshipReturnType { get; set; }
    }
}
