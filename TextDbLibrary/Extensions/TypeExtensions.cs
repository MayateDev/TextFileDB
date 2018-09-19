using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
//using DomainLibrary.Interfaces;
using TextDbLibrary.Interfaces;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Enums;
using TextDbLibrary.Classes;

namespace TextDbLibrary.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns a filepath from filename string
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string FullFilePath(this string fileName)
        {
            return TextDbSchema.DbTextFilesPath + "\\" + fileName;
        }

        /// <summary>
        /// Loads file from filepath and reads it to List<string>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        internal static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Takes a TextDbFile, reads it and converts to a lsit of entities
        /// </summary>
        /// <typeparam name="T">Generic type for our return</typeparam>
        /// <param name="lines">The List<string> we are working on</param>
        /// <param name="tblSet">The table we are working with</param>
        /// <param name="csvHeaderString">Header string for Csv</param>
        /// <returns></returns>
        internal static List<T> ConvertLinesToEntities<T>(this List<string> lines, IDbTableSet tblSet) //, out string csvHeaderString
        {
            List<T> output = new List<T>();

            CheckAndRemoveCsvHeader(lines, tblSet);

            foreach (string l in lines)
            {
                T entity = TextDbHelpers.ConvertTextDbLineToEntity<T>(l, tblSet);
                output.Add(entity);
            }
            return output;
        }

        /// <summary>
        /// Get the next id for table
        /// </summary>
        /// <param name="tblSet">Table you want new id for</param>
        /// <returns></returns>
        internal static int GetNewId(this IDbTableSet tblSet)
        {
            var pkDict = TextDbHelpers.DbPrimaryKeyDictionary();

            if (pkDict.ContainsKey(tblSet.TableName))
            {
                return pkDict[tblSet.TableName] + 1;
            }

            throw new KeyNotFoundException("The primary key for table: " + tblSet.TableName + " could not be retreived.");
        }

        /// <summary>
        /// Sets the new current Primary Key in TextDbInfo file
        /// </summary>
        /// <param name="tblSet">Table current operating on</param>
        internal static void SetNewPrimaryKeyInDbInfoFile(this IDbTableSet tblSet, int addToCount)
        {
            int rowPos;
            var dbInfoLines = TextDbSchema.DbInfoFile.FullFilePath().LoadFile();
            var editLine = dbInfoLines.FirstOrDefault(l => l.Contains("[" + tblSet.TableName + "] [PrimaryKeyCount="));

            if (editLine != null)
            {
                if (addToCount > 0)
                {
                    addToCount--;
                }

                int pkStartPos = editLine.IndexOf("PrimaryKeyCount=") + 16;
                int pkEndPos = editLine.IndexOf(']', pkStartPos);
                int currentPk =int.Parse(editLine.Substring(pkStartPos, pkEndPos - pkStartPos));
                rowPos = dbInfoLines.IndexOf(editLine);

                dbInfoLines[rowPos] = dbInfoLines[rowPos].Replace(currentPk.ToString(), (currentPk + 1 + addToCount).ToString());

                File.WriteAllLines(TextDbSchema.DbInfoFile.FullFilePath(), dbInfoLines);
            }
        }

        /// <summary>
        /// Helper method that takes the csv header string, checks it and coreccts it if faulty.
        /// </summary>
        /// <param name="lines">Lines from DbText file</param>
        /// <param name="tblSet">Table we are working on</param>
        /// <returns>A new CsvHeader generated from passed IDbTableSet</returns>
        private static string CheckAndRemoveCsvHeader(List<string> lines, IDbTableSet tblSet)
        {
            string csvHeaderString = "";
            var headerLine = lines[0];
            var generatedHeaderLine = TextDbHelpers.GenerateCsvHeader(tblSet);

            if (headerLine == generatedHeaderLine)
            {
                csvHeaderString = lines[0];
                lines.Remove(lines[0]);
            }
            else
            {
                bool colsValid = true;
                var headerCols = headerLine.Split(';');

                foreach (var s in headerCols)
                {
                    if (!(s.Substring(0, 1) == "[" && s.Substring(s.Length - 1, 1) == ")"))
                    {
                        colsValid = false;
                    }
                }

                if (colsValid)
                {
                    lines.Remove(lines[0]);
                }

                csvHeaderString = generatedHeaderLine;
            }

            return csvHeaderString;
        }
    }
}
