using System.Collections.Generic;
using System.IO;
using TrackerLibrary.Models;
using TrackerLibrary.TextDb.Classes;
using TrackerLibrary.TextDb.Enums;
using TrackerLibrary.TextDb.Extensions;
using TrackerLibrary.TextDb.Interfaces;

namespace TrackerLibrary.TextDb.DbSchema
{
    public static class TextDbSchema
    {
        public static List<IDbTableSet> Tables { get; private set; } = 
            new List<IDbTableSet>
                {
                    PersonsTbl,
                    PrizesTbl
                };
        public const string DbTextFilesPath = "c:\\temp";

        public static IDbTableSet PersonsTbl
        {
            get
            {
                var tbl = new DbTableSet();

                tbl.Columns = new List<IDbColumn>
                {
                    new DbParseableColumn<int> { ColumnName = "Id", ColumnPosition = 0, DataType = ColumnDataType.IntType },
                    new DbColumn { ColumnName = "FirstName", ColumnPosition = 1, DataType = ColumnDataType.StringType },
                    new DbColumn { ColumnName = "LastName", ColumnPosition = 2, DataType = ColumnDataType.StringType },
                    new DbColumn { ColumnName = "EmailAddress", ColumnPosition = 3, DataType = ColumnDataType.StringType },
                    new DbColumn { ColumnName = "CellphoneNumber", ColumnPosition = 4, DataType = ColumnDataType.StringType },
                    new DbRelationshipColumn { ColumnName = "Prize", ColumnPosition = 5, DataType = ColumnDataType.MultipleRelationships, ToTable = "PrizesTbl", RelationshipReturnType = typeof(PrizeModel) }
                };
                tbl.DbTextFile = "PersonModels.csv";
                tbl.TableName = "PersonsTbl";

                return tbl;
            }
        }

        public static IDbTableSet PrizesTbl
        {
            get
            {
                var tbl = new DbTableSet();

                tbl.Columns = new List<IDbColumn>
                {
                    new DbParseableColumn<int> { ColumnName = "Id", ColumnPosition = 0, DataType = ColumnDataType.IntType },
                    new DbParseableColumn<int> { ColumnName = "PlaceNumber", ColumnPosition = 1, DataType = ColumnDataType.IntType },
                    new DbColumn { ColumnName = "PlaceName", ColumnPosition = 2, DataType = ColumnDataType.StringType },
                    new DbParseableColumn<decimal> { ColumnName = "PrizeAmount", ColumnPosition = 3, DataType = ColumnDataType.DecimalType },
                    new DbParseableColumn<double> { ColumnName = "PrizePercentage", ColumnPosition = 4, DataType = ColumnDataType.DoubleType }
                };
                tbl.DbTextFile = "PrizeModels.csv";
                tbl.TableName = "PrizesTbl";

                return tbl;
            }
        }

        // TODO - Slutför denna
        public static void InitializeDbTextFiles()
        {
            foreach (var tbl in Tables)
            {
                var file = tbl.DbTextFile.FullFilePath();
                if (!File.Exists(file))
                {
                    // <![TableInfo]>[PKCount=4256],[Columns=Id(IntType),FirstName(StringType),LastName(StringType),EmailAddress(StringType),CellphoneNumber(StringType),Prize(MultipleRelationships)]</[TableInfo]>
                    List<string> lines = new List<string>();
                    string tblString = "<![TableInfo]>[PKCount=0],[Columns=";

                    foreach (var column in tbl.Columns)
                    {
                        tblString += column.ColumnName + "(" + column.DataType + "),";
                    }

                    tblString = tblString.Substring(0, tblString.Length - 1);
                    tblString += "]</[TableInfo]>";

                    lines.Add(tblString);

                    File.WriteAllLines(file, lines);
                }
            }
        }

        public static List<T> GetAllRecordsFromTableAsModels<T>(string tableName)
        {
            string tableInfoString;
            var tblSet = Tables.Find(t => t.TableName == tableName);

            return tblSet.DbTextFile.FullFilePath().LoadFile().ConvertToModels<T>(tblSet, out tableInfoString);
        }
    }
}
