using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TrackerLibrary.Interfaces;
using TrackerLibrary.Models;
using TrackerLibrary.TextDb.DbSchema;
using TrackerLibrary.TextDb.Enums;
using TrackerLibrary.TextDb.Interfaces;

namespace TrackerLibrary.TextDb.Extensions
{
    public static class TypeExtensions
    {
        public static string FullFilePath(this string fileName)
        {
            return TextDbSchema.DbTextFilesPath + "\\" + fileName;
        }
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<TModel> ConvertToModels<TModel>(this List<string> lines, IDbTableSet tblSet, out string tableInfoString) // where TModel : new()
        {
            List<TModel> output = new List<TModel>();
            tableInfoString = "";

            if (lines.Count > 0 && lines[0].Contains("<![TableInfo]>"))
            {
                tableInfoString = lines[0];
                lines.Remove(lines[0]);
            }
            else
            {
                tableInfoString = "ERROR!";
            }

            foreach (string l in lines)
            {
                string[] cols = l.Split('|');

                // TModel model = new TModel();
                TModel model = (TModel)Activator.CreateInstance(typeof(TModel));

                foreach (var c in tblSet.Columns)
                {
                    
                    var prop = model.GetType().GetProperty(c.ColumnName, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        switch (c.DataType)
                        {
                            case ColumnDataType.IntType:
                                if (c as IParseableColumn<int> != null)
                                {
                                    var column = c as IParseableColumn<int>;
                                    prop.SetValue(model, column.ParseColumn(cols[column.ColumnPosition]), null);
                                }
                                break;
                            case ColumnDataType.DoubleType:
                                if (c as IParseableColumn<double> != null)
                                {
                                    var column = c as IParseableColumn<double>;
                                    prop.SetValue(model, column.ParseColumn(cols[column.ColumnPosition].Replace('.', ',')), null);
                                }
                                break;
                            case ColumnDataType.DecimalType:
                                if (c as IParseableColumn<decimal> != null)
                                {
                                    var column = c as IParseableColumn<decimal>;
                                    prop.SetValue(model, column.ParseColumn(cols[column.ColumnPosition].Replace('.', ',')), null);
                                }
                                break;
                            case ColumnDataType.StringType:
                                prop.SetValue(model, cols[c.ColumnPosition], null);
                                break;
                            case ColumnDataType.SingleRelationship:
                                if (c as IDbRelationshipColumn != null)
                                {
                                    var column = c as IDbRelationshipColumn;
                                    var columnValue = cols[column.ColumnPosition];
                                    var columnType = column.RelationshipReturnType;
                                    var genericMethod = CreateGenericMethodOfType(typeof(TextDbSchema), "GetAllRecordsFromTableAsModels", columnType);
                                    var relationshipList = InvokeMethodAndCastResultToListOfIModel(genericMethod, column.ToTable);

                                    var obj = relationshipList.FirstOrDefault(o => o.Id == int.Parse(columnValue));
                                    prop.SetValue(model, obj, null);
                                }
                                break;
                            case ColumnDataType.MultipleRelationships:
                                if (c as IDbRelationshipColumn != null)
                                {
                                    var column = c as IDbRelationshipColumn;
                                    var columnValue = cols[column.ColumnPosition];
                                    var columnType = column.RelationshipReturnType;
                                    var genericMethod = CreateGenericMethodOfType(typeof(TextDbSchema), "GetAllRecordsFromTableAsModels", columnType);
                                    var relationshipList = InvokeMethodAndCastResultToListOfIModel(genericMethod, column.ToTable);
                                    string[] ids = columnValue.Split('^');
                                    var objList = CreateGenericInstanceOfType(typeof(List<>), columnType);

                                    foreach (var id in ids)
                                    {
                                        var obj = relationshipList.FirstOrDefault(o => o.Id == int.Parse(id));

                                        // Add method with reflection
                                        var addMethod = objList.GetType().GetMethod("Add");
                                        addMethod.Invoke(objList, new object[] { obj });
                                    }

                                    prop.SetValue(model, objList, null);
                                }
                                break;
                            default:
                                prop.SetValue(model, cols[c.ColumnPosition], null);
                                break;
                        }
                    }
                }
                output.Add(model);
            }
            return output;
        }

        public static void SaveModelsToFile<T>(this List<T> models, string fileName, IDbTableSet tblSet, string tableInfoString)
        {
            List<string> lines = new List<string>();

            lines.Add(tableInfoString);

            foreach (var item in models)
            {
                var line = "";

                foreach (var c in tblSet.Columns)
                {
                    var value = "";

                    if (c as IDbRelationshipColumn != null)
                    {
                        var column = c as IDbRelationshipColumn;
                        var property = item.GetType().GetProperty(c.ColumnName, BindingFlags.Public | BindingFlags.Instance);
                        var obj = property.GetValue(item);

                        if (obj as IEnumerable != null)
                        {
                            var idString = "";

                            foreach (var o in (IEnumerable)obj)
                            {
                                idString += ((IModel)o).Id.ToString() + "^";
                            }

                            value = idString.Substring(0, idString.Length - 1);
                        }
                        else
                        {
                            value = ((IModel)obj).Id.ToString();
                        }
                    }
                    else
                    {
                        var property = item.GetType().GetProperty(c.ColumnName, BindingFlags.Public | BindingFlags.Instance);
                        value = property.GetValue(item).ToString();
                    }

                    line += value + "|";
                }

                line = line.Substring(0, line.Length - 1);
                lines.Add(line);
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static MethodInfo CreateGenericMethodOfType(Type methodHolder, string methodName, Type genericType)
        {
            MethodInfo methodBase = methodHolder.GetMethod(methodName);
            MethodInfo genericMethod = methodBase.MakeGenericMethod(genericType);

            return genericMethod;
        }

        private static List<IModel> InvokeMethodAndCastResultToListOfIModel(MethodInfo method, object inValue)
        {
            IList result = (IList)method.Invoke(null, new object[] { inValue });
            List<IModel> list = result.Cast<IModel>().ToList();

            return list;
        }

        private static object CreateGenericInstanceOfType(Type baseType, Type genericType)
        {
            return Activator.CreateInstance(baseType.MakeGenericType(genericType));
        }

        public static int GetNewId<T>(this List<T> models) where T : IModel
        {
            int currentId = 1;

            if (models.Count > 0)
            {
                currentId = models.OrderByDescending(p => p.Id).First().Id + 1;
            }

            return currentId;
        }

        public static int GetNewId(this string tblInfo, out string newTableInfoString)
        {
            var startPos = tblInfo.IndexOf("PKCount=") + 8;
            var endPos = tblInfo.IndexOf(']', startPos);
            var currPK = tblInfo.Substring(startPos, endPos - startPos);
            var newPKId = int.Parse(currPK) + 1;

            newTableInfoString = tblInfo.Replace("[PKCount=" + currPK + "]", "[PKCount=" + newPKId + "]");

            return newPKId;
        }
    }
}
