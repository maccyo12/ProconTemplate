using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProconTemplate
{
    class Program
    {
        static string GetRunningPath() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static void Main(string[] args)
        {
            // ファイルチェック
            var fileList = Directory.GetFiles(Path.Combine(GetRunningPath(), "test_data"));
            var prefixList = fileList.Select(x => Path.GetFileName(x).Split('_')[0]).Distinct();
                             
            var filePairList = new List<(string inputPath,string outputPath)>();
            foreach(var prefix in prefixList)
            {
                var inputPath = fileList.FirstOrDefault(x => Path.GetFileName(x) == $"{prefix}_in.txt");
                var outputPath = fileList.FirstOrDefault(x => Path.GetFileName(x) == $"{prefix}_out.txt");
                if(inputPath != null && outputPath != null)
                {
                    filePairList.Add((inputPath, outputPath));
                }
                else
                {
                    Console.WriteLine($"No Pair : {Path.GetFileName(inputPath)}{Path.GetFileName(outputPath)}");
                }
            }

            if (!filePairList.Any())
            {
                Console.WriteLine("No File.");
                Console.ReadLine();
                return;
            }

            var defaultOutput = Console.Out;
            var defaultInput = Console.In;
            foreach (var (inputPath, outputPath) in filePairList)
            {
                var outputStream = new MemoryStream();
                Console.SetIn(new StreamReader(inputPath));
                Console.SetOut(new StreamWriter(outputStream));

                // 実際の処理
                var watch = Stopwatch.StartNew();
                Doing.Main(null);
                watch.Stop();

                Console.Out.Flush();

                // 判定
                outputStream.Position = 0;
                var consoleResult = new StreamReader(outputStream).ReadToEnd();
                var textResult = File.ReadAllText(outputPath);

                Console.SetOut(defaultOutput);
                Console.WriteLine($"{Path.GetFileName(inputPath)} -> {Path.GetFileName(outputPath)}");
                Console.WriteLine($"Result : {consoleResult == textResult}");
                Console.WriteLine($"Time : {watch.ElapsedMilliseconds}[ms]");
                Console.WriteLine();
            }

            Console.WriteLine("Finish. Press Enter Key...");
            Console.SetIn(defaultInput);
            Console.ReadLine();
        }
    }
}
