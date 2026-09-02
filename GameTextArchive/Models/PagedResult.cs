namespace GameTextArchive.Models;

// custom defined collection for paged objects.
// initially using for search result objects in search service,
// but we can use this for any object type when needed. 
public class PagedResult<T>
{
    // readonly collection of only the objects required to be rendered on the page. 
    // init prevents setting value after creation.
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
}