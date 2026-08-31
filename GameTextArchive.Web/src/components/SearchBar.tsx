// class has method that receives string query and returns void.
import {useState} from "react";

interface SearchBarProps 
{
    onSearch: (query: string) => void;
}

// destructuring; get onSearch from props and pass it.
export default function SearchBar({ onSearch }: SearchBarProps) 
{
    // create react state that updates query components.
    const [query, setQuery] = useState("");
    
    return (
        <div>
            <input
            type='text'
            placeholder='Search...'
            value='{query}'
            // change to text box sets query to string value. button passes query set by setQuery to onSearch.
            onChange={(event) => setQuery(event.target.value)}
            />
            
            
            <button onClick={() => onSearch(query)}>
                Search 
            </button>
        </div>
    );
}