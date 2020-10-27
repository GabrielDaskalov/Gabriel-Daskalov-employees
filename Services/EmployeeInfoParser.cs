using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Employees.Services
{
    public class EmployeeInfoParser
    {
        public static List<Employee> GetEmployeesFromDatatable(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }

            List<Employee> list = new List<Employee>();
            
            foreach (DataRow dataRow in dt.Rows)
            {
                Employee e = new Employee();
                e.Id = (int)dataRow.ItemArray[0];
                e.ProjectId = (int)dataRow.ItemArray[1];
                e.DateFrom = (DateTime)dataRow.ItemArray[2];
                if (dataRow.ItemArray[3] != DBNull.Value)
                {
                    e.DateTo = (DateTime)dataRow.ItemArray[3];
                }

                list.Add(e);
            }
            return list;
        }

    }
}
