using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<List<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity  entity);
        IQueryable<TEntity> Query();
    }
}
