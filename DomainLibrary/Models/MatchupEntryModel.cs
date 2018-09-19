using TextDbLibrary.Interfaces;

namespace DomainLibrary.Models
{
    public class MatchupEntryModel : IEntity
    {
        public int Id { get; set; }
        public TeamModel TeamCompeting { get; set; }
        public double Score { get; set; }
        public MatchupModel ParentMatchup { get; set; }
    }
}
