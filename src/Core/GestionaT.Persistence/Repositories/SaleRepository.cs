using System.Linq.Expressions;
using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Entities;
using GestionaT.Persistence.PGSQL;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.Repositories
{
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(AppPostgreSqlDbContext context) : base(context) { }

        public IQueryable<Sale> GetSalesForReport()
        {
            return _dbSet
                .Include(x => x.Customer)
                .Include(x => x.Business)
                .Include(x => x.SaleProducts)
                    .ThenInclude(sp => sp.Product)
                .Where(x => !x.IsDeleted); // si aplica soft delete
        }
    }
}
