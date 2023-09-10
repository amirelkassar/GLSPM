using GLSPM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Gridify;
namespace GLSPM.Application.EFCore.Repositories
{
    public class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        private readonly GLSPMDBContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;
        public BaseRepository(GLSPMDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }
        public async Task DeleteAsync(TEntity entity)
        {
             _dbSet.Remove(entity);
        }

        public async Task DeleteAsync(TKey key)
        {
            var entity = await _dbSet.IgnoreQueryFilters().Where($"e=>e.ID=={key}").FirstOrDefaultAsync();
            await DeleteAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null, string sorting = null, int skipCound = 0, int maxResult = 100)
        {
            var data = condition != null ? 
                await _dbSet.Where(condition) // applying conditions
                .OrderBy(sorting)
                .Skip(skipCound)
                .Take(maxResult)
                .ToArrayAsync() :
                await _dbSet //without conditions
                .OrderBy(sorting)
                .Skip(skipCound)
                .Take(maxResult)
                .ToArrayAsync();

            return data;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(string filter, string sorting = null, int skipCound = 0, int maxResult = 100)
        {
            var data = _dbSet.Gridify(new GridifyQuery
            {
                Filter = filter,
                OrderBy = sorting,
                PageSize = maxResult,
                Page = skipCound / maxResult
            });
                
            return data.Data;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(string sorting = null, int skipCound = 0, int maxResult = 100)
        {
            var data = await _dbSet
                .OrderBy(sorting)
                .Skip(skipCound)
                .Take(maxResult)
                .ToArrayAsync();
            return data;

        }

        public async Task<IQueryable<TEntity>> GetAsQueryableAsync()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<TEntity> GetAsync(TKey key)
        {
            return await _dbSet.FindAsync(keyValues: key);
        }

        public async Task<int> GetCountAsync(string? filter=null)
        {
            return filter != null ? _dbSet.Gridify(new GridifyQuery(0,int.MaxValue,filter)).Count : await _dbSet.CountAsync();
        }

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>>? condition = null)
        {
            return condition != null ?await _dbSet.Where(condition).CountAsync() : await _dbSet.CountAsync();
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entry= await _dbSet.AddAsync(entity);
            return entry.Entity;
        }
        public virtual async Task UpdateAsync(TEntity entity)
        {
           _dbSet.Update(entity);
        }
    }
}
