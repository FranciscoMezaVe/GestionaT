using GestionaT.Domain.Entities;

namespace GestionaT.Application.Interfaces.Repositories
{
    public interface IBusinessRepository : IRepository<Business>
    {
        IList<Business> GetAllByUserId(Guid userId);
        IList<Guid> GetBusinessIdsAccessibleByUser(Guid userId);
        IList<Business> GetBusinessAccessibleByUser(Guid userId);
        IList<Business> GetAllIncludingDeletedByUser(Guid userId);
    }
}
