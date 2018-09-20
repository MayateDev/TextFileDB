using System.Collections.Generic;
using TextDbLibrary.Interfaces;

namespace DomainLibrary.Models
{
    public class PersonModel : IEntity
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string CellphoneNumber { get; set; }
        public List<PrizeModel> Prize { get; set; }
    }
}
