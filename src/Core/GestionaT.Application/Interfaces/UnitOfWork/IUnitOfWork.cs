using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Abstractions;

namespace GestionaT.Application.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
