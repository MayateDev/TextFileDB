using DataLibrary.Entities;
using System;
using System.Collections.Generic;
using TextDbLibrary.TableClasses;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Enums;
using TextDbLibrary.Interfaces;

namespace DataLibrary
{
    public class DbContext : TextDbSchema, IDisposable
    {
        public DbContext()
            //: base (new List<IDbTableSet> { PersonsTbl, PrizesTbl, TeamsTbl })
        {
            var tables = new List<IDbTableSet>();
            tables.Add(PersonsTbl);
            tables.Add(PrizesTbl);
            tables.Add(TeamsTbl);

            SetTablesList(tables);
        }

        public IDbTableSet PersonsTbl
        {
            get
            {
                IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
                {
                    new DbPrimaryKeyColumn<string>("Id", 0, ColumnDataType.String),
                    new DbColumn("FirstName", 1, ColumnDataType.String),
                    new DbColumn("LastName", 2, ColumnDataType.String),
                    new DbColumn("EmailAddress", 3, ColumnDataType.String),
                    new DbColumn("CellphoneNumber", 4, ColumnDataType.String),
                    new DbParseableColumn<DateTime>("CreateDate", 5, ColumnDataType.DateTime),
                    new DbRelationshipColumn("Prize", 6, ColumnDataType.MultipleRelationships, typeof(Prize), "PrizesTbl")
                };
                string dbTextFile = "PersonModels.csv";
                string tableName = "PersonsTbl";

                var tblSet = new DbTableSet<Person>(columns, dbTextFile, tableName);

                return tblSet;
            }
        }

        public IDbTableSet PrizesTbl
        {
            get
            {
                IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
                {
                    new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.Int),
                    new DbParseableColumn<int>("PlaceNumber", 1, ColumnDataType.Int),
                    new DbColumn("PlaceName", 2, ColumnDataType.String),
                    new DbParseableColumn<decimal>("PrizeAmount", 3, ColumnDataType.Decimal),
                    new DbParseableColumn<double>("PrizePercentage", 4, ColumnDataType.Double)
                };
                string dbTextFile = "PrizeModels.csv";
                string tableName = "PrizesTbl";

                var tblSet = new DbTableSet<Prize>(columns, dbTextFile, tableName);

                return tblSet;
            }
        }

        public IDbTableSet TeamsTbl
        {
            get
            {
                IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
                {
                    new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.Int),
                    new DbRelationshipColumn("TeamMembers", 1, ColumnDataType.MultipleRelationships, typeof(Person), "PersonsTbl"),
                    new DbColumn("TeamName", 2, ColumnDataType.String)
                };
                string dbTextFile = "TeamModels.csv";
                string tableName = "TeamsTbl";

                var tblSet = new DbTableSet<Team>(columns, dbTextFile, tableName);

                return tblSet;
            }
        }

        public IDbTableSet MatchupsTbl
        {
            get
            {
                IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
                {
                    new DbPrimaryKeyColumn<string>("Id", 0, ColumnDataType.String),
                    new DbRelationshipColumn("Entries", 1, ColumnDataType.MultipleRelationships, typeof(MatchupEntry), "MatchupEntriesTbl"),
                    new DbRelationshipColumn("Winner", 2, ColumnDataType.SingleRelationship, typeof(Team), "PrizesTbl"),
                    new DbParseableColumn<int>("MatchupRound", 3, ColumnDataType.Int)
                };
                string dbTextFile = "PersonModels.csv";
                string tableName = "PersonsTbl";

                var tblSet = new DbTableSet<Person>(columns, dbTextFile, tableName);

                return tblSet;
            }
        }

        public IDbTableSet MatchupEntriesTbl
        {
            get
            {
                IReadOnlyList<IDbColumn> columns = new List<IDbColumn>
                {
                    new DbPrimaryKeyColumn<string>("Id", 0, ColumnDataType.String),
                    new DbRelationshipColumn("TeamCompeting", 1, ColumnDataType.SingleRelationship, typeof(Team), "TeamsTbl"),
                    new DbParseableColumn<double>("Score", 2, ColumnDataType.Double),
                    new DbRelationshipColumn("ParentMatchup", 3, ColumnDataType.SingleRelationship, typeof(Matchup), "MatchupsTbl")
                };
                string dbTextFile = "PersonModels.csv";
                string tableName = "PersonsTbl";

                var tblSet = new DbTableSet<Person>(columns, dbTextFile, tableName);

                return tblSet;
            }
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}
