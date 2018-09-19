using TextDbLibrary.Interfaces;

namespace DataLibrary.Entities
{
    public class MatchupEntry : IEntity
    {
        public int Id { get; set; }
        public Team TeamCompeting { get; set; }
        public double Score { get; set; }
        public Matchup ParentMatchup { get; set; }
    }
}
