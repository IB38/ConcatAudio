namespace ConcatAudio;

public class CombinerInput
{
    public string InputFolderPath { get; }
    public string OutputFilePath { get; }

    public CombinerInput(string inputFolderPath, string outputFilePath)
    {
        InputFolderPath = inputFolderPath;
        OutputFilePath = outputFilePath;
    }
}