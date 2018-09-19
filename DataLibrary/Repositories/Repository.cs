﻿using DataLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextDbLibrary.Classes;
using TextDbLibrary.Interfaces;

namespace DataLibrary.Repositories
{
    public class Repository<TContext, TEntity> : IDisposable
        where TContext : DbContext, new()
        where TEntity : class, IEntity
    {
        private TContext _context;
        private IDbTableSet _tblSet;

        public Repository()
        {
            _context = new TContext();
            _tblSet = TextDbHelpers.GetTableSetFromEntity(typeof(TEntity));
        }

        public virtual TContext Context
        {
            get
            {
                _context = new TContext();

                return _context;
            }
        }

        public virtual TEntity Add(TEntity entity)
        {
            _tblSet.Add(entity);

            return entity;
        }

        public virtual TEntity Read(int id)
        {
            return _tblSet.Read<TEntity>(id);
        }

        public virtual TEntity Update(TEntity entity)
        {
            _tblSet.Update(entity);

            return entity;
        }

        public virtual void Delete(TEntity entity)
        {
            _tblSet.Delete(entity);
        }

        public virtual IEnumerable<TEntity> List()
        {
            return _tblSet.List<TEntity>();
        }

        public void Dispose()
        {
            if (Context != null)
            {
                Context.Dispose();
            }
        }
    }
}