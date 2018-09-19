using System.Collections.Generic;
using TextDbLibrary.Interfaces;

namespace DomainLibrary.Models
{
    public class MatchupModel : IEntity
    {
        public int Id { get; set; }
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        public TeamModel Winner { get; set; }
        public int MatchupRound { get; set; }
    }
}
