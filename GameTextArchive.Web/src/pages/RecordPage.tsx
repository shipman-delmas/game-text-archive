import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import type { SearchResult } from "../types/SearchResult";

function cleanText(text: string | null): string 
{
    if (text === null) 
    {
        return "";
    }

    return text
        .replace(/<BR\s*\/?>/gi, "\n")
        .replace(/<[^>]*>/g, "")
        .trim();
}

// dynamically displays webpage for single record.
export default function RecordPage() {

    // use params router hook pulls id from encoded url.
    const { id } = useParams();

    // create react state for retrieved record. 
    const [record, setRecord] = useState<SearchResult["record"] | null>(null);

    // when record page is rendered run api request.
    useEffect(() => {
        async function fetchRecord() 
        {
            // fetches from backend endpoint for specific record guid. 
            const response = await fetch(`http://localhost:5080/api/records/${id}`);

            // basic not found exception if no response from backend api.
            if (!response.ok) {throw new Error("Record not found.");}

            // convert response to json.
            const data = await response.json();

            // pass json data into react state.
            setRecord(data);
        }

        fetchRecord();

        // dependency array.
        // if the id changes, react rerenders the page based on new id and request to api.
        // this makes it dynamic.
    }, [id]);

    if (!record) {
        return <p>Record not found.</p>;
    }

    return (
        <main className="min-h-screen p-8">

            <div className="mx-auto max-w-4xl">

                <article className="rounded-lg border p-6">

                    {record.name?.trim() && (
                        <h2>
                            [Name: {record.name.trim()}]
                        </h2>
                    )}

                    {record.type?.trim() && (
                        <h3>
                            [Record Type: {record.type.trim()}]
                        </h3>
                    )}

                    {record.speakerId?.trim() && (
                        <h3>
                            [Speaker: {record.speakerId.trim()}]
                        </h3>
                    )}

                    <br />

                    <p>
                        {cleanText(record.text)}
                    </p>

                </article>

            </div>

        </main>
    );
}