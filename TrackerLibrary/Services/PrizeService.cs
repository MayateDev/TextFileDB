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
    public class PrizeService : Service<IPrizeRepository, Prize, PrizeModel>, IPrizeService
    {
        private readonly IPrizeRepository _prizeRepo;

        public PrizeService(IPrizeRepository prizeRepo)
            : base(prizeRepo)
        {
            _prizeRepo = prizeRepo;
        }
    }
}
