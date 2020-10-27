using System;
using System.Collections.Generic;
using System.Text;

namespace Employees.Model
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }


    }
}
