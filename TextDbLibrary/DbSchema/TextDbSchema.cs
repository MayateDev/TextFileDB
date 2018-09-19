using System.Collections.Generic;
using System.IO;
using TextDbLibrary.Classes;
using TextDbLibrary.Enums;
using TextDbLibrary.Extensions;
using TextDbLibrary.Interfaces;
using System.Linq;
using System.Reflection;
using System;

namespace TextDbLibrary.DbSchema
{
    /// <summary>
    /// Static class for setting upp the Schema for our TextDb
    /// </summary>
    public class TextDbSchema
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public TextDbSchema()
        {
            Tables = SchemaTables;
        }

        /// <summary>
        /// A private static List of IDbTableSet where we instanciate all our tables
        /// </summary>
        private static List<IDbTableSet> SchemaTables { get; set; }

        /// <summary>
        /// Private backingfield for Tables
        /// </summary>
        private IReadOnlyList<IDbTableSet> _tables { get; set; }

        /// <summary>
        /// A public List of IDbTableSet where we instanciate all our tables
        /// </summary>
        public IReadOnlyList<IDbTableSet> Tables
        {
            get
            {
                return _tables;
            }

            private set
            {
                _tables = value;
            }
        }

        /// <summary>
        /// Folder where we want to store our DbText files
        /// </summary>
        internal static string DbTextFilesPath
        {
            get
            {
                var value = TextDbHelpers.AppSettingLookUp("TextDbFolderPath");

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                throw new FileNotFoundException("The AppSetting for TextDbFolderPath is null or not set.");
            }
        }

        /// <summary>
        /// Const string with the filename of our DbInfo file
        /// </summary>
        internal const string DbInfoFile = "TextDbInfo.tdb";

        /// <summary>
        /// Const string with the filename of our DbInfo Json file
        /// </summary>
        internal const string DbInfoJsonFile = "TextDbInfo.json";

        /// <summary>
        /// Gets all the records from a specific table with a specific return type
        /// </summary>
        /// <typeparam name="T">The return type you expect from that table</typeparam>
        /// <param name="tableName">The name of the table you are quering</param>
        /// <returns>Returns a List of records with generic type</returns>
        public List<T> GetAllRecordsFromTableAsEntities<T>(string tableName)
        {
            var tblSet = SchemaTables.Find(t => t.TableName == tableName);

            return tblSet.DbTextFile.FullFilePath().LoadFile().ConvertLinesToEntities<T>(tblSet);
        }

        /// <summary>
        /// Initializer for our DbText files. If they not exists they will be created.
        /// If they exist then we check that the first line contains TableInfo data,
        /// if it doesn't we check for highest Id and create new TableInfo data.
        /// </summary>
        public void InitializeTextDb()
        {
            TextDbHelpers.CreateTextDataFilesIfNotExists(SchemaTables);
            TextDbHelpers.WriteNewDbInfoFile(SchemaTables);
            TextDbTableActions.EntityDeletedFromFileEvent += TextDbTableActions_EntityDeletedFromFileEvent;
        }

        // vid problem testa att skapa private static bool subscribed för att hålla koll om event är reggat
        /// <summary>
        /// Method that runs when EntityDeletedFromFileEvent gets invoked
        /// </summary>
        /// <param name="sender">Object who called invoke on event</param>
        /// <param name="e">EventArgs for the EntityDeletedFromFileEvent</param>
        private void TextDbTableActions_EntityDeletedFromFileEvent(object sender, EntityDeletedEventArgs e)
        {
            foreach (var tbl in SchemaTables)
            {
                var columns = tbl.Columns.Where(
                    c =>
                        (c as IDbRelationship != null) &&
                        (c as IDbRelationship).RelationshipReturnType == e.DeletedType
                    ).ToList();

                if (columns.Count > 0)
                {
                    var textDbFile = tbl.DbTextFile.FullFilePath();
                    List<string> entities = tbl.DbTextFile
                        .FullFilePath()
                        .LoadFile();

                    TextDbHelpers.CleanUpDeletedRelationsInEntities(entities, e.DeletedId, columns);

                    File.WriteAllLines(textDbFile, entities);
                }
            }
            e.DeleteRelationsSucceded = true;
        }

        // TODO - Behöver jag båda två?
        /// <summary>
        /// A method that should get called from user class derived from this class
        /// </summary>
        /// <param name="tables">A list of all the tables in our TextDb database</param>
        protected void SetTablesList(List<IDbTableSet> tables)
        {
            if (SchemaTables == null || SchemaTables.Count == 0)
            {
                SchemaTables = tables;
            }

            if (Tables == null || Tables.Count == 0)
            {
                Tables = tables;
            }
        }
    }
}
