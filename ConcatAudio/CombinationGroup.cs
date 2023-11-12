using CodeJam;
using CodeJam.Collections;

namespace ConcatAudio;

public class CombinationGroup
{
    public IReadOnlyCollection<string> InputFilePaths => _inputFilePaths.AsReadOnly();
    public string OutputFilePath { get; }
    public long OutputSize { get; private set; }

    private readonly List<string> _inputFilePaths = new();

    public CombinationGroup(string outputFilePath)
    {
        OutputFilePath = outputFilePath;
    }

    public CombinationGroup AddInputFile(FileInfo fi)
    {
        Code.NotNull(fi, nameof(fi));
        if (fi.Exists)
        {
            _inputFilePaths.Add(fi.FullName);
            OutputSize += fi.Length;
        }

        return this;
    }

    public CombinationGroup AddInputFiles(IEnumerable<FileInfo> fis)
    {
        foreach (var fi in fis.EmptyIfNull())
        {
            AddInputFile(fi);
        }

        return this;
    }
}