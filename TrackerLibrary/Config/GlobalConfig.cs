using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;
using DomainLibrary.Enums;
using DomainLibrary.Interfaces;
using TextDbLibrary.DbSchema;
using DataLibrary;

namespace TrackerLibrary.Config
{
    public static class GlobalConfig
    {
        public static void InitializeConnection()
        {
            var db = new DbContext();
            db.InitializeTextDb();
        }
    }
}
