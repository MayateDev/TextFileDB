using System.Collections.Generic;
using TextDbLibrary.Interfaces;

namespace DomainLibrary.Models
{
    public class TeamModel : IEntity
    {
        public int Id { get; set; }
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
        public string TeamName { get; set; }
    }
}