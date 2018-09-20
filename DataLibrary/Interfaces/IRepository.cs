using System.Collections.Generic;

namespace DataLibrary.Interfaces
{
    public interface IRepository<TEntity>
    {
        TEntity Add(TEntity entity);
        TEntity Read<T>(T id);
        TEntity Update(TEntity entity);
        void Delete(TEntity entity);
        IEnumerable<TEntity> List();
        IEnumerable<TEntity> AddEntities(List<TEntity> entities);
    }
}