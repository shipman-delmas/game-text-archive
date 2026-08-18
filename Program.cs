using GameTextArchive.Config;
using GameTextArchive.Data;
using GameTextArchive.Search;

public class Program
{
    public static void Main(string[] args)
    {
        // object references.
        Program program = new();
        DbContext context = new();
        RecordFieldDefinitions definitions = new();
        JsonImporter importer = new(definitions);

        // temporary methods for testing. 
        program.TestImporter(context, definitions, importer);
    }

    public void TestImporter(DbContext context, RecordFieldDefinitions definitions, JsonImporter importer)
    {
        var running = true;
        
        
    }
}