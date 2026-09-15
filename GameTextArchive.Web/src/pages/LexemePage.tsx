import {useParams} from "react-router-dom";
import {useEffect, useState} from "react";

import type {Lexeme} from "../types/Lexeme.ts";

// probably doesnt work.
export default function LexemePage() 
{
    // gets id as parameter from url. 
    // sets react state for lexeme and setter method.
    
    const { id } = useParams();
    
    const [lexeme, setLexeme] = useState<Lexeme | null>(null);
    
    // when page is rendered, api call is sent. 
    
    useEffect(() => 
    {
        async function fetchLexeme() 
        {
            // check api endpoints if not working. 
            
            const response = await fetch('http://localhost:5080/api/lexemes/${id}');
            
            if (!response.ok) { throw new Error("Lexeme not found."); }
            
            const data = await response.json();
            
            setLexeme(data);
        }
        
        fetchLexeme();
        
        // dependency array rerenders page when lexeme id changes. 
    }, [id]);
    
    if (!lexeme) 
    {
        return <p> Lexeme not found. </p>;
    }
    
    // NOT YET TESTED.
    return (
        <main className="min-h-screen p-8">

            <div className="mx-auto max-w-4xl">

                <article className="rounded-lg border p-6">

                    {lexeme.value?.trim() && (
                        <h2>
                            {lexeme.value.trim()}
                        </h2>
                    )}
                    
                    <h2>
                        {lexeme.frequency}
                    </h2>
                    
                </article>
                
            </div>
            
        </main>
    );
}