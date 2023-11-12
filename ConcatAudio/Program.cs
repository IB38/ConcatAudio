using CodeJam;
using CodeJam.Strings;
using FFMpegCore;

namespace ConcatAudio
{
    public static class Program
    {
        public static async Task Main(params string[] args)
        {
            Code.AssertArgument(args.Length == 2, nameof(args), "Pass exactly 2 arguments: input folder path and target output file path");
            var (inputFolderPath, outputFilePath) = (args[0], args[1]);

            var combiner = new AudioCombiner();
            var input = new CombinerInput(inputFolderPath, outputFilePath);
            var resultFilePaths = await combiner.CombineAsync(input);
            Console.WriteLine($"Output file(s):{Environment.NewLine}{resultFilePaths.Join(Environment.NewLine)}");
        }
        
    }
}