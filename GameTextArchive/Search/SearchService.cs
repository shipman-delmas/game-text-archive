using GameTextArchive.Data;
using GameTextArchive.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace GameTextArchive.Search;

public class SearchService(GameTextDbContext dbContext)
{
    private readonly GameTextDbContext context = dbContext;
    
    // Search database for matching records and return as a search result object that references
    // the record in database and a rank based on matching lexeme frequency.
    public async Task<PagedResult<SearchResult>> SearchAsync(string query, int page, int pageSize)
    {
        // filters record table for matching text in ts_vector columns.
        // ranks matching records based on frequency of matching lexemes in descending order.
        
        // store all results.
        var searchQuery = context.TextRecords
            .Where(p => p.SearchVector != null &&
                        p.SearchVector.Matches(
                            EF.Functions.WebSearchToTsQuery(query)))
            .Select(p => new SearchResult
            {
                Record = p,
                Rank = p.SearchVector!.RankCoverDensity(EF.Functions.WebSearchToTsQuery(query))
            })
            .OrderByDescending(x => x.Rank);

        // define int num results for data model.
        int totalCount = await searchQuery.CountAsync();

        // store list of only current ten results for page for readonly list in data model.
        var results = await searchQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        // return collection model of search results. 
        return new PagedResult<SearchResult>()
        {
            Items = results,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}