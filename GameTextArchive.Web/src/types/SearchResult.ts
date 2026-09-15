// typescript interface similar to database table; defining properties and data types.
// interface is class; all classes interfaces. 
export interface SearchResult 
{
    record: {
        recordId: string;
        editorId: string;
        type: string;
        name: string;
        speakerId: string | null;
        text: string | null;
        sourceFile: string;
        importedAt: string;
        metadata: {
            data?: {
                dialogue_type?: string;
            };
        } | null;
    };
    rank: number;
}