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
                    new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.IntType),
                    new DbColumn("FirstName", 1, ColumnDataType.StringType),
                    new DbColumn("LastName", 2, ColumnDataType.StringType),
                    new DbColumn("EmailAddress", 3, ColumnDataType.StringType),
                    new DbColumn("CellphoneNumber", 4, ColumnDataType.StringType),
                    new DbRelationshipColumn("Prize", 5, ColumnDataType.MultipleRelationships, typeof(Prize), "PrizesTbl")
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
                    new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.IntType),
                    new DbParseableColumn<int>("PlaceNumber", 1, ColumnDataType.IntType),
                    new DbColumn("PlaceName", 2, ColumnDataType.StringType),
                    new DbParseableColumn<decimal>("PrizeAmount", 3, ColumnDataType.DecimalType),
                    new DbParseableColumn<double>("PrizePercentage", 4, ColumnDataType.DoubleType)
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
                    new DbPrimaryKeyColumn<int>("Id", 0, ColumnDataType.IntType),
                    new DbRelationshipColumn("TeamMembers", 1, ColumnDataType.MultipleRelationships, typeof(Person), "PersonsTbl"),
                    new DbColumn("TeamName", 2, ColumnDataType.StringType)
                };
                string dbTextFile = "TeamModels.csv";
                string tableName = "TeamsTbl";

                var tblSet = new DbTableSet<Team>(columns, dbTextFile, tableName);

                return tblSet;
            }
        }

        public void Dispose()
        {
            this.Dispose();
        }

        //public IDbTableSet TeamsTbl
        //{
        //    get
        //    {
        //        var tblSet = new DbTableSet<TeamModel>();

        //        tblSet.Columns = new List<IDbColumn>
        //        {
        //            new DbParseableColumn<int> { ColumnName = "Id", ColumnPosition = 0, DataType = ColumnDataType.IntType },
        //            new DbRelationshipColumn { ColumnName = "TeamMembers", ColumnPosition = 1, DataType = ColumnDataType.MultipleRelationships, ToTable = "PersonsTbl", RelationshipReturnType = typeof(PersonModel) },
        //            new DbColumn { ColumnName = "TeamName", ColumnPosition = 2, DataType = ColumnDataType.StringType }
        //        };
        //        tblSet.DbTextFile = "TeamModels.csv";
        //        tblSet.TableName = "TeamsTbl";

        //        return tblSet;
        //    }
        //}
    }
}
