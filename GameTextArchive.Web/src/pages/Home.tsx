import { useState } from "react";
import { useNavigate } from "react-router-dom";

// moved search function/coordination from app.tsx.
function Home() {
    const [query, setQuery] = useState("");
    const navigate = useNavigate();

    function handleSearch() {
        if (query.trim() === "") {
            return;
        }

        navigate(`/search?query=${encodeURIComponent(query)}`);
    }

    return (
        <main className="home-background min-h-screen p-8">
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
                        type="button"
                        onClick={handleSearch}
                        className="rounded-md bg-black px-5 py-2 text-white"
                    >
                        Search
                    </button>
                </div>
            </div>
        </main>
    );
}

export default Home;