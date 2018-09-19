namespace TextDbLibrary.Interfaces
{
    public interface IDbParseableColumn<T> : IDbColumn, IDbParseable<T>
    {
    }
}
