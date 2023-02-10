using Twitter.Stats.Application.Common.Models;

namespace Twitter.Stats.Application.Extensions
{
    public static class MappingExtensions
    {
        public static PaginatedList<TDestination> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageNumber, int pageSize) where TDestination : class
        => PaginatedList<TDestination>.Create(queryable, pageNumber, pageSize);
    }
}
