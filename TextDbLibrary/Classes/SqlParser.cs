using DynamicExpresso;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Enums;
using TextDbLibrary.Extensions;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.Classes
{
    public class SqlParser
    {
        public DataSet ParseSql(string sqlString)
        {
            var _sqlString = Regex.Replace(sqlString, "select", "SELECT", RegexOptions.IgnoreCase);
            _sqlString = Regex.Replace(_sqlString, "from", "FROM", RegexOptions.IgnoreCase);
            _sqlString = Regex.Replace(_sqlString, "where", "WHERE", RegexOptions.IgnoreCase);

            var db = new TextDbSchema();

            int currPos = 0;
            int nextPos = _sqlString.IndexOf(' ', currPos);

            string firstStatement = GetFirstStatement(ref currPos, ref nextPos, _sqlString);
            List<string> selectedColumnsList = GetSelectedColumnsList(ref currPos, ref nextPos, _sqlString);
            string secondStatement = GetSecondStatement(ref currPos, ref nextPos, _sqlString);
            string selectedTable = GetSelectedTable(ref currPos, ref nextPos, _sqlString);

            string thirdStatement = "";
            string conditionsString = "";

            if (_sqlString.Contains("WHERE"))
            {
                thirdStatement = GetThirdStatement(ref currPos, ref nextPos, _sqlString);
                conditionsString = GetConditionsString(ref currPos, ref nextPos, _sqlString);
            }

            var tblSet = db.Tables.Where(t => t.TableName == selectedTable).FirstOrDefault();
            IfStarDoAllColumnsInsert(ref selectedColumnsList, tblSet.Columns);

            Dictionary<string, int> selectedColumnsDict = GetColumnsDictionary(selectedTable, tblSet.Columns, selectedColumnsList);
            DataSet queryDataSet = CreateDataSetTableAndColumns(selectedColumnsDict, tblSet.Columns);

            List<string> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile();
            entities.RemoveAt(0);

            foreach (var e in entities)
            {
                string tmpConditionsString = conditionsString;
                string[] cols = e.Split(';');

                FormatConditionString(tblSet.Columns, ref tmpConditionsString, cols);

                try
                {
                    bool conditionResult = false;

                    if (tmpConditionsString == "")
                    {
                        conditionResult = true;
                    }
                    else
                    {
                        conditionResult = new Interpreter().Eval<bool>(tmpConditionsString);
                    }

                    if (conditionResult)
                    {
                        CreateDataRow(ref queryDataSet, selectedColumnsDict, tblSet.Columns, cols);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not parse your sqlString.", ex);
                }
            }

            return queryDataSet;
        }

        private Dictionary<string, int> GetColumnsDictionary(string selectedTable, IReadOnlyList<IDbColumn> columns, List<string> selectedColumnsList)
        {
            Dictionary<string, int> selectedColumnsDict = new Dictionary<string, int>();

            foreach (var c in columns)
            {
                if (selectedColumnsList.Any(s => s.Contains(c.ColumnName)))
                {
                    selectedColumnsDict.Add(c.ColumnName, c.ColumnPosition);
                }
            }

            return selectedColumnsDict;
        }

        private DataSet CreateDataSetTableAndColumns(Dictionary<string, int> selectedColumnsDict, IReadOnlyList<IDbColumn> columns)
        {
            DataSet queryDataSet = new DataSet();
            queryDataSet.Locale = CultureInfo.InvariantCulture;

            DataTable queryTable = queryDataSet.Tables.Add("QueryData");

            for (int i = 0; i < selectedColumnsDict.Count; i++)
            {
                string tableName = selectedColumnsDict.ElementAt(i).Key;
                Type dtColumnType = typeof(string);
                ColumnDataType columnDataType = columns[selectedColumnsDict.ElementAt(i).Value].DataType;

                if (columnDataType == ColumnDataType.IntType)
                {
                    dtColumnType = typeof(int);
                }
                else if (columnDataType == ColumnDataType.DecimalType)
                {
                    dtColumnType = typeof(decimal);
                }
                else if (columnDataType == ColumnDataType.DoubleType)
                {
                    dtColumnType = typeof(double);
                }
                else if ((columnDataType == ColumnDataType.StringType) || columnDataType == ColumnDataType.SingleRelationship || columnDataType == ColumnDataType.MultipleRelationships)
                {
                    dtColumnType = typeof(string);
                }

                queryTable.Columns.Add(tableName, dtColumnType);
            }

            return queryDataSet;
        }

        private void CreateDataRow(ref DataSet queryDataSet, Dictionary<string, int> selectedColumnsDict, IReadOnlyList<IDbColumn> columns, string[] cols)
        {
            DataRow queryDataRow;
            queryDataRow = queryDataSet.Tables["QueryData"].NewRow();

            foreach (var c in selectedColumnsDict)
            {
                ColumnDataType columnDataType = columns[c.Value].DataType;

                SetQueryDataRow(ref queryDataRow, c, cols, columnDataType);
            }

            queryDataSet.Tables["QueryData"].Rows.Add(queryDataRow);
        }

        private void SetQueryDataRow(ref DataRow queryDataRow, KeyValuePair<string, int> c, string[] cols, ColumnDataType columnDataType)
        {
            if (columnDataType == ColumnDataType.IntType)
            {
                queryDataRow[c.Key] = int.Parse(cols[c.Value]);
            }
            else if (columnDataType == ColumnDataType.DecimalType)
            {
                queryDataRow[c.Key] = decimal.Parse(cols[c.Value]);
            }
            else if (columnDataType == ColumnDataType.DoubleType)
            {
                queryDataRow[c.Key] = double.Parse(cols[c.Value]);
            }
            else if ((columnDataType == ColumnDataType.StringType) || columnDataType == ColumnDataType.SingleRelationship || columnDataType == ColumnDataType.MultipleRelationships)
            {
                queryDataRow[c.Key] = cols[c.Value];
            }
        }

        private void FormatConditionString(IReadOnlyList<IDbColumn> columns, ref string tmpConditionsString, string[] cols)
        {
            foreach (var c in columns)
            {
                if (c.DataType == ColumnDataType.StringType || c.DataType == ColumnDataType.SingleRelationship || c.DataType == ColumnDataType.MultipleRelationships)
                {
                    tmpConditionsString = tmpConditionsString.Replace("[" + c.ColumnName + "]", "\"" + cols[c.ColumnPosition] + "\"");
                }
                else
                {
                    tmpConditionsString = tmpConditionsString.Replace("[" + c.ColumnName + "]", cols[c.ColumnPosition]);
                }
            }
        }

        private void IfStarDoAllColumnsInsert(ref List<string> selectedColumnsList, IReadOnlyList<IDbColumn> columns)
        {
            if (selectedColumnsList.Any(c => c.Contains("*")))
            {
                selectedColumnsList.Clear();

                foreach (var c in columns)
                {
                    selectedColumnsList.Add(c.ColumnName);
                }
            }
        }

        private string GetFirstStatement(ref int currPos, ref int nextPos, string _sqlString)
        {
            string firstStatement = _sqlString.Substring(currPos, nextPos);

            return firstStatement;
        }

        private List<string> GetSelectedColumnsList(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos;
            nextPos = _sqlString.IndexOf(" F", currPos);

            var selectedColumnsList = _sqlString.Substring(currPos, nextPos - currPos).Trim().Replace(" ", "").Split(',').ToList();

            return selectedColumnsList;
        }

        private string GetSecondStatement(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos + 1;
            nextPos = _sqlString.IndexOf(" [", currPos);

            string secondStatement = _sqlString.Substring(currPos, nextPos - currPos);

            return secondStatement;
        }

        private string GetSelectedTable(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos + 1;
            nextPos = _sqlString.IndexOf("]", currPos);

            string selectedTable = _sqlString.Substring(currPos, nextPos - currPos).Trim();
            selectedTable = selectedTable.Replace("[", "").Replace("]", "");

            return selectedTable;
        }

        private string GetThirdStatement(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos + 2;
            nextPos = _sqlString.IndexOf(' ', currPos);

            if (currPos < 0 || nextPos < 0)
            {
                return "";
            }

            string thirdStatement = _sqlString.Substring(currPos, nextPos - currPos);

            return thirdStatement;
        }

        private string GetConditionsString(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos;
            nextPos = _sqlString.Length;

            if (currPos < 0 || nextPos < 0)
            {
                return "";
            }

            string conditionsString = _sqlString.Substring(currPos, nextPos - currPos).Trim();
            conditionsString = conditionsString.Replace("'", "\"");

            return conditionsString;
        }
    }
}
