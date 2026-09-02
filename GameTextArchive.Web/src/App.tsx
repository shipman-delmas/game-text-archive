import { BrowserRouter, Route, Routes } from "react-router-dom";
import RecordPage from "./pages/RecordPage";

import Home from "./pages/HomePage";
import SearchResultsPage from "./pages/ResultsPage";

// literally just for routing web pages now. 
function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/search" element={<SearchResultsPage />} />

                <Route
                    path="/records/:id"
                    element={<RecordPage />}
                />
            </Routes>
        </BrowserRouter>
    );
}

export default App;