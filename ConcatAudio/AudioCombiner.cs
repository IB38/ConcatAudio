using CodeJam;
using CodeJam.Strings;
using CodeJam.Threading;
using FFMpegCore;

namespace ConcatAudio;

public class AudioCombiner
{
    public const long TargetFileSize = 7 * 1024 * 1024;
    
    /// <summary>
    /// Combine audio files (wav/mp3) to a single wav file.<br/>
    /// If result file is larger than <see cref="TargetFileSize"/> bytes, input files will be combined evenly into multiple files smaller than that threshold.
    /// </summary>
    /// <param name="input">Comber input.<br/>Note: result output file path is not guaranteed to be equal to one from <see cref="CombinerInput.OutputFilePath"/>.</param>
    /// <returns>Array of result file paths leading to result wav files.</returns>
    public async Task<string[]> CombineAsync(CombinerInput input)
    {

        var groups = AssembleCombinationGroups(input);

        return await groups.Select(FfmpegConcatAsync).WhenAll();
        
        async Task<string> FfmpegConcatAsync(CombinationGroup group)
        {
            Console.WriteLine($"[{group.InputFilePaths.Select(Path.GetFileName).Join(", ")}] -> {Path.GetFileName(group.OutputFilePath)}");
            var success = await FFMpegArguments
                .FromDemuxConcatInput(group.InputFilePaths)
                .OutputToFile(group.OutputFilePath, true, opt => opt.CopyChannel())
                .ProcessAsynchronously();
            
            Code.AssertState(success, "FFMpeg call failed");

            return group.OutputFilePath;
        }
    }

    private List<CombinationGroup> AssembleCombinationGroups(CombinerInput input)
    {
        var dirInfo = new DirectoryInfo(input.InputFolderPath);
        var extensions = new[] { "*.mp3", "*.wav" };
        var files = extensions.SelectMany(ext => dirInfo.GetFiles(ext)).OrderByDescending(f => f.Length).ToArray();
        var totalSize = files.Sum(f => f.Length);

        if (totalSize <= TargetFileSize)
        {
            Console.WriteLine($"Combined size {ToKb(totalSize)} kb is less than target size {ToKb(TargetFileSize)} kb, all input will be combined into one audio file");
            return new List<CombinationGroup>() { new CombinationGroup(input.OutputFilePath).AddInputFiles(files) };
        }

        var groupCount = (long) Math.Ceiling((double)totalSize / TargetFileSize);
        Console.WriteLine($"File count = {files.Length}, total size = {totalSize}, group count = {groupCount}");
        if(groupCount > files.Length)
        {
            Console.WriteLine($"WARN: Group count required for target file size {TargetFileSize} is {groupCount}, more than number of files ({files.Length}), target file size most likely won't be achieved");
            groupCount = files.Length;
        }

        var absOutputPath = Path.GetFullPath(input.OutputFilePath);
        var outputDir = Path.GetDirectoryName(absOutputPath)!;
        var baseFileName = Path.GetFileNameWithoutExtension(absOutputPath);
        var ext = Path.GetExtension(absOutputPath);
        
        var groups = new List<CombinationGroup>();
        for (var i = 0; i < groupCount; i++)
        {
            var postfix = $"{i + 1}";
            var fn = $"{baseFileName}-{postfix}{ext}";
            var path = Path.Combine(outputDir, fn);
            
            groups.Add(new CombinationGroup(path));
        }

        foreach (var f in files)
        {
            var group = groups.MinBy(g => g.OutputSize)!;
            group.AddInputFile(f);
        }

        return groups;
    }

    private double ToKb(long byteCount) => (double) byteCount / 1024;
}