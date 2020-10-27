using Employees.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Employees.NewFolder
{
    public class DataFactory
    {
        private const string DEFAULT_CSV_FILE = "employees.csv";
        private string initialDirectory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\..\\..\\Test").FullName;

        public DataFactory()
        {
            LoadData();
        }

        public string Result { get; private set; }

        private void LoadData()
        {
            string filename = initialDirectory + "\\" + DEFAULT_CSV_FILE;
            if (!File.Exists(filename))
            {
                throw new ArgumentException("File doesn't exist!");
            }

           Result =  FindData(filename);
        }

        private string FindData(string filename)
        {
            DataTable dt = null;

            try
            {
                dt = DataParser.GetDataTableFromCsv(filename);
            }
            catch
            {
                throw new FormatException("Error has occured while parsing the data!");
            }

            List<Employee> allEmployees = EmployeeInfoParser.GetEmployeesFromDatatable(dt);

            //method which finds all employees who works together
            var pairs = FindPairs(allEmployees);
            var bestPair = FindLongestWorkByPairs(pairs);

            if (bestPair == null)
            {
                return "There are no legit pairs!";
            }

            return bestPair.ToString();

        }

        //method for finding the pair, which has worked on more than one project and has most working days
        private static object FindLongestWorkByPairs(List<WorkTogether> pairs)
        {
            var longestWorkByPairs = pairs.GroupBy(gr => new { gr.Id1, gr.Id2 }).Select(
                g => new
                {
                    Id1 = g.Key.Id1,
                    Id2 = g.Key.Id2,
                    Days = g.Sum(s => s.Days), //sum per project for pair of employees
                    ProjectIDs = string.Join(",", g.Select(s => s.ProjectId))
                })
                .OrderByDescending(o => o.Days)
                .ToList();

            var bestPair = longestWorkByPairs
                .Where(lp => lp.ProjectIDs.Split(',').Count() > 1)
                .FirstOrDefault();

            return bestPair;
        }

        private List<WorkTogether> FindPairs(List<Employee> allEmployees)
        {
            var employeesGroupedByProject = allEmployees
                .GroupBy(u => u.ProjectId)
                .Select(grp => grp.OrderBy(o => o.DateFrom).ToList())
                .Where(lst => lst.Count > 1) //remove employees worked alone
                .ToList();

            List<WorkTogether> allWorkTogether = new List<WorkTogether>();

            foreach (List<Employee> employeesPerProject in employeesGroupedByProject)
            {
                var employeesByID = employeesPerProject
                    .GroupBy(u => u.Id)
                    .Select(grp => grp.ToList())
                    .ToList();

                for (int i = 0; i < employeesByID.Count; i++)
                {
                    var emplList1 = employeesByID[i];

                    foreach (Employee employee1 in emplList1)
                    {
                        DateTime start1 = employee1.DateFrom;
                        DateTime end1 = employee1.DateTo;

                        //comparison between current employee work and next employee work
                        for (int j = i + 1; j < employeesByID.Count; j++)
                        {
                            var emplList2 = employeesByID[j];

                            foreach (Employee employee2 in emplList2)
                            {
                                DateTime start2 = employee2.DateFrom;
                                DateTime end2 = employee2.DateTo;

                                if (start2 > end1)
                                {
                                    break;
                                }

                                if (start1 > end2 || end1 < start2)
                                {
                                    continue;
                                }

                                var daysTogether = CalcDaysTogetherForPair(start1, start2, end1, end2);

                                if (daysTogether > 0)
                                {
                                    allWorkTogether.Add(new WorkTogether
                                    {
                                        Id1 = employee1.Id,
                                        Id2 = employee2.Id,
                                        ProjectId = employee1.ProjectId,
                                        Days = daysTogether
                                    });
                                }
                            }
                        }
                    }
                }

            }
        //getting the pairs
            var workByPairs = allWorkTogether.GroupBy(gr => new { gr.Id1, gr.Id2, gr.ProjectId }).Select(
                g => new WorkTogether()
                {
                    Id1 = g.Key.Id1,
                    Id2 = g.Key.Id2,
                    Days = g.Sum(s => s.Days),
                    ProjectId = g.First().ProjectId
                }).OrderByDescending(o => o.Days).ToList();

            return workByPairs;
        }

        //method for calculating the count of days of the different pairs
        private int CalcDaysTogetherForPair(DateTime start1, DateTime start2, DateTime end1, DateTime end2)
        {
            int daysTogether = 0;

            if (start1 <= start2 && end1 <= end2)
            {
                daysTogether = CalcDaysDiff(start2, end1);
            }
            else if (start1 >= start2 && end1 >= end2)
            {
                daysTogether = CalcDaysDiff(start1, end2);
            }
            else if (start1 >= start2 && end1 <= end2)
            {
                daysTogether = CalcDaysDiff(start1, end1);
            }
            else if (start1 <= start2 && end1 >= end2)
            {
                daysTogether = CalcDaysDiff(start2, end2);
            }
            return daysTogether;
        }

        private int CalcDaysDiff(DateTime start, DateTime end)
        {
            return (int)(end - start).TotalDays;
        }

        //structure where I save the data for the different pairs
        private struct WorkTogether
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public int ProjectId { get; set; }
            public int Days { get; set; }

            public override string ToString()
            {
                return $"Id1={Id1} Id2={Id2} ProjectId={ProjectId} Days={Days} ";
            }
        }
    }


}
