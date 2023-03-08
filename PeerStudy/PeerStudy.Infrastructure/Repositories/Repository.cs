using Microsoft.EntityFrameworkCore;
using PeerStudy.Core.Interfaces.Repositories;
using PeerStudy.Infrastructure.AppDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PeerStudy.Infrastructure.Repositories
{

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        protected readonly ApplicationDbContext Context;

        public Repository(ApplicationDbContext context)
        {
            Context = context;
        }

        public Task<T> AddAsync(T entity)
        {
            Context.Set<T>().Add(entity);
            return Task.FromResult(entity);
        }

        public Task<List<T>> AddRangeAsync(List<T> entities)
        {
            Context.Set<T>().AddRange(entities);
            return Task.FromResult(entities);
        }

        public async Task<T> GetAsync(Guid id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public async Task<T> RemoveAsync(Guid id)
        {
            var entityToBeDeleted = await Context.Set<T>().FindAsync(id);
            if (entityToBeDeleted == null)
            {
                return entityToBeDeleted;
            }
            Context.Set<T>().Remove(entityToBeDeleted);

            return entityToBeDeleted;
        }

        public Task<T> RemoveAsync(T entity)
        {
            Context.Set<T>().Remove(entity);

            return Task.FromResult(entity);
        }

        public Task<T> UpdateAsync(T entity)
        {
            Context.Set<T>().Update(entity);

            return Task.FromResult(entity);
        }

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            var entities = Context.Set<T>().Where(filter);

            return Task.FromResult(entities.Any());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Context.Set<T>().Find(id));
        }

        public Task<IQueryable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            int? skip = null,
            int? take = null,
            bool trackChanges = true)
        {
            IQueryable<T> entities = trackChanges ? Context.Set<T>() : Context.Set<T>().AsNoTracking();

            if (filter != null)
            {
                entities = entities.Where(filter);
            }

            if (skip != null)
            {
                entities = entities.Skip(skip.Value);
            }

            if (take != null)
            {
                entities = entities.Take(take.Value);
            }

            entities = GetEntitiesWithIncludedProperties(entities, includeProperties);

            return Task.FromResult(entities);
        }

        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> entities = Context.Set<T>();

            if (filter != null)
            {
                entities = entities.Where(filter);
            }

            entities = GetEntitiesWithIncludedProperties(entities, includeProperties);

            return await entities.FirstOrDefaultAsync();
        }

        private static IQueryable<T> GetEntitiesWithIncludedProperties(IQueryable<T> entities, string includeProperties)
        {
            if (includeProperties != null)
            {
                var propertiesToBeIncluded = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var property in propertiesToBeIncluded)
                {
                    entities = entities.Include(property);
                }
            }

            return entities;
        }

        public Task<int> GetTotalRecordsAsync(Expression<Func<T, bool>>? filter = null)
        {
            if (filter != null)
            {
                return Task.FromResult(Context.Set<T>().Where(filter).Count());
            }

            return Task.FromResult(Context.Set<T>().Count());
        }
    }
}
