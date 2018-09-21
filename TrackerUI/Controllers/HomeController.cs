using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using DomainLibrary.Models;
using DomainLibrary.ViewModels;
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
            var prize1 = new PrizeModel
            {
                Id = 2,
                PlaceNumber = 1,
                PlaceName = "First place",
                PrizeAmount = 100,
                PrizePercentage = 0
            };

            var person1 = new PersonModel
            {
                //Id = 7,
                FirstName = "Robert",
                LastName = "Petersson",
                EmailAddress = "robert@itka.se",
                CellphoneNumber = "0735-00 13 37",
                Prize = new List<PrizeModel> { prize1 }
            };

            var person2 = new PersonModel
            {
                //Id = 7,
                FirstName = "Rut",
                LastName = "Stensson",
                EmailAddress = "rut@rutan.se",
                CellphoneNumber = "0735-12 34 89",
                Prize = new List<PrizeModel>()
            };
            //var p1 = _personService.Read("bfcf5e0c-64f8-448b-a30d-115f4756a60f");
            //var p2 = _personService.Add(person1);
            //var p3 = _personService.Add(person2);

            //_personService.AddModels(new List<PersonModel> { person1, person2});
            //var persons = _personService.List().Skip(1).ToList();

            //var team = new TeamModel
            //{
            //    TeamMembers = persons,
            //    TeamName = "The Misters"
            //};

            //_teamService.Add(team);

            //var p = _personService.Read("bfcf5e0c-64f8-448b-a30d-115f4756a60f");
            //p.CellphoneNumber = "0700-00 13 37";
            //p = _personService.Update(p);

            //// SqlParser - Send a basic sqlstring and get a DataSet back with the results

            //var sqlString = "Select [FirstName], [LastName] From [PersonsTbl] Where [Id] != 4";
            //var results = _sqlParser.ParseSql(sqlString);
            //var p = _personService.Read("bfcf5e0c-64f8-448b-a30d-115f4756a60f");
            //_personService.Delete(p);
            //p.CreateDate = DateTime.Now;
            //_personService.Update(p);

            var persons = _personService.List().ToList();

            //var me = new MatchupEntryModel
            //{
            //    TeamCompeting = team,
            //    Score = 5,
            //    ParentMatchup = null
            //};

            

            return View();
        }

        [HttpPost]
        public ActionResult Index(ParseSqlStringViewModel model)
        {
            var results = new DataSet();

            try
            {
                results = _sqlParser.ParseSql(model.SqlString);

                model.QueryDataSet = results;
                model.ColumnNames = results.Tables[0].Columns.Cast<DataColumn>()
                                        .Select(x => x.ColumnName)
                                        .ToArray();

                ViewBag.Error = false;
            }
            catch (Exception ex)
            {
                ViewBag.Error = true;
                ViewBag.ErrorMsg = "Något gick åt helvete när jag skulle tyda din taskigt skrivna Sql statement, så jag gav upp!";
                //Log(ex);
            }


            return View(model);
        }
    }
}