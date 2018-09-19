namespace TrackerLibrary.TextDb.Interfaces
{
    interface IParseableColumn<T> : IDbColumn, IParseable<T>
    {
    }
}
