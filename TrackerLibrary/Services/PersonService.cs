using DataLibrary.Entities;
using DataLibrary.Interfaces;
using DomainLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Interfaces;

namespace TrackerLibrary.Services
{
    public class PersonService : Service<IPersonRepository, Person, PersonModel>, IPersonService
    {
        private readonly IPersonRepository _personRepo;

        public PersonService(IPersonRepository personRepo)
            : base(personRepo)
        {
            _personRepo = personRepo;
        }
    }
}
