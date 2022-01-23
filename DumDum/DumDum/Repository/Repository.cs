using DumDum.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DumDum.Interfaces.IRepositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DumDum.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext DbContext;
        public Repository(ApplicationDbContext context)
        {
            DbContext = context;
        }

        public async Task Add(T entity)
        {
           await DbContext.Set<T>().AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<T> entities)
        {
            await DbContext.Set<T>().AddRangeAsync(entities);
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await DbContext.Set<T>().Where(expression).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await DbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
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

        public async Task<bool> Any(Expression<Func<T, bool>> expression)
        {
            return await DbContext.Set<T>().AnyAsync(expression);
        }

        public void Update(T entity)
        {
            DbContext.Update(entity);
        }
    }
}
