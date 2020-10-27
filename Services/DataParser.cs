using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace Employees.Services
{
    public class DataParser
    {
        //I use this array to keep the different date formats
        public static string[] DateFormats = { "yyyy-MM-dd", "yyyy/MM/dd", "yyyy.MM.dd",
            "yyyy-dd-MM", "yyyy/dd/MM", "yyyy.dd.MM",
            "MM-dd-yyyy", "MM/dd/yyyy", "MM.dd.yyyy",
            "MMM-dd-yyyy", "MMM/dd/yyyy", "MMM.dd.yyyy",
            "MMMM-dd-yyyy", "MMMM/dd/yyyy", "MMMM.dd.yyyy" };

        public static DataTable GetDataTableFromCsv(string path, string dateFormat = "")
        {
            DataTable dt = new DataTable();
            dt.Locale = CultureInfo.CurrentCulture;

            dt.Columns.AddRange(new[]
            {
                new DataColumn("EmpID", typeof(int)),
                new DataColumn("ProjectID", typeof(int)),
                new DataColumn("DateFrom", typeof(DateTime)),
                new DataColumn("DateTo", typeof(DateTime))
            });

            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                string[] dateFormats = string.IsNullOrWhiteSpace(dateFormat) ? DateFormats : new[] { dateFormat };

                int line = 0;

                while (!parser.EndOfData)
                {
                    line++;

                    try
                    {
                        string[] fields = parser.ReadFields();

                        DataRow dr = dt.NewRow();
                        dr["EmpID"] = int.Parse(fields[0]);
                        dr["ProjectID"] = int.Parse(fields[1]);

                        DateTime time;

                        dr["DateFrom"] = DateTime.ParseExact(fields[2], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);

                        if (string.Compare(fields[3].Trim(), "null", StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            if (DateTime.TryParseExact(fields[3], dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                            {
                                dr["DateTo"] = time;
                            }
                            else
                            {
                                dr["DateTo"] = DateTime.Now;
                            }
                        }

                        dt.Rows.Add(dr);


                    }
                    catch (Exception ex)
                    {
                        throw new FormatException($"Invalid character in line {line}.", ex);

                        throw;
                    }


                }




            }




            return dt;
        }


    }
}
