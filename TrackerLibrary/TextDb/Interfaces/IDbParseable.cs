namespace TrackerLibrary.TextDb.Interfaces
{
    public interface IParseable<T>
    {
        T ParseColumn(string value);
    }
}
