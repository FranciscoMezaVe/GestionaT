using System.Linq.Expressions;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Abstractions;
using GestionaT.Persistence.PGSQL;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        private readonly AppPostgreSqlDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(AppPostgreSqlDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            // Buscar respetando soft delete si aplica
            var entity = await _dbSet.FindAsync(id);
            if (entity is ISoftDeletable softDeletable && softDeletable.IsDeleted)
                return null;

            return entity;
        }

        public async Task<List<TEntity>> GetAllAsync()
        {
            return await Query().ToListAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsDeleted = true;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public IQueryable<TEntity> Query()
        {
            // Filtra eliminados si aplica
            var query = _dbSet.AsQueryable();
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(e => !EF.Property<bool>(e, nameof(ISoftDeletable.IsDeleted)));
            }

            return query;
        }

        public IQueryable<TEntity> QueryIncludingDeleted()
        {
            return _dbSet.IgnoreQueryFilters();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await Query().AnyAsync(predicate, cancellationToken);
        }

        public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Query();

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}