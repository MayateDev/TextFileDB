using System.ComponentModel;
using TextDbLibrary.Enums;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.TableClasses
{
    public class DbParseableColumn<T> : DbColumn, IDbParseableColumn<T>
    {
        public DbParseableColumn(string columnName, int columnPosition, ColumnDataType dataType)
            : base(columnName, columnPosition, dataType)
        {

        }

        // TODO - This part probably needs an overhaul
        //public T ParseColumn(T value) // Test code
        public T ParseColumn(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFromString(value);
            }
            return default(T);

            // Test code
            //if (typeof(T) != typeof(string))
            //{
            //    var converter = TypeDescriptor.GetConverter(typeof(T));
            //    if (converter != null)
            //    {
            //        return (T)converter.ConvertFromString(value.ToString());
            //    }
            //    return default(T);
            //}
            //return value;
            // End
        }
    }
}
