namespace ConcatAudio;

public class CombinerInput
{
    public string InputFolderPath { get; }
    public string OutputFilePath { get; }
    public bool Recursive { get; }

    public CombinerInput(string inputFolderPath, string outputFilePath, bool recursive = false)
    {
        InputFolderPath = inputFolderPath;
        OutputFilePath = outputFilePath;
        Recursive = recursive;
    }
}