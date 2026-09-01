import { useState } from "react";
import type { SearchResult } from "../types/SearchResult";

interface SearchResultProps {
    results: SearchResult[];
}

// FIX: MOVE PAGINATION TO SEARCH API.
const RESULTS_PER_PAGE = 10;

// if text not null, find and replace formatting tags.
function cleanText(text: string | null): string {
    if (text === null) {
        return "None";
    }
    
    return text
        .replace(/<BR\s*\/?>/gi, "\n")
        .replace(/<[^>]*>/g, "")
        .trim();
}

// Maps search results to UI elements.
export default function SearchResults({ results }: SearchResultProps) 
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
                        
                        <h3> [Record Type: {result.record.type}] </h3>
                        
                        <h3> [Speaker: {result.record.speakerId?.trim() || "none"}] </h3>
                        
                        <p> {cleanText(result.record.text)} </p>
                        
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