using DumDum.Database;
using DumDum.Interfaces;
using DumDum.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DumDum.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext DbContext;
        public Repository(ApplicationDbContext context)
        {
            DbContext = context;
        }

        async public Task Add(T entity)
        {
           await DbContext.Set<T>().AddAsync(entity);
        }

        async public Task AddRange(IEnumerable<T> entities)
        {
            await DbContext.Set<T>().AddRangeAsync(entities);
        }

        async public Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await DbContext.Set<T>().Where(expression).ToListAsync();
        }

        async public Task<IEnumerable<T>> GetAll()
        {
            return await DbContext.Set<T>().ToListAsync();
        }

        async public Task<T> GetById(int id)
        {
            return await DbContext.Set<T>().FindAsync(id);
        }

        public void Remove(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
        }

        async public Task<bool> Any(Expression<Func<T, bool>> expression)
        {
            return await DbContext.Set<T>().AnyAsync(expression);
        }

        public void Update(T entity)
        {
            DbContext.Update(entity);
        }
    }
}
