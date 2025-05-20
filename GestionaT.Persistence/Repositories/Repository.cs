using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Abstractions;
using GestionaT.Persistence.PGSQL;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.Repositories
{
    public class Repository<TEntity> 
        : IRepository<TEntity> where TEntity 
        : class, IEntity
    {
        private readonly AppPostgreSqlDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public Repository(AppPostgreSqlDbContext context)
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
