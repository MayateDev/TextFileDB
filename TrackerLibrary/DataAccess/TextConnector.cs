using System;
using System.Collections.Generic;
using DataLibrary;
using DomainLibrary.Interfaces;
using DomainLibrary.Models;
using TextDbLibrary.Classes;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Extensions;
using TextDbLibrary.Interfaces;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        public PersonModel CreatePerson(PersonModel model)
        {
            var db = new DbContext();
            db.PersonsTbl.Add(model);

            return model;
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
            var db = new DbContext();
            

            return model;
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            //string csvHeaderString;
            //IDbTableSet tblSet = DbContext.TeamsTbl;

            //List<TeamModel> teams = tblSet.DbTextFile
            //    .FullFilePath()
            //    .LoadFile()
            //    .ConvertToModels<TeamModel>(tblSet, out csvHeaderString);

            //model.Id = tblSet.GetNewId();

            //teams.Add(model);
            //teams.SaveModelsToFile<TeamModel>(tblSet, csvHeaderString);

            var db = new DbContext();
            db.TeamsTbl.Add(model);

            return model;
        }
    }
}
