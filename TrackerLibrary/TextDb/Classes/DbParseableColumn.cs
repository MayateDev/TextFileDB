using System.ComponentModel;
using TrackerLibrary.TextDb.Interfaces;

namespace TrackerLibrary.TextDb.Classes
{
    public class DbParseableColumn<T> : DbColumn, IParseableColumn<T>
    {
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
