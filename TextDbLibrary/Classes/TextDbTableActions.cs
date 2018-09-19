﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Enums;
using TextDbLibrary.Extensions;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.Classes
{
    public static class TextDbTableActions
    {
        /// <summary>
        /// Event to handle deletion of relationship to deleted entity in all tables
        /// </summary>
        internal static event EventHandler<EntityDeletedEventArgs> EntityDeletedFromFileEvent;

        /// <summary>
        /// Extension method to Add a entity to the TextDb database
        /// </summary>
        /// <typeparam name="T">Type of the entity you want to add</typeparam>
        /// <param name="tblSet">Tableset we are working on</param>
        /// <param name="entity">The entity we want to add</param>
        /// <returns>Entity passed with new id</returns>
        public static T Add<T>(this IDbTableSet tblSet, T entity) where T : IEntity
        {
            var textDbFile = tblSet.DbTextFile.FullFilePath();
            List<string> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile();

            entity.Id = tblSet.GetNewId();
            var entityString = TextDbHelpers.ConvertEntityToTextDbLine(entity, tblSet);
            entities.Add(entityString);

            File.WriteAllLines(textDbFile, entities);

            tblSet.SetNewPrimaryKeyInDbInfoFile(0);

            return entity;
        }

        /// <summary>
        /// Extension method to read a entity with specific id from a table
        /// </summary>
        /// <typeparam name="T">Type for the entity passed</typeparam>
        /// <param name="tblSet">Tableset we are working on</param>
        /// <param name="id">Id of the entity we want to get</param>
        /// <returns>Entity with the same id as requested</returns>
        public static T Read<T>(this IDbTableSet tblSet, int id) where T : class, IEntity
        {
            var textDbFile = tblSet.DbTextFile.FullFilePath();
            List<string> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile();
            int rowPos = FindRowNumberForId(entities, tblSet, id);

            var entity = TextDbHelpers.ConvertTextDbLineToEntity<T>(entities[rowPos], tblSet);

            return entity;
        }

        /// <summary>
        /// Extension method to update a entity in the database
        /// </summary>
        /// <typeparam name="T">Type of the entity you want to update</typeparam>
        /// <param name="tblSet">Tableset we are working with</param>
        /// <param name="entity">Entity you want to update</param>
        /// <returns>Entity passed</returns>
        public static T Update<T>(this IDbTableSet tblSet, T entity) where T : IEntity
        {
            var textDbFile = tblSet.DbTextFile.FullFilePath();
            List<string> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile();

            var updateId = entity.Id;

            var rowPos = FindRowNumberForId(entities, tblSet, updateId);
            var entityString = TextDbHelpers.ConvertEntityToTextDbLine(entity, tblSet);
            entities[rowPos] = entityString;

            File.WriteAllLines(textDbFile, entities);

            return entity;
        }

        /// <summary>
        /// Extension method to list all entity in a table
        /// </summary>
        /// <typeparam name="T">Type of the entity we are working with</typeparam>
        /// <param name="tblSet">Tableset we are working with</param>
        /// <returns>A list of all entities in the table</returns>
        public static List<T> List<T>(this IDbTableSet tblSet) where T : IEntity
        {
            List<T> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile()
                .ConvertLinesToEntities<T>(tblSet);

            return entities;
        }

        /// <summary>
        /// Adds a list of entity to table
        /// </summary>
        /// <typeparam name="T">Type of the entity you want to add</typeparam>
        /// <param name="tblSet">The table we are working on</param>
        /// <param name="entityList">The list of entities we want to add</param>
        /// <returns>Passed list of entities with their new ids</returns>
        public static List<T> AddEntities<T>(this IDbTableSet tblSet, List<T> entityList) where T : IEntity
        {
            var textDbFile = tblSet.DbTextFile.FullFilePath();
            List<string> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile();

            int currentPk = tblSet.GetNewId();

            for (var i = 0; i < entityList.Count; i++)
            {
                entityList[i].Id = (currentPk + i);
                entities.Add(TextDbHelpers.ConvertEntityToTextDbLine(entityList[i], tblSet));
            }

            File.WriteAllLines(textDbFile, entities);

            tblSet.SetNewPrimaryKeyInDbInfoFile(entityList.Count);

            return entityList;
        }

        /// <summary>
        /// Extension method to delee a entity from the database
        /// </summary>
        /// <typeparam name="T">Type of the entity we want to delete</typeparam>
        /// <param name="tblSet">Tableset we are working with</param>
        /// <param name="entity">Entity we want to delete</param>
        public static void Delete<T>(this IDbTableSet tblSet, T entity) where T : IEntity
        {
            var textDbFile = tblSet.DbTextFile.FullFilePath();
            List<string> entities = tblSet.DbTextFile
                .FullFilePath()
                .LoadFile();

            var deleteId = entity.Id;

            var rowPos = FindRowNumberForId(entities, tblSet, deleteId);
            entities.RemoveAt(rowPos);

            var eventArgs = new EntityDeletedEventArgs(deleteId, tblSet.EntityType);

            EntityDeletedFromFileEvent?.Invoke(null, eventArgs);

            if (eventArgs.DeleteRelationsSucceded)
            {
                File.WriteAllLines(textDbFile, entities);
            }
            else
            {
                throw new OperationCanceledException("Something went wrong when trying to delete relationships in tables.");
            }
        }

        /// <summary>
        /// Helper method to check for reltionship ids in table after a delete have been made in the database
        /// </summary>
        /// <param name="tblSet">Tableset we want to check for relationships</param>
        /// <returns></returns>
        private static int CheckForIdColumnAndReturnPosition(IDbTableSet tblSet)
        {
            var column = tblSet.Columns.FirstOrDefault(c => c as IDbPrimaryKey != null);

            if (column != null)
            {
                return column.ColumnPosition;
            }

            throw new NullReferenceException("Table error. There is no column with the datatype of PrimaryKey in this table");
        }

        /// <summary>
        /// Helper method to find what row to work with when performing an read, update or delete
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="tblSet"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int FindRowNumberForId(List<string> entities, IDbTableSet tblSet, int id)
        {
            int colPos = CheckForIdColumnAndReturnPosition(tblSet);

            for (int i = 0; i < entities.Count; i++)
            {
                var cols = entities[i].Split(';');

                if (cols[colPos] == id.ToString())
                {
                    return i;
                }
            }
            throw new NullReferenceException("Table error. There is no row with a id that matches in this table");
        }
    }
}