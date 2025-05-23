using GestionaT.Application.Interfaces.Repositories;
using GestionaT.Domain.Entities;
using GestionaT.Persistence.PGSQL;
using Microsoft.EntityFrameworkCore;

namespace GestionaT.Persistence.Repositories
{
    public class BusinessRepository : Repository<Business>, IBusinessRepository
    {
        private readonly AppPostgreSqlDbContext _context;

        public BusinessRepository(AppPostgreSqlDbContext context) : base(context)
            => _context = context;

        public IList<Business> GetAllByUserId(Guid userId)
            => Query() // Usa repositorio base que ya filtra eliminados
                .Where(b => b.OwnerId == userId)
                .ToList();

        public IList<Business> GetBusinessAccessibleByUser(Guid userId)
            => Query()
                .Include(b => b.Members)
                .Where(b => b.Members.Any(m => m.UserId == userId) || b.OwnerId == userId)
                .ToList();

        public IList<Guid> GetBusinessIdsAccessibleByUser(Guid userId)
            => Query()
                .Include(b => b.Members)
                .Where(b => b.Members.Any(m => m.UserId == userId) || b.OwnerId == userId)
                .Select(b => b.Id)
                .ToList();
        public IList<Business> GetAllIncludingDeletedByUser(Guid userId)
    => _context.Businesses
        .IgnoreQueryFilters()
        .Where(b => b.OwnerId == userId)
        .ToList();
    }
}
