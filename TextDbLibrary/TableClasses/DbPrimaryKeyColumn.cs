using System.ComponentModel;
using TextDbLibrary.Enums;
using TextDbLibrary.Interfaces;

namespace TextDbLibrary.TableClasses
{
    public class DbPrimaryKeyColumn<T> : DbColumn, IDbPrimaryKeyColumn<T>, IDbPrimaryKey
    {
        public DbPrimaryKeyColumn(string columnName, int columnPosition, ColumnDataType dataType)
            : base(columnName, columnPosition, dataType)
        {

        }

        public T ParseColumn(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                return (T)converter.ConvertFromString(value);
            }
            return default(T);
        }
    }
}
