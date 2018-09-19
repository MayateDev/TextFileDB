using System.Collections.Generic;
using TextDbLibrary.Interfaces;

namespace DataLibrary.Entities
{
    public class Matchup : IEntity
    {
        public int Id { get; set; }
        public List<MatchupEntry> Entries { get; set; } = new List<MatchupEntry>();
        public Team Winner { get; set; }
        public int MatchupRound { get; set; }
    }
}
