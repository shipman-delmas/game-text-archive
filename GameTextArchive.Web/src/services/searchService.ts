// communicate with asp.net.

// using data table 'search result.'
import type { SearchResult } from "../types/SearchResult.ts";

// our base url.
const API_BASE_URL = "http://localhost:5080";

// public async method takes string query and return array of search result objects.
export async function search(query: string): Promise<SearchResult[]> 
{
    // variable stores asp.net http request.
    // fetch is the http request and passes out url + encoded query.
    const response = await fetch(`${API_BASE_URL}/api/search?query=${encodeURIComponent(query)}`);
    
    // asp.net returns json in response to http request.
    // react converts to array of search result objects.
    return await response.json();
}