import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";
import "./index.css";

// literally just an entry point for react app.
// root element for the visual tree. 
createRoot(document.getElementById("root")!).render(<StrictMode><App /></StrictMode>);