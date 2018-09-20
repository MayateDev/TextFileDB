using TextDbLibrary.Interfaces;

namespace DataLibrary.Entities
{
    public class MatchupEntry : IPrimaryString
    {
        public string Id { get; set; }
        public Team TeamCompeting { get; set; }
        public double Score { get; set; }
        public Matchup ParentMatchup { get; set; }
    }
}
