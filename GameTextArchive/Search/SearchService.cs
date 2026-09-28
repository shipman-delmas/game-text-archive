using GameTextArchive.Data;
using GameTextArchive.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace GameTextArchive.Search;

public class SearchService(GameTextDbContext dbContext, IMemoryCache memoryCache, ILogger<SearchService> logger)
{
    private readonly GameTextDbContext context = dbContext;
    private readonly IMemoryCache cache = memoryCache;
    private readonly ILogger<SearchService> logger = logger;
    
    // Search database for matching records and return as a search result object that references
    // the record in database and a rank based on matching lexeme frequency.
    public async Task<PagedResult<SearchResult>> SearchAsync(string query, int page, int pageSize)
    {
        // mainly just whitespace. prevent redundant cache entries.
        string normalizedQuery = string.Join(' ', query.Trim().Split(
                        (char[]?)null, StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant();
        
        string cacheKey = $"search:{normalizedQuery}:{page}:{pageSize}";
        
        // check cache for matching result first.
        if (cache.TryGetValue(cacheKey, out PagedResult<SearchResult>? cachedResults))
        {
            logger.LogInformation(
                "Search cache hit for {Query}, page {Page}, page size {PageSize}",
                normalizedQuery,
                page,
                pageSize);
            
            return cachedResults!; 
        }
        
        logger.LogInformation(
            "Search cache miss for {Query}, page {Page}, page size {PageSize}",
            normalizedQuery,
            page,
            pageSize);
        
        // filters record table for matching text in ts_vector columns.
        // ranks matching records based on frequency of matching lexemes in descending order.
        var searchQuery = context.TextRecords
            .AsNoTracking()
            .Where(p => p.SearchVector != null &&
                        p.SearchVector.Matches(
                            EF.Functions.WebSearchToTsQuery(normalizedQuery)))
            .Select(p => new SearchResult
            {
                Record = p,
                Rank = p.SearchVector!.RankCoverDensity(EF.Functions.WebSearchToTsQuery(normalizedQuery))
            })
            .OrderByDescending(x => x.Rank);

        // define int num results for data model.
        int totalCount = await searchQuery.CountAsync();

        // store list of only current ten results for page for readonly list in data model.
        var results = await searchQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // return collection model of search results. 
        var pagedResults = new PagedResult<SearchResult>()
        {
            Items = results,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
        
        // insert result to cache.
        cache.Set(cacheKey, pagedResults, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            });

        return pagedResults;
    }
}