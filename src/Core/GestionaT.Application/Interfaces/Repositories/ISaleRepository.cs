using GestionaT.Domain.Entities;

namespace GestionaT.Application.Interfaces.Repositories
{
    public interface ISaleRepository : IRepository<Sale>
    {
        IQueryable<Sale> GetSalesForReport();
    }
}
