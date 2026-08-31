// typescript interface similar to database table; defining properties and data types.
// interface is class; all classes interfaces. 
export interface SearchResult 
{
    record: {
        id: string;
        editorId: string;
        type: string;
        speakerId: string | null;
        text: string | null;
        sourceFile: string;
        importedAt: string;
        metadata: Record<string, unknown> | null;
    };
    rank: number;
}