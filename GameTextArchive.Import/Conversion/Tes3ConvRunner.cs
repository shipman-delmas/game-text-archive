using System.Diagnostics;

namespace GameTextArchive.Import.Conversion;

// wrapper for external tes3conv.exe
public class Tes3ConvRunner
{
    public void Execute(string inputFile, string outputFile, bool compact = false, bool overwrite = false)
    {
        try
        {
            // creates process object and sets start info attributes before calling start method.
            using Process process = new();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "tes3conv.exe";
            process.StartInfo.Arguments = $"\"{inputFile}\" \"{outputFile}\"";
            process.StartInfo.CreateNoWindow = false;

            process.Start();
            process.WaitForExit();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
        }
    }
}