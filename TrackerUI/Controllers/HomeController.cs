using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using DomainLibrary.Models;
using TextDbLibrary.Classes;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Extensions;
using TrackerLibrary.Config;
using TrackerLibrary.DataAccess;
using TrackerLibrary.Interfaces;

namespace TrackerUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ISqlParserService _sqlParser;
        private readonly ITeamService _teamService;

        public HomeController(
            IPersonService personService,
            ITeamService teamService,
            ISqlParserService sqlParser
            )
        {
            _personService = personService;
            _sqlParser = sqlParser;
            _teamService = teamService;
        }

        public ActionResult Index()
        {
            //var prize1 = new PrizeModel
            //{
            //    PlaceNumber = 1,
            //    PlaceName = "First place",
            //    PrizeAmount = 100,
            //    PrizePercentage = 0
            //};

            //var person1 = new PersonModel
            //{
            //    //Id = 7,
            //    FirstName = "Egon",
            //    LastName = "Rutgersson",
            //    EmailAddress = "egon@rutgersson.se",
            //    CellphoneNumber = "0735-12 34 89",
            //    Prize = new List<PrizeModel>()
            //};

            //var p = _personService.Read(4);
            //p.CellphoneNumber = "0700-00 13 37";
            //p = _personService.Update(p);

            //// SqlParser - Send a basic sqlstring and get a DataSet back with the results
            
            var sqlString = "Select [FirstName], [LastName] From [PersonsTbl] Where [Id] != 4";
            var results = _sqlParser.ParseSql(sqlString);

            var teams = _teamService.List();

            return View(results);
        }
    }
}