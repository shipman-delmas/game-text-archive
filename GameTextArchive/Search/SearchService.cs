using GameTextArchive.Data;
using Microsoft.EntityFrameworkCore;

namespace GameTextArchive.Search;

public class SearchService(GameTextDbContext dbContext)
{
    private readonly GameTextDbContext context = dbContext;
    
    public async Task<List<TextRecord>> SearchAsync(string query)
    {
        return await context.TextRecords
            .Where(p => p.SearchVector.Matches(query))
            .ToListAsync();
    }
}