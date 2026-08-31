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
    public async Task<List<SearchResult>> SearchAsync(string query)
    {
        // convert user query to ts_query.
        var tsQuery = EF.Functions.WebSearchToTsQuery(query);
        
        // filters record table for matching text in ts_vector columns.
        // ranks matching records based on frequency of matching lexemes in descending order.
        return await context.TextRecords
            .Where(p => p.SearchVector.Matches(tsQuery))
            .Select(p => new SearchResult
            {
                Record = p,
                Rank = p.SearchVector.Rank(tsQuery)
            })
            .OrderByDescending(x => x.Rank)
            .Take(100)
            .ToListAsync();
    }
}