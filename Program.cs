using Employees.NewFolder;
using System;

namespace Employees
{
    public class Program
    {

        public static void Main(string[] args)
        {

            DataFactory newData = new DataFactory();
            
            Console.WriteLine(newData.Result);

        }


    }
}
