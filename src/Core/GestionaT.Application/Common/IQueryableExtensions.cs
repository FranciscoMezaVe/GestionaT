using AutoMapper.QueryableExtensions;
using AutoMapper;
using GestionaT.Application.Common.Pagination;

namespace GestionaT.Application.Common
{
    public static class IQueryableExtensions
    {
        public static PaginatedList<TDto> ToPagedList<TEntity, TDto>(
            this IQueryable<TEntity> query,
            IMapper mapper,
            int pageIndex,
            int pageSize)
            where TEntity : class
        {
            var totalCount = query.Count();

            var items = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<TDto>(mapper.ConfigurationProvider)
                .ToList();

            return new PaginatedList<TDto>(items, totalCount, pageIndex, pageSize);
        }

        public static PaginatedList<TDto> ToPagedList<TEntity, TDto>(
            this IList<TEntity> source,
            IMapper mapper,
            int pageIndex,
            int pageSize)
            where TEntity : class
        {
            var totalCount = source.Count;

            var items = source
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedItems = mapper.Map<List<TDto>>(items);

            return new PaginatedList<TDto>(mappedItems, totalCount, pageIndex, pageSize);
        }
    }
}
