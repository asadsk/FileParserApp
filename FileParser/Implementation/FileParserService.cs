using FileParser.Interface;
using GenericParsing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FileParser.Implementation
{
    public class FileParserService : IFileParserService
    {

        /// <summary>
        /// Gets all the file present within the given directory.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public List<string> GetFiles(string folderPath, string fileName)
        {
            string[] Files;
            Files = Directory.GetFiles(folderPath);
            Regex FileRegex = new Regex(fileName);
            List<string> Result = new List<string>();
            foreach (string File in Files)
            {
                if (FileRegex.IsMatch(File.Replace(folderPath, "")))
                    Result.Add(File);
            }
            return Result;
        }

        /// <summary>
        /// Reads the file and converts it into desired datatable for clean CSV with columns {"ISIN", "CFICode", "Venue", "ContractSize"}
        /// Processes the complex field "Algo Params" and gets the contract size.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public DataTable GetResultDataTable(string filePath)
        {

            DataTable table = new DataTable();
            GenericParserAdapter genericParserAdapter = new GenericParserAdapter();

            genericParserAdapter.SetDataSource(filePath);
            genericParserAdapter.SkipStartingDataRows = 1;
            table = genericParserAdapter.GetDataTable();

            foreach (DataColumn column in table.Columns)
            {
                string cName = table.Rows[0][column.ColumnName].ToString();
                if (!table.Columns.Contains(cName) && cName != "")
                {
                    column.ColumnName = cName;
                }

            }

            table.Rows[0].Delete();

            foreach (DataRow dr in table.Rows)
            {
                if (!string.IsNullOrEmpty(dr["AlgoParams"].ToString()))
                {
                    string algoParams = dr["AlgoParams"].ToString();
                    var arrayOfItems = algoParams.Split('|');
                    string priceMultiplier = arrayOfItems.Where(x => x.Contains("PriceMultiplier")).FirstOrDefault();
                    string priceMultiplierValue = priceMultiplier.Split(':')[1];
                    Double.TryParse(priceMultiplierValue, out double contractSize);
                    dr["AlgoParams"] = contractSize;
                }

            }


            table.Columns["AlgoParams"].ColumnName = "ContractSize";
            string[] selectedColumns = new[] { "ISIN", "CFICode", "Venue", "ContractSize" };

            DataTable result = new DataView(table).ToTable(false, selectedColumns);

            return result;

        }


        /// <summary>
        /// Accepts datatable and converts it into CSV.
        /// </summary>
        /// <param name="datatable"></param>
        /// <returns></returns>
        public string ConvertToCsv(DataTable datatable)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = datatable.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in datatable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                {
                    string result = field.ToString();
                    result = result.Replace("\n", " ");
                    return result.IndexOf(',') != -1 ? $"\"{result}\"" : result;
                });
                sb.AppendLine(string.Join(",", fields));
            }

            var result = sb.ToString();
            return result;
        }
    }
}
