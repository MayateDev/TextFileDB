using System.Collections.Generic;
using TextDbLibrary.Interfaces;

namespace DataLibrary.Entities
{
    public class Team : IEntity
    {
        public int Id { get; set; }
        public List<Person> TeamMembers { get; set; } = new List<Person>();
        public string TeamName { get; set; }
    }
}