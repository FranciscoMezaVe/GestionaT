using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.Repositories
{
    public class Repository<TEntity, TContext> 
        : IRepository<TEntity> where TEntity 
        : class, IEntity where TContext : DbContext
    {
        private readonly TContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public Repository(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }
        public async Task<TEntity?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);
        public async Task<List<TEntity>> GetAllAsync()
            => await _dbSet.ToListAsync();
        public async Task AddAsync(TEntity entity)
            => await _dbSet.AddAsync(entity);
        public void Update(TEntity entity)
            => _dbSet.Update(entity);
        public void Remove(TEntity entity)
            => _dbSet.Remove(entity);
        public IQueryable<TEntity> Query()
            => _dbSet.AsQueryable();
    }
}
