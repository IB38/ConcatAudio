using CodeJam;
using CodeJam.Strings;
using FFMpegCore;

namespace ConcatAudio
{
    public static class Program
    {
        public static async Task Main(params string[] args)
        {
            Code.AssertArgument(args.Length is >= 2 and <= 3, nameof(args), "Pass 2 or 3 arguments: input folder path, target output file path and optional -R for input folder recursion");
            var (inputFolderPath, outputFilePath) = (args[0], args[1]);
            var recursive = false;
            if (args.Length >= 3)
            {
                recursive = args[2].Equals("-R", StringComparison.OrdinalIgnoreCase);
                if(recursive)
                    Console.WriteLine("Recursion flag detected, input folder will be searched recursively");
            }

            var combiner = new AudioCombiner();
            var input = new CombinerInput(inputFolderPath, outputFilePath, recursive);
            var resultFilePaths = await combiner.CombineAsync(input);
            Console.WriteLine($"Output file(s):{Environment.NewLine}{resultFilePaths.Join(Environment.NewLine)}");
        }
        
    }
}