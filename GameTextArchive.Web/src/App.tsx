import { useState } from "react";

import SearchResults from "./components/Results";
import type { SearchResult } from "./types/SearchResult";
import { search } from "./services/searchService";

function App() {
    // create react state of type array search result 
    const [results, setResults] = useState<SearchResult[]>([]);
    const [error, setError] = useState("");
    const [query, setQuery] = useState("");
    
    // method coordinates components for search.
    async function handleSearch(query: string) 
    {
        try 
        {
            setError("");

            const searchResults = await search(query);

            setResults(searchResults);
        } catch (error) {
            console.error(error);
            setError("Search failed.");
        }
    }

    return (
        <main className="min-h-screen p-8">
            <div className="mx-auto max-w-4xl">
                <h1 className="text-3xl font-bold">
                    TESIII: Morrowind Text Archive
                </h1>

                <p className="mt-2 text-gray-600">
                    Search all Morrowind text data.
                </p>

                <div className="mt-8 flex gap-2">
                    <input
                        type="text"
                        placeholder="Search..."
                        value={query}
                        onChange={(event) => setQuery(event.target.value)}
                        className="flex-1 rounded-md border bg-white px-4 py-2"
                    />

                    <button
                        onClick={() => handleSearch(query)}
                        className="rounded-md bg-black px-5 py-2 text-white"
                    >
                        Search
                    </button>
                </div>

                {error && (
                    <p className="mt-4 text-red-600">
                        {error}
                    </p>
                )}

                <SearchResults results={results} />
            </div>

            <p className="fixed bottom-2 right-3 text-xs text-black/70">
                Background: Michael Kirkbride / Bethesda Softworks — via UESP.
            </p>
        </main>
    );
}


export default App;