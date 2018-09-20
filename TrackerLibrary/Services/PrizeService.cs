using DataLibrary.Entities;
using DataLibrary.Interfaces;
using DomainLibrary.Models;
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
