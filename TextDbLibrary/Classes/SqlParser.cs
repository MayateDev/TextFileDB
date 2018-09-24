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
        /// <summary>
        /// A method that takes an simple sql string of format: Select [ColumnName], [ColumnName] From [TableName] Where [ColumnName] >= 10
        /// the Where clause is optional and you can also use a wildcard "*" like this: Select * From [TableName]
        /// </summary>
        /// <param name="sqlString">Sql SELECT statement</param>
        /// <returns></returns>
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

        /// <summary>
        /// A Dictionary with all the columns of the selected table
        /// </summary>
        /// <param name="selectedTable">Name of the table we are working on</param>
        /// <param name="columns">List of columns of that table</param>
        /// <param name="selectedColumnsList">Columnnames extracted from sql statement</param>
        /// <returns>Dictionary with ColumnName as key, and ColumnPosition as value</returns>
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

        /// <summary>
        /// Creates a new DataSet with a table and the columns from our select statement
        /// </summary>
        /// <param name="selectedColumnsDict">Dictionary with the selected columns</param>
        /// <param name="columns">List of all our columns in the table</param>
        /// <returns></returns>
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

                if (columnDataType == ColumnDataType.Int)
                {
                    dtColumnType = typeof(int);
                }
                else if (columnDataType == ColumnDataType.Decimal)
                {
                    dtColumnType = typeof(decimal);
                }
                else if (columnDataType == ColumnDataType.Double)
                {
                    dtColumnType = typeof(double);
                }
                else if (columnDataType == ColumnDataType.DateTime)
                {
                    dtColumnType = typeof(DateTime);
                }
                else if ((columnDataType == ColumnDataType.String) || columnDataType == ColumnDataType.SingleRelationship || columnDataType == ColumnDataType.MultipleRelationships)
                {
                    dtColumnType = typeof(string);
                }

                queryTable.Columns.Add(tableName, dtColumnType);
            }

            return queryDataSet;
        }

        /// <summary>
        /// Creates a new DataRow in out DataSet.Table
        /// </summary>
        /// <param name="queryDataSet">The DataSet we wish to insert a row in</param>
        /// <param name="selectedColumnsDict">Selected columns dicitionary</param>
        /// <param name="columns">All the columns in the table</param>
        /// <param name="cols">Row in string[] format</param>
        private void CreateDataRow(ref DataSet queryDataSet, Dictionary<string, int> selectedColumnsDict, IReadOnlyList<IDbColumn> columns, string[] cols)
        {
            DataRow queryDataRow;
            queryDataRow = queryDataSet.Tables["QueryData"].NewRow();

            foreach (var c in selectedColumnsDict)
            {
                ColumnDataType columnDataType = columns[c.Value].DataType;

                SetQueryDataColumn(ref queryDataRow, c, cols, columnDataType);
            }

            queryDataSet.Tables["QueryData"].Rows.Add(queryDataRow);
        }

        /// <summary>
        /// Parses and sets value to a DataRow
        /// </summary>
        /// <param name="queryDataRow">THe DataRow we wish to set values to</param>
        /// <param name="c">KeyValuePair of string and int, Tablename and Columnposition</param>
        /// <param name="cols">Row in string[] format</param>
        /// <param name="columnDataType">ColumnDataType enum</param>
        private void SetQueryDataColumn(ref DataRow queryDataRow, KeyValuePair<string, int> c, string[] cols, ColumnDataType columnDataType)
        {
            if (columnDataType == ColumnDataType.Int)
            {
                queryDataRow[c.Key] = int.Parse(cols[c.Value]);
            }
            else if (columnDataType == ColumnDataType.Decimal)
            {
                queryDataRow[c.Key] = decimal.Parse(cols[c.Value]);
            }
            else if (columnDataType == ColumnDataType.Double)
            {
                queryDataRow[c.Key] = double.Parse(cols[c.Value]);
            }
            else if (columnDataType == ColumnDataType.DateTime)
            {
                if (cols[c.Value] != null && cols[c.Value] != "")
                {
                    queryDataRow[c.Key] = Convert.ToDateTime(cols[c.Value]);
                }
                else
                {
                    queryDataRow[c.Key] = default(DateTime);
                }
            }
            else if ((columnDataType == ColumnDataType.String) || columnDataType == ColumnDataType.SingleRelationship || columnDataType == ColumnDataType.MultipleRelationships)
            {
                queryDataRow[c.Key] = cols[c.Value];
            }
        }

        /// <summary>
        /// Formats the conditionString from our select statement
        /// </summary>
        /// <param name="columns">All columns of a table</param>
        /// <param name="tmpConditionsString">String to format</param>
        /// <param name="cols">Row in strng[] format</param>
        private void FormatConditionString(IReadOnlyList<IDbColumn> columns, ref string tmpConditionsString, string[] cols)
        {
            var relationshipColumns = columns.Where(c => c as IDbRelationshipColumn != null).ToList();
            var conditions = tmpConditionsString.Split(new string[] { "&&", "||", "&", "|" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var andOrOperatorsString = "";

            GetAndOrOperators(ref andOrOperatorsString, tmpConditionsString);

            string[] andOrOperatorsArray;

            if (andOrOperatorsString == "")
            {
                andOrOperatorsArray = new string[0];
            }
            else
            {
                andOrOperatorsArray = andOrOperatorsString.Split(' ');
            }

            LookForAndEvaluateRealationshipConditions(ref conditions, relationshipColumns, cols);

            tmpConditionsString = "";

            for (int i = 0; i < conditions.Count; i++)
            {
                tmpConditionsString += conditions[i].Trim();

                if (andOrOperatorsArray.Length >= (i + 1))
                {
                    tmpConditionsString += " " + andOrOperatorsArray[i] + " ";
                }
            }

            foreach (var c in columns)
            {
                if (c.DataType == ColumnDataType.String || 
                    c.DataType == ColumnDataType.SingleRelationship || 
                    c.DataType == ColumnDataType.MultipleRelationships || 
                    c.DataType == ColumnDataType.DateTime)
                {
                    tmpConditionsString = tmpConditionsString.Replace("[" + c.ColumnName + "]", "\"" + cols[c.ColumnPosition] + "\"");
                }
                else
                {
                    tmpConditionsString = tmpConditionsString.Replace("[" + c.ColumnName + "]", cols[c.ColumnPosition]);
                }
            }
        }

        /// <summary>
        /// Get the And & Or operators fron conditions string
        /// </summary>
        /// <param name="andOrOperatorsString">Variable we are setting string to</param>
        /// <param name="tmpConditionsString">Conditionstring to get operators from</param>
        private static void GetAndOrOperators(ref string andOrOperatorsString, string tmpConditionsString)
        {
            for (int i = 0; i < tmpConditionsString.Length; i++)
            {
                if (tmpConditionsString[i] == '&' || tmpConditionsString[i] == '&')
                {
                    andOrOperatorsString += tmpConditionsString[i];

                    if (tmpConditionsString[i + 1] != tmpConditionsString[i])
                    {
                        andOrOperatorsString += " ";
                    }
                }
            }
        }

        /// <summary>
        /// Looks for relationship columns in selected table
        /// </summary>
        /// <param name="conditions">List of conditions extracted from condition string</param>
        /// <param name="relationshipColumns">Columns in table that is a relationship column</param>
        /// <param name="cols">String array of the current row from database that we want to check if it matches the statement</param>
        private static void LookForAndEvaluateRealationshipConditions(ref List<string> conditions, List<IDbColumn> relationshipColumns, string[] cols)
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                foreach (var c in relationshipColumns)
                {
                    if (conditions[i].Contains("[" + c.ColumnName + "]"))
                    {
                        var relationships = cols[c.ColumnPosition];
                        var relationshipsArray = relationships.Split('^');


                        var startChar = "";
                        var endChar = "";
                        var value = conditions[i];

                        if (value.StartsWith("("))
                        {
                            startChar = "(";
                        }

                        if (value.EndsWith(")"))
                        {
                            endChar = ")";
                        }

                        value = conditions[i].Replace("[" + c.ColumnName + "]", "")
                            .Replace(" ", "")
                            .Replace("(", "")
                            .Replace(")", "");

                        DoRelationshipConditionEvaluation(ref conditions, value, i, relationshipsArray, startChar, endChar);
                    }
                }
            }
        }

        /// <summary>
        /// Does the evaluation for each relationship column based on what operator is found in condition
        /// </summary>
        /// <param name="conditions">List of conditions extracted from condition string</param>
        /// <param name="value">Value extracted from condition with operator</param>
        /// <param name="i">Iterator from for loop in passing method</param>
        /// <param name="relationshipsArray">An array of relationship ids</param>
        /// <param name="startChar">If this is the first conditon in a ( x = 1 && x = 2 ) then a ( needs to be added to returned value</param>
        /// <param name="endChar">If this is the last conditon in a ( x = 1 && x = 2 ) then a ) needs to be added to returned value</param>
        private static void DoRelationshipConditionEvaluation(ref List<string> conditions, string value, int i, string[] relationshipsArray, string startChar, string endChar)
        {
            if (conditions[i].Contains("=="))
            {
                value = value.Replace("==", "");

                if (relationshipsArray.Contains(value))
                {
                    conditions[i] = startChar + "true" + endChar;
                }
                else
                {
                    conditions[i] = startChar + "false" + endChar;
                }

            }
            else if (conditions[i].Contains("!="))
            {
                value = value.Replace("!=", "");

                if (relationshipsArray.Contains(value))
                {
                    conditions[i] = startChar + "false" + endChar;
                }
                else
                {
                    conditions[i] = startChar + "true" + endChar;
                }
            }
            else if (conditions[i].Contains("<="))
            {
                value = value.Replace("<=", "");

                int intValue = int.Parse(value);

                if (relationshipsArray.Count() <= intValue)
                {
                    conditions[i] = startChar + "true" + endChar;
                }
                else
                {
                    conditions[i] = startChar + "false" + endChar;
                }
            }
            else if (conditions[i].Contains(">="))
            {
                value = value.Replace(">=", "");

                int intValue = int.Parse(value);

                if (relationshipsArray.Count() >= intValue)
                {
                    conditions[i] = startChar + "true" + endChar;
                }
                else
                {
                    conditions[i] = startChar + "false" + endChar;
                }
            }
            else if (conditions[i].Contains(">"))
            {
                value = value.Replace(">", "");

                int intValue = int.Parse(value);

                if (relationshipsArray.Count() > intValue)
                {
                    conditions[i] = startChar + "true" + endChar;
                }
                else
                {
                    conditions[i] = startChar + "false" + endChar;
                }
            }
            else if (conditions[i].Contains("<"))
            {
                value = value.Replace("<", "");

                int intValue = int.Parse(value);

                if (relationshipsArray.Count() < intValue)
                {
                    conditions[i] = startChar + "true" + endChar;
                }
                else
                {
                    conditions[i] = startChar + "false" + endChar;
                }
            }
        }

        /// <summary>
        /// Check if the select statement is a * statement
        /// </summary>
        /// <param name="selectedColumnsList">ist of selected columns</param>
        /// <param name="columns">List of all columns in a table</param>
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

        /// <summary>
        /// Extracts the first statement (Select)
        /// </summary>
        /// <param name="currPos">Current position in string</param>
        /// <param name="nextPos">Next position in string</param>
        /// <param name="_sqlString">The sql statement string</param>
        /// <returns></returns>
        private string GetFirstStatement(ref int currPos, ref int nextPos, string _sqlString)
        {
            string firstStatement = _sqlString.Substring(currPos, nextPos);

            return firstStatement;
        }

        /// <summary>
        /// Extracts the selected columns
        /// </summary>
        /// <param name="currPos">Current position in string</param>
        /// <param name="nextPos">Next position in string</param>
        /// <param name="_sqlString">The sql statement string</param>
        /// <returns></returns>
        private List<string> GetSelectedColumnsList(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos;
            nextPos = _sqlString.IndexOf(" F", currPos);

            var selectedColumnsList = _sqlString.Substring(currPos, nextPos - currPos).Trim().Replace(" ", "").Split(',').ToList();

            return selectedColumnsList;
        }

        /// <summary>
        /// Extracts the second statement (From)
        /// </summary>
        /// <param name="currPos">Current position in string</param>
        /// <param name="nextPos">Next position in string</param>
        /// <param name="_sqlString">The sql statement string</param>
        /// <returns></returns>
        private string GetSecondStatement(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos + 1;
            nextPos = _sqlString.IndexOf(" [", currPos);

            string secondStatement = _sqlString.Substring(currPos, nextPos - currPos);

            return secondStatement;
        }

        /// <summary>
        /// Extracts the selected table
        /// </summary>
        /// <param name="currPos">Current position in string</param>
        /// <param name="nextPos">Next position in string</param>
        /// <param name="_sqlString">The sql statement string</param>
        /// <returns></returns>
        private string GetSelectedTable(ref int currPos, ref int nextPos, string _sqlString)
        {
            currPos = nextPos + 1;
            nextPos = _sqlString.IndexOf("]", currPos);

            string selectedTable = _sqlString.Substring(currPos, nextPos - currPos).Trim();
            selectedTable = selectedTable.Replace("[", "").Replace("]", "");

            return selectedTable;
        }

        /// <summary>
        /// Extrcts the third statement (Where)
        /// </summary>
        /// <param name="currPos">Current position in string</param>
        /// <param name="nextPos">Next position in string</param>
        /// <param name="_sqlString">The sql statement string</param>
        /// <returns></returns>
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

        /// <summary>
        /// Extracts the conditions string
        /// </summary>
        /// <param name="currPos">Current position in string</param>
        /// <param name="nextPos">Next position in string</param>
        /// <param name="_sqlString">The sql statement string</param>
        /// <returns></returns>
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
