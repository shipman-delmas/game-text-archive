import type { SearchResult } from "../types/SearchResult";

interface SearchResultProps {
    results: SearchResult[];
}

// Maps search results to UI elements.
export default function SearchResults({ results}: SearchResultProps) 
{
    return (
        <div>
            {results.map((result) => (
                <div key={result.record.id}>
                    <h3>
                        {result.record.type}
                    </h3>

                    <p>
                        {result.record.text}
                    </p>

                    <small>
                        {result.record.editorId}
                    </small>
                </div>
            ))}
        </div>
    );
}