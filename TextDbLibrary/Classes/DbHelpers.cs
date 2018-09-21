//using DomainLibrary.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using TextDbLibrary.DbSchema;
using TextDbLibrary.Enums;
using TextDbLibrary.Extensions;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.Classes
{
    public static class DbHelpers
    {
        /// <summary>
        /// Get a IDbTableSet from TextDbSchema based on an entity type
        /// </summary>
        /// <param name="entityType">Type of the entity</param>
        /// <returns></returns>
        public static IDbTableSet GetTableSetFromEntity(Type entityType)
        {
            var dbSchema = new TextDbSchema();
            var tblSet = dbSchema.Tables.Where(t => t.EntityType == entityType).FirstOrDefault();

            return tblSet;
        }

        /// <summary>
        /// Converts a TextDb rowstring to entity
        /// </summary>
        /// <typeparam name="T">Type for entity</typeparam>
        /// <param name="line">Rowstring you want to convert</param>
        /// <param name="tblSet">The tableset we are workig on</param>
        /// <returns>Entity object of type T</returns>
        internal static T ConvertTextDbLineToEntity<T>(string line, IDbTableSet tblSet)
        {
            string[] cols = line.Split(';');

            T entity = (T)Activator.CreateInstance(typeof(T));

            foreach (var c in tblSet.Columns)
            {
                var property = entity.GetType().GetProperty(c.ColumnName, BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.CanWrite)
                {
                    switch (c.DataType)
                    {
                        case ColumnDataType.Int:
                            if (c as IDbParseableColumn<int> != null)
                            {
                                var column = c as IDbParseableColumn<int>;

                                property.SetValue(entity, column.ParseColumn(cols[column.ColumnPosition]), null);
                            }
                            else if (c as IDbPrimaryKeyColumn<int> != null)
                            {
                                var column = c as IDbParseableColumn<int>;

                                property.SetValue(entity, column.ParseColumn(cols[column.ColumnPosition]), null);
                            }
                            break;
                        case ColumnDataType.Double:
                            if (c as IDbParseableColumn<double> != null)
                            {
                                var column = c as IDbParseableColumn<double>;

                                property.SetValue(entity, column.ParseColumn(cols[column.ColumnPosition].Replace('.', ',')), null);
                            }
                            break;
                        case ColumnDataType.Decimal:
                            if (c as IDbParseableColumn<decimal> != null)
                            {
                                var column = c as IDbParseableColumn<decimal>;

                                property.SetValue(entity, column.ParseColumn(cols[column.ColumnPosition].Replace('.', ',')), null);
                            }
                            break;
                        case ColumnDataType.String:
                            if (c as IDbPrimaryKeyColumn<string> != null)
                            {
                                var column = c as IDbParseableColumn<string>;

                                property.SetValue(entity, column.ParseColumn(cols[column.ColumnPosition]), null);
                            }
                            else
                            {
                                property.SetValue(entity, cols[c.ColumnPosition], null);
                            }
                            break;
                        case ColumnDataType.DateTime:
                            if (c as IDbParseableColumn<DateTime> != null)
                            {
                                var column = c as IDbParseableColumn<DateTime>;

                                property.SetValue(entity, column.ParseColumn(cols[column.ColumnPosition]), null);
                            }
                            break;
                        case ColumnDataType.SingleRelationship:
                            if (c as IDbRelationshipColumn != null)
                            {
                                var column = c as IDbRelationshipColumn;

                                DbHelpers.SetSingleRelationshipPropertyOnEntity(column, cols, property, entity);
                            }
                            break;
                        case ColumnDataType.MultipleRelationships:
                            if (c as IDbRelationshipColumn != null)
                            {
                                var column = c as IDbRelationshipColumn;

                                DbHelpers.SetMultipleRelatinshipPropertyOnEntity(column, cols, property, entity);
                            }
                            break;
                        default:
                            property.SetValue(entity, cols[c.ColumnPosition], null);
                            break;
                    }
                }
            }
            return entity;
        }

        /// <summary>
        /// Converts a entity object to a DbText rowstring
        /// </summary>
        /// <typeparam name="T">Type of entity passed in</typeparam>
        /// <param name="entity">Entity object to convert</param>
        /// <param name="tblSet">The tableset we are working on</param>
        /// <returns>A rowstring to insert in TextDb</returns>
        internal static string ConvertEntityToTextDbLine<T>(T entity, IDbTableSet tblSet)
        {
            var line = "";

            foreach (var c in tblSet.Columns)
            {
                var value = "";
                var property = entity.GetType().GetProperty(c.ColumnName, BindingFlags.Public | BindingFlags.Instance);

                if (c as IDbRelationshipColumn != null)
                {
                    GenerateRelationsStringForDbFile(property, entity, ref value);
                }
                else
                {
                    value = property.GetValue(entity).ToString();
                }

                line += value + ";";
            }

            line = line.Substring(0, line.Length - 1);
            return line;
        }

        /// <summary>
        /// Create instance of a method with generic type
        /// </summary>
        /// <param name="methodHolder">Type of class/type that has the method </param>
        /// <param name="methodName">Name of the method</param>
        /// <param name="genericType">Generic type for method</param>
        /// <returns></returns>
        internal static MethodInfo CreateGenericMethodOfType(Type methodHolder, string methodName, Type genericType)
        {
            MethodInfo methodBase = methodHolder.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            MethodInfo genericMethod = methodBase.MakeGenericMethod(genericType);

            return genericMethod;
        }

        /// <summary>
        /// Invoke a created method and cast result to List<IEntity>
        /// </summary>
        /// <param name="method">Instance of the method you want to invoke</param>
        /// <param name="inValue">Parameter for method (TableName)</param>
        /// <returns></returns>
        internal static List<IEntity> InvokeMethodAndCastResultToListOfIEntity(MethodInfo method, object inValue)
        {
            var obj = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(method.DeclaringType.FullName);
            IList result = (IList)method.Invoke(obj, new object[] { inValue });
            List<IEntity> list = result.Cast<IEntity>().ToList();

            return list;
        }

        /// <summary>
        /// Generates a generic instance of a Type
        /// </summary>
        /// <param name="baseType">Generic basetype</param>
        /// <param name="genericType">Generic type</param>
        /// <returns></returns>
        internal static object CreateGenericInstanceOfType(Type baseType, Type genericType)
        {
            return Activator.CreateInstance(baseType.MakeGenericType(genericType));
        }

        private static void GenerateRelationsStringForDbFile<T>(PropertyInfo property, T entity, ref string value)
        {
            var obj = property.GetValue(entity);

            if (obj as IEnumerable != null)
            {
                obj = ((IEnumerable)obj).Cast<IEntity>();
                var idString = "";

                if (obj as IEnumerable<IPrimaryInt> != null)
                {
                    foreach (var o in (IEnumerable)obj)
                    {
                        idString += ((IPrimaryInt)o).Id.ToString() + "^";
                    }
                }

                if (obj as IEnumerable<IPrimaryString> != null)
                {
                    foreach (var o in (IEnumerable)obj)
                    {
                        idString += ((IPrimaryString)o).Id.ToString() + "^";
                    }
                }

                if (idString.Length > 0)
                {
                    value = idString.Substring(0, idString.Length - 1);
                }
            }
            else
            {
                var idString = "";

                if (obj as IPrimaryInt != null)
                {
                    idString += ((IPrimaryInt)obj).Id.ToString();
                }

                if (obj as IPrimaryString != null)
                {
                    idString += ((IPrimaryString)obj).Id;
                }

                value = idString;
            }
        }

        /// <summary>
        /// Set property on entity object with multiple relations
        /// </summary>
        /// <param name="column">The column we are working with</param>
        /// <param name="cols">Column values in string[]</param>
        /// <param name="property">The property we are trying to set</param>
        /// <param name="entity">The entity we are trying to set the property on</param>
        internal static void SetMultipleRelatinshipPropertyOnEntity(IDbRelationshipColumn column, string[] cols, PropertyInfo property, object entity)
        {
            string columnValue = cols[column.ColumnPosition];
            Type returnType = column.RelationshipReturnType;
            MethodInfo genericMethod = DbHelpers.CreateGenericMethodOfType(typeof(TextDbSchema), "GetAllRecordsFromTableAsEntities", returnType);
            IEnumerable<IEntity> relationshipList = DbHelpers.InvokeMethodAndCastResultToListOfIEntity(genericMethod, column.ToTable);
            string[] ids = columnValue.Split('^');
            var objList = DbHelpers.CreateGenericInstanceOfType(typeof(List<>), returnType);

            foreach (var id in ids)
            {
                if (id != "")
                {
                    var addMethod = objList.GetType().GetMethod("Add");

                    if (relationshipList.TryCast<IPrimaryInt>())
                    {
                        var tmpList = relationshipList.Cast<IPrimaryInt>().ToList();
                        var obj = tmpList.FirstOrDefault(o => o.Id == int.Parse(id));

                        addMethod.Invoke(objList, new object[] { obj });
                    }

                    if (relationshipList.TryCast<IPrimaryString>())
                    {
                        var tmpList = relationshipList.Cast<IPrimaryString>().ToList();
                        var obj = tmpList.FirstOrDefault(o => o.Id == id);

                        addMethod.Invoke(objList, new object[] { obj });
                    }
                }
            }

            property.SetValue(entity, objList, null);
        }

        /// <summary>
        /// Set property on entity object with multiple relations
        /// </summary>
        /// <param name="column">The column we are working with</param>
        /// <param name="cols">Column values in string[]</param>
        /// <param name="property">The property we are trying to set</param>
        /// <param name="entity">The entity we are trying to set the property on</param>
        internal static void SetSingleRelationshipPropertyOnEntity(IDbRelationshipColumn column, string[] cols, PropertyInfo property, object entity)
        {
            string columnValue = cols[column.ColumnPosition];
            Type returnType = column.RelationshipReturnType;
            MethodInfo genericMethod = DbHelpers.CreateGenericMethodOfType(typeof(TextDbSchema), "GetAllRecordsFromTableAsEntities", returnType);
            IEnumerable<IEntity> relationshipList = DbHelpers.InvokeMethodAndCastResultToListOfIEntity(genericMethod, column.ToTable);

            object obj = new object();

            if (relationshipList.TryCast<IPrimaryInt>() && (columnValue != null && columnValue != ""))
            {
                var tmpList = relationshipList.Cast<IPrimaryInt>().ToList();
                obj = tmpList.FirstOrDefault(o => o.Id == int.Parse(columnValue));

                property.SetValue(entity, obj, null);
            }

            if (relationshipList.TryCast<IPrimaryString>() && (columnValue != null && columnValue != ""))
            {
                var tmpList = relationshipList.Cast<IPrimaryInt>().ToList();
                obj = tmpList.FirstOrDefault(o => o.Id == int.Parse(columnValue));

                property.SetValue(entity, obj, null);
            }
        }

        /// <summary>
        /// Cleans up relationship ids in tables after a delete has happend
        /// </summary>
        /// <param name="entities">Entities in table</param>
        /// <param name="deletedId">Id for the deleted entity</param>
        /// <param name="columns">Columns in the table</param>
        internal static void CleanUpDeletedRelationsInEntities(List<string> entities, string deletedId, List<IDbColumn> columns, ref int deletedRelations)
        {
            var csvHeaderLine = entities[0];
            entities.RemoveAt(0);

            for (var i = 0; i < entities.Count; i++)
            {
                foreach (var c in columns)
                {
                    int colPos = c.ColumnPosition;
                    string[] cols = entities[i].Split(';');
                    List<string> relationsIds = cols[colPos].Split('^').ToList();

                    deletedRelations += relationsIds.RemoveAll(x => x == deletedId);

                    var newRelationsString = "";

                    if (relationsIds.Count > 0)
                    {
                        foreach (var id in relationsIds)
                        {
                            newRelationsString += id + "^";
                        }
                        newRelationsString = newRelationsString.Substring(0, newRelationsString.Length - 1);
                    }

                    cols[colPos] = newRelationsString;

                    entities[i] = string.Join(";", cols);
                }
            }
            entities.Insert(0, csvHeaderLine);
        }

        /// <summary>
        /// A Dictionary of Tablenames and their current PrimaryKey
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, int> DbPrimaryKeyDictionary()
        {
            Dictionary<string, int> pkDict = new Dictionary<string, int>();

            List<string> pkLines = File.ReadAllLines(TextDbSchema.DbInfoFile.FullFilePath()).ToList();
            pkLines = pkLines.Where(l => l.Contains("PrimaryKeyCount=")).ToList();

            foreach (var line in pkLines)
            {
                string tmp = line.Trim();
                string tableName = "";
                int pkCount = 0;

                int tblNameEndPos = tmp.IndexOf(']');
                tableName = tmp.Substring(1, tblNameEndPos - 1);

                int pkStartPos = tmp.IndexOf("PrimaryKeyCount=") + 16;
                int pkEndPos = tmp.IndexOf(']', tblNameEndPos + 1);
                string pkCurrent = tmp.Substring(pkStartPos, pkEndPos - pkStartPos);

                if (int.TryParse(pkCurrent, out pkCount))
                {
                    pkDict.Add(tableName, pkCount);
                }
            }

            return pkDict;
        }

        /// <summary>
        /// Method that writes the new DbInfo file at application startup
        /// </summary>
        /// <param name="tblSet">List of tables in TextDb (TextDbSchema.Tables should have all tables)</param>
        internal static void WriteNewDbInfoFile(List<IDbTableSet> tblSet)
        {
            var dbInfoFile = TextDbSchema.DbInfoFile.FullFilePath();
            var dbInfoJsonFile = TextDbSchema.DbInfoJsonFile.FullFilePath();

            if (!File.Exists(dbInfoFile))
            {
                File.WriteAllText(dbInfoFile, "");
            }

            var pkDict = DbPrimaryKeyDictionary();

            List<string> dbInfoLines = new List<string>
            {
                "<![DbInfo]>",
                $"    <Tables Count={ tblSet.Count }>"
            };

            foreach (var tbl in tblSet)
            {
                var pk = 0;

                if (pkDict.ContainsKey(tbl.TableName))
                {
                    pk = pkDict[tbl.TableName];
                }
                else
                {
                    // TODO - Look up the highest id of table if primary key is of type int
                }

                dbInfoLines.Add($"        [{ tbl.TableName }] [PrimaryKeyCount={ pk }] ({ tbl.EntityType })");
                dbInfoLines.Add($"            <Columns Count={ tbl.Columns.Count }>");

                foreach (var c in tbl.Columns)
                {
                    dbInfoLines.Add($"                [{ c.ColumnName }] ({ c.DataType })");
                }

                dbInfoLines.Add($"            </Columns>");
            }

            dbInfoLines.Add($"    </Tables>");
            dbInfoLines.Add($"</[DbInfo]>");

            var json = JsonConvert.SerializeObject(tblSet, 
                            Formatting.Indented,
                            new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Error
                            });

            File.WriteAllLines(dbInfoFile, dbInfoLines);
            File.WriteAllText(dbInfoJsonFile, json);
        }

        /// <summary>
        /// Creates text database files for all our tables
        /// </summary>
        /// <param name="tblSet">List of tables in TextDb (TextDbSchema.Tables should have all tables)</param>
        internal static void CreateTextDataFilesIfNotExists(List<IDbTableSet> tblSet)
        {
            foreach (var tbl in tblSet)
            {
                var textDbfile = tbl.DbTextFile.FullFilePath();

                if (!File.Exists(textDbfile))
                {
                    List<string> lines = new List<string>();
                    lines.Add(GenerateCsvHeader(tbl));

                    File.WriteAllLines(textDbfile, lines);
                }
                else
                {
                    List<string> entities = textDbfile.LoadFile();
                    var headerLine = entities[0];
                    var headerCols = headerLine.Split(';');

                    if (headerCols.Count() != tbl.Columns.Count)
                    {
                        throw new Exception("The column count in file: " + tbl.DbTextFile + " for table: " + tbl.TableName + " does not match");
                    }

                    bool colsValid = true;

                    foreach (var s in headerCols)
                    {
                        if (!(s.Substring(0, 1) == "[" && s.Substring(s.Length - 1, 1) == ")"))
                        {
                            colsValid = false;
                        }
                    }

                    if (!colsValid)
                    {
                        entities.Insert(0, GenerateCsvHeader(tbl));
                        File.WriteAllLines(textDbfile, entities);
                    }
                }
            }
        }

        /// <summary>
        /// Generates our Csv header line
        /// </summary>
        /// <param name="tblSet">The table we want to create csv header for</param>
        /// <returns>A Csv header string</returns>
        internal static string GenerateCsvHeader(IDbTableSet tblSet)
        {
            string headerString = "";

            foreach (var column in tblSet.Columns)
            {
                headerString += $"[{ column.ColumnName }] ({ column.DataType });";
            }

            headerString = headerString.Substring(0, headerString.Length - 1);

            return headerString;
        }

        /// <summary>
        /// Simple method to lookup appsetting in Config file
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string AppSettingLookUp(string key)
        {
            if (ConfigurationManager.AppSettings[key] != null)
            {
                return ConfigurationManager.AppSettings[key];
            }
            return null;
        }
    }
}
