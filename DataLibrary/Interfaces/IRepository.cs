using System.Collections.Generic;

namespace DataLibrary.Interfaces
{
    public interface IRepository<TEntity>
    {
        TEntity Add(TEntity entity);
        TEntity Read(int id);
        TEntity Update(TEntity entity);
        void Delete(TEntity entity);
        IEnumerable<TEntity> List();
    }
}