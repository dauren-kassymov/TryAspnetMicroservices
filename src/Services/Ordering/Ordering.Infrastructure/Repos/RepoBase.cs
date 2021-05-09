using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repos
{
    public class RepoBase<T> : IAsyncRepo<T> where T : EntityBase
    {
        protected readonly OrderContext Context;

        protected RepoBase(OrderContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await Context.Set<T>()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string include = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = Context.Set<T>();

            if (disableTracking) query = query.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(include)) query = query.Include(include);
            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true)
        {
            IQueryable<T> query = Context.Set<T>();

            if (disableTracking) query = query.AsNoTracking();
            if (includes != null)
                query = includes.Aggregate(query, (cur, include) => cur.Include(include));

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
                return await orderBy(query).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            Context.Set<T>().Remove(entity);
            await Context.SaveChangesAsync();
        }
    }
}