using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace FileParser.Interface
{
    public interface IFileParserService
    {
        List<string> GetFiles(string folderPath, string fileName);
        DataTable GetResultDataTable(string filePath);
        string ConvertToCsv(DataTable datatable);
    }
}
