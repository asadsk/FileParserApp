
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace FileParser.UnitTests
{
    class TestModel
    {
        public List<Output> Data { get; set; }
    }

    public class Output

    {

        public string ISIN { get; set; }
        public string CFICode { get; set; }
        public string Venue { get; set; }
        public int ContractSize { get; set; }

    }

    public class Helper
    {

        public DataTable ToDataTable<T>(List<T> items)

        {

            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)

            {

                //Setting column names as Property names

                dataTable.Columns.Add(prop.Name);

            }

            foreach (T item in items)

            {

                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)

                {

                    //inserting property values to datatable rows

                    values[i] = Props[i].GetValue(item, null);

                }

                dataTable.Rows.Add(values);

            }

            //put a breakpoint here and check datatable

            return dataTable;

        }
    }
}
