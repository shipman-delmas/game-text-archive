import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";

import Results from "../components/Results";
import type { SearchResult } from "../types/SearchResult";
import { search } from "../services/searchService";

function SearchResultsPage() {
    // array destructing to get only query string held in search parameter.
    const [searchParams] = useSearchParams();
    const query = searchParams.get("query") ?? "";

    // react state.
    const [results, setResults] = useState<SearchResult[]>([]);
    const [error, setError] = useState("");

    // effect is result of calling api.
    useEffect(() => {
        // receive query and return if empty or null.
        async function performSearch() {
            if (query.trim() === "") {
                return;
            }

            // clear error and call api.
            try {
                setError("");

                const searchResults = await search(query);

                setResults(searchResults);
            } catch (error) {
                console.error(error);
                setError("Search failed.");
            }
        }

        performSearch();
    }, [query]);

    return (
        <main className="search-background min-h-screen p-8">
            <div className="mx-auto max-w-4xl">
                <h1 className="text-3xl font-bold">
                    Search Results
                </h1>

                <p className="mt-2 text-gray-600">
                    Results for: <strong>{query}</strong>
                </p>

                {error && (
                    <p className="mt-4 text-red-600">
                        {error}
                    </p>
                )}

                <div className="mt-8">
                    <Results results={results} />
                </div>
            </div>
        </main>
    );
}

export default SearchResultsPage;