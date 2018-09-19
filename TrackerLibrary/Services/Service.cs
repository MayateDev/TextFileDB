using AutoMapper;
using DataLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDbLibrary.Interfaces;

namespace TrackerLibrary.Services
{
    public class Service<TRepository, TEntity, TModel>
        where TRepository : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly TRepository _repo;

        public Service(TRepository repo)
        {
            _repo = repo;
        }

        public virtual TModel Add(TModel model)
        {
            //_repo.Add(Mapper.Map<TEntity>(model));
            return Mapper.Map<TModel>(_repo.Add(Mapper.Map<TEntity>(model)));
        }

        public virtual TModel Read(int id)
        {
            return Mapper.Map<TModel>(_repo.Read(id));
        }

        public virtual TModel Update(TModel model)
        {
            //_repo.Update(Mapper.Map<TEntity>(model));
            return Mapper.Map<TModel>(_repo.Update(Mapper.Map<TEntity>(model)));
        }

        public virtual void Delete(TModel model)
        {
            _repo.Delete(Mapper.Map<TEntity>(model));
        }

        public virtual IEnumerable<TModel> List()
        {
            return Mapper.Map<IEnumerable<TModel>>(_repo.List());
        }
    }
}
