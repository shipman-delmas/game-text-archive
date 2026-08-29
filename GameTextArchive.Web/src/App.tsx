function App() {
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
                className="flex-1 rounded-md border bg-white px-4 py-2"
            />

            <button
                className="rounded-md bg-black px-5 py-2 text-white"
            >
              Search
            </button>
          </div>
        </div>
        
        <p className="fixed bottom-2 right-3 text-xs text-black/70">
          Background: Michael Kirkbride / Bethesda Softworks — via UESP.
        </p>
      </main>
  )
}

export default App