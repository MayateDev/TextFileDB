namespace TextDbLibrary.Interfaces
{
    public interface IDbParseable<T>
    {
        T ParseColumn(string value);
    }
}
