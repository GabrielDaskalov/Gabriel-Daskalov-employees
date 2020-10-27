using Employees.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Employees.Services
{
    public class Employee: EmployeeModel
    {
        public Employee()
        {
            DateTo = DateTime.Now.Date;

        }


    }
}
