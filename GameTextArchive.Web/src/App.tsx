import { BrowserRouter, Route, Routes } from "react-router-dom";

import Home from "./pages/Home";
import SearchResultsPage from "./pages/SearchResults";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/search" element={<SearchResultsPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;