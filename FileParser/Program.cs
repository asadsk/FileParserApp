using FileParser.Implementation;
using FileParser.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using System.IO;

namespace FileParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string FolderPath = args[0]
                 , FileName = args[1]
                 , Extension = args[2]
                 , OutputPath = args[3]
                 , OutputFileName = args[4];


            var serviceProvider = new ServiceCollection()
                       .AddLogging()
                       .AddSingleton<IFileParserService, FileParserService>()
                       .BuildServiceProvider();



            var service = serviceProvider.GetService<IFileParserService>();

            var FilesToParse = service.GetFiles(FolderPath, FileName); // Retrieves a list of files if multiple files with same name is found.


            if (FilesToParse.Count == 0)
                throw new Exception("File(s) not found for regex " + FileName + $".{Extension}");

            DataTable result = service.GetResultDataTable(FilesToParse[0]); // Passing only one file, but can be improved later to merge multiple input files into one output file.

            OutputPath = Path.Combine(OutputPath, OutputFileName);
            File.WriteAllText(OutputPath, service.ConvertToCsv(result));
        }
    }
}
