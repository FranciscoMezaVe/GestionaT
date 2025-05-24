using System.Linq.Expressions;
using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Interfaces.Repositories
{
    public interface IRepository<TEntity> where TEntity : IEntity
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<List<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        void Update(TEntity entity);

        /// <summary>Realiza eliminación lógica si la entidad implementa ISoftDeletable.</summary>
        void Remove(TEntity entity);

        IQueryable<TEntity> Query();

        /// <summary>Incluye eliminados lógicamente (IsDeleted == true).</summary>
        IQueryable<TEntity> QueryIncludingDeleted();

        IQueryable<TEntity> QueryIncluding(params Expression<Func<TEntity, object>>[] includes);
    }
}
