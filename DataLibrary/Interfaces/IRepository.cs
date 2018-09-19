using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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