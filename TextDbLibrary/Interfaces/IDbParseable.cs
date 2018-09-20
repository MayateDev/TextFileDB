namespace TextDbLibrary.Interfaces
{
    public interface IDbParseable<T>
    {
        T ParseColumn(string value);
        //T ParseColumn(T value); // Test
    }
}
