import { useState } from "react";
import type { SearchResult } from "../types/SearchResult";
import type { ReactNode } from "react";
import { Link } from "react-router-dom";

interface SearchResultProps 
{
    results: SearchResult[];
    query: string;
}

// FIX: MOVE PAGINATION TO SEARCH API.
const RESULTS_PER_PAGE = 10;

// if text not null, find and replace formatting tags.
function cleanText(text: string | null): string 
{
    if (text === null) 
    {
        return "None";
    }
    
    return text
        .replace(/<BR\s*\/?>/gi, "\n")
        .replace(/<[^>]*>/g, "")
        .trim();
}

// shorten result entries to excerpt with search term.
// passes radius data type for number on either side of term.
function createExcerpt(text: string | null, query: string, radius = 100): ReactNode 
{
    // cleans text via method.
    const cleanedText = cleanText(text);

    // checks if valid text left after cleaning.
    if (!cleanedText) 
    {
        return null;
    }

    // trim query.
    const trimmedQuery = query.trim();

    // text and query to lowercase for ease of value matching.
    const lowerText = cleanedText.toLowerCase();
    const lowerQuery = trimmedQuery.toLowerCase();

    // find index of value equal to query in text.
    const matchIndex = lowerText.indexOf(lowerQuery);
    
    // excerpt start is either index - radius or 0.
    // index - predetermined radius could be negative, so ensures start index is not negative/non-existent.
    const start = Math.max(0, matchIndex - radius);
    // excerpt end is either text length or index + query length + radius.
    // prevents end index from being greater than actual text length.
    const end = Math.min(cleanedText.length, matchIndex + trimmedQuery.length + radius);

    // assign substring of text.
    const excerpt = cleanedText.slice(start, end);

    // find new index of query term inside substring.
    // the start value is unchanged but should technically be zero, so subtract it from query
    // index to adjust for that.
    const relativeMatchIndex = matchIndex - start;

    // get another substring of excerpt start to before the query index.
    const beforeMatch = excerpt.slice(0, relativeMatchIndex);

    // substring of query term.
    const match = excerpt.slice(relativeMatchIndex, relativeMatchIndex + trimmedQuery.length);

    // substring of excerpt after query term.
    const afterMatch = excerpt.slice(relativeMatchIndex + trimmedQuery.length);

    // return react fragments in formatted order.
    // if start index was greater than zero, begin return value with ellipses.
    // then substrings in order with query term in highlighted format.
    return (
        <>
            {start > 0 && "..."} {beforeMatch}
            <mark className="rounded bg-yellow-200 px-1"> {match} </mark>
            {afterMatch} {end < cleanedText.length && "..."}
        </>
    );
}

// Maps search results to UI elements.
export default function SearchResults({ results, query }: SearchResultProps) 
{
    // calculate number pages and where current page starts.
    const [currentPage, setCurrentPage] = useState(1);

    const totalPages = Math.ceil(results.length / RESULTS_PER_PAGE);

    const startIndex = (currentPage - 1) * RESULTS_PER_PAGE;

    // get part of array that will be represented on the page.
    const pageResults = results.slice(
        startIndex,
        startIndex + RESULTS_PER_PAGE
    );
    
    // individual result entry formatting.
    // pagination buttons receive react state variables.
    return (
        <div>
            <div className="space-y-4">
                {pageResults.map((result) => (
                    <article
                        key={result.record.id}
                        className="w-full rounded-lg border p-4">

                        {result.record.name?.trim() && (
                            <h3>[Name: {result.record.name.trim()}]</h3>
                        )}

                        {result.record.type?.trim() && (
                            <h3>[Record Type: {result.record.type.trim()}]</h3>
                        )}

                        {result.record.speakerId?.trim() && (
                            <h3>[Speaker: {result.record.speakerId.trim()}]</h3>
                        )}

                        <br />

                        <p>{createExcerpt(result.record.text, query)}</p>

                        <Link
                            to={`/records/${result.record.id}`}
                            className="mt-3 inline-block text-blue-600 hover:underline"
                        >
                            View full record
                        </Link>
                        
                    </article>
                ))}
            </div>
            
            <div className="mt-6 flex justify-center gap-4">
                <button
                    disabled={currentPage === 1}
                    onClick={() => setCurrentPage(currentPage - 1)}
                >
                    Previous
                </button>
        
                <span>
                            Page {currentPage} of {totalPages}
                        </span>
        
                <button
                    disabled={currentPage === totalPages}
                    onClick={() => setCurrentPage(currentPage + 1)}
                >
                    Next
                </button>
            </div>
        </div>
    );
}